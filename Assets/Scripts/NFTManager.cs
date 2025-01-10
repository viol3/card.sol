using Cysharp.Threading.Tasks;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Solana.Unity.Metaplex.NFT.Library;
using Solana.Unity.Metaplex.Utilities;
using Solana.Unity.Programs;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;
using System.Collections.Generic;
using UnityEngine;

public class NFTManager : MonoBehaviour
{

    List<Nft> _nfts = new List<Nft>();

    public static NFTManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public async UniTask FetchNFTs(IRpcClient rpcClient, Account owner)
    {
        var tokenAccounts = await rpcClient.GetTokenAccountsByOwnerAsync(owner.PublicKey, null, TokenProgram.ProgramIdKey, Solana.Unity.Rpc.Types.Commitment.Confirmed);
        Debug.Log("Token Account Count : " + tokenAccounts.Result.Value.Count);
        _nfts = new List<Nft>();
        if (tokenAccounts.Result != null)
        {
            foreach (var account in tokenAccounts.Result.Value)
            {
                var accountInfo = account.Account.Data.Parsed.Info;
                var mintAddress = accountInfo.Mint;

                // Metadata hesabını al
                var metadata = await Nft.TryGetNftData(mintAddress, rpcClient);
                if (metadata != null)
                {
                    _nfts.Add(metadata);
                }
            }
        }

        for (var i = 0; i < _nfts.Count; i++)
        {
            Debug.Log(_nfts[i].metaplexData.data.metadata.name);
        }

    }

    public bool IsOwningNft(string name)
    {
        foreach (var nft in _nfts) 
        {
            if (nft.metaplexData.data.metadata.name.Equals(name))
            {
                return true;
            }
        }
        return false;
    }


    public async void MintNFT(IRpcClient rpcClient, Account payer)
    {
        var mint = new Account();
        var associatedTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(payer, mint.PublicKey);
        var metadata = new Metadata()
        {
            name = "Homing Fire",
            symbol = "HF1",
            uri = "https://bermuda.gs/nft/homingfire.json",
            sellerFeeBasisPoints = 500,
            creators = new List<Creator> { new(payer.PublicKey, 100, true) }
        };

        var blockHash = await rpcClient.GetLatestBlockHashAsync();
        var minimumRent = await rpcClient.GetMinimumBalanceForRentExemptionAsync(TokenProgram.MintAccountDataSize);

        var transaction = new TransactionBuilder()

        //TX'i son bloka eklemek için
        .SetRecentBlockHash(blockHash.Result.Value.Blockhash)

        //Fee'yi kim ödeyecek
        .SetFeePayer(payer)

        //Solana'daki her token, bir mint hesabı gerektirir. NFT'ler için bu hesap, token'ın özelliklerini (örneğin, tekil olmasını) tanımlar.
        //Her token için ayrı bir hesap oluşmak zorunda, diğer türlü çakışmalar meydana gelebilir.
        .AddInstruction(
            SystemProgram.CreateAccount(
                payer,
                mint.PublicKey,
                minimumRent.Result,//minimum SOL miktarı bu hesapta tutulması gerekiyor, kira ücreti gibi düşünülebilir.
                TokenProgram.MintAccountDataSize,//arz ve decimal gibi bilgileri depolamak için 82 bytes
                TokenProgram.ProgramIdKey))//TokenAccount işlemleri için bu program

            // Mint hesabını etkinleştirmek ve token'ın özelliklerini (örneğin, bölünebilirlik) belirlemek için.
            .AddInstruction(
                TokenProgram.InitializeMint(//Veri yapılarındaki initialize metotları gibi düşünülebilir, constructor görevini görür.
                    mint.PublicKey,
                    0,//decimal basamagi, sifir olmalıdır çünkü bölünemez olmalı
                    payer,
                    payer))


            //Solana'da token'lar, sahiplik ve transfer işlemleri için bir ATA gerektirir. Bu, token'ların sahibine özel bir hesap sağlar.
            //Token miktarı, token sahibinin adresi ve mint adresi bilgilerini tutar. Her token sahibine atanır.
            .AddInstruction(
                AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                    payer,
                    payer,
                    mint.PublicKey))

            //NFT'yi oluşturmak ve sahibine atamak için.
            .AddInstruction(
                TokenProgram.MintTo(
                    mint.PublicKey,
                    associatedTokenAccount,
                    1,//miktar
                    payer))

            //NFT'lerin değeri genellikle meta verilerine dayanır (örneğin, görseller, bilgiler). Bu adım, NFT'ye özgü meta verileri saklar.
            .AddInstruction(MetadataProgram.CreateMetadataAccount(
                PDALookup.FindMetadataPDA(mint),
                mint.PublicKey,
                payer,
                payer,
                payer.PublicKey,
                metadata,
                TokenStandard.NonFungible,
                true,
                true,
                null,
                metadataVersion: MetadataVersion.V3))

            //Master Edition, NFT'nin kopyalanamayacağını ve tekil olduğunu garanti eder.
            .AddInstruction(MetadataProgram.CreateMasterEdition(
                maxSupply: null,
                masterEditionKey: PDALookup.FindMasterEditionPDA(mint),
                mintKey: mint,
                updateAuthorityKey: payer,
                mintAuthority: payer,
                payer: payer,
                metadataKey: PDALookup.FindMetadataPDA(mint),
                version: CreateMasterEditionVersion.V3
            )
        );
        var txBytes = transaction.Build(new List<Account> { payer, mint });
        var txResult = await rpcClient.SendTransactionAsync(txBytes, false, Solana.Unity.Rpc.Types.Commitment.Confirmed);
        if (txResult.WasSuccessful)
        {
            Debug.Log("Minted NFT : " + txResult.Result);
        }
        else
        {
            Debug.LogError("Failed to mint: " + txResult.Reason);
        }

    }
}
