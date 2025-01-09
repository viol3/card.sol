using Solana.Unity.Metaplex.NFT.Library;
using Solana.Unity.Metaplex.Utilities;
using Solana.Unity.Programs;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Models;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NFTManager : MonoBehaviour
{
    private IRpcClient _rpcClient;
    private Wallet _wallet;
    private Account _payer;

    private void Start()
    {
        Initialize();
        //CreateMintAccount();
        //CreateMetadataAccount();
        MintNFT();
    }
    //EmghDekCuzvLy6wojW4sXXysHWoxc9qBHECERJJH2oif
    private void Initialize()
    {
        _rpcClient = ClientFactory.GetClient(Cluster.DevNet);
        _wallet = new Wallet("thing scene federal network isolate action hold hood involve hill sweet cute");
        _payer = _wallet.Account;
        Debug.Log(_payer.PrivateKey);
    }

    private async void MintNFT()
    {
        var mint = new Account();
        var associatedTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(_payer, mint.PublicKey);
        var metadata = new Metadata()
        {
            name = "Fireball",
            symbol = "CSNFIRE",
            uri = "https://bermuda.gs/nft/fireball.json",
            sellerFeeBasisPoints = 500,
            creators = new List<Creator> { new(_payer.PublicKey, 100, true) }
        };

        var blockHash = await _rpcClient.GetLatestBlockHashAsync();
        var minimumRent = await _rpcClient.GetMinimumBalanceForRentExemptionAsync(TokenProgram.MintAccountDataSize);

        var transaction = new TransactionBuilder()
        .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
        .SetFeePayer(_payer)
        .AddInstruction(
            SystemProgram.CreateAccount(
                _payer,
                mint.PublicKey,
                minimumRent.Result,
                TokenProgram.MintAccountDataSize,
                TokenProgram.ProgramIdKey))
            .AddInstruction(
                TokenProgram.InitializeMint(
                    mint.PublicKey,
                    0,
                    _payer,
                    _payer))
            .AddInstruction(
                AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                    _payer,
                    _payer,
                    mint.PublicKey))
            .AddInstruction(
                TokenProgram.MintTo(
                    mint.PublicKey,
                    associatedTokenAccount,
                    1,
                    _payer))
            .AddInstruction(MetadataProgram.CreateMetadataAccount(
                PDALookup.FindMetadataPDA(mint),
                mint.PublicKey,
                _payer,
                _payer,
                _payer.PublicKey,
                metadata,
                TokenStandard.NonFungible,
                true,
                true,
                null,
                metadataVersion: MetadataVersion.V3))
            .AddInstruction(MetadataProgram.CreateMasterEdition(
                maxSupply: null,
                masterEditionKey: PDALookup.FindMasterEditionPDA(mint),
                mintKey: mint,
                updateAuthorityKey: _payer,
                mintAuthority: _payer,
                payer: _payer,
                metadataKey: PDALookup.FindMetadataPDA(mint),
                version: CreateMasterEditionVersion.V3
            )
        );
        var txBytes = transaction.Build(new List<Account> { _payer, mint });
        var txResult = await _rpcClient.SendTransactionAsync(txBytes, false, Solana.Unity.Rpc.Types.Commitment.Confirmed);
        if (txResult.WasSuccessful)
        {
            Debug.Log("Mint account created: " + txResult.Result);
        }
        else
        {
            Debug.LogError("Failed to create mint account: " + txResult.Reason);
        }

    }

    private async Task<string> GetRecentBlockHash()
    {
        // 2. Recent Blockhash alın
        var blockhashResponse = await _rpcClient.GetLatestBlockHashAsync();
        if (!blockhashResponse.WasSuccessful)
        {
            Debug.LogError("Failed to get recent blockhash.");
            return null;
        }

        return blockhashResponse.Result.Value.Blockhash;
    }

    private async void CreateMintAccount()
    {
        
        var builders = new List<Account>();
        // Yeni bir mint hesabı oluşturun
        var mintAccount = new Account();

        // 1. Mint hesabını finanse etmek için gerekli lamports miktarını alın
        var rentExemption = await _rpcClient.GetMinimumBalanceForRentExemptionAsync(TokenProgram.MintAccountDataSize);
        if (!rentExemption.WasSuccessful)
        {
            Debug.LogError("Failed to get rent exemption amount.");
            return;
        }
        // 2. Recent Blockhash alın
        var blockhashResponse = await _rpcClient.GetLatestBlockHashAsync();
        if (!blockhashResponse.WasSuccessful)
        {
            Debug.LogError("Failed to get recent blockhash.");
            return;
        }
        
        var recentBlockhash = blockhashResponse.Result.Value.Blockhash ;
        builders.Add(_payer);
        builders.Add(mintAccount);
        // 3. Mint hesabını oluşturmak için bir işlem gönderin
        var transaction = new TransactionBuilder()
            .SetRecentBlockHash(await GetRecentBlockHash())
            .SetFeePayer(_payer)
            .AddInstruction(SystemProgram.CreateAccount(
                _payer,
                mintAccount,
                rentExemption.Result,
                TokenProgram.MintAccountDataSize,
                TokenProgram.ProgramIdKey
            ))
            .AddInstruction(TokenProgram.InitializeMint(
                mintAccount.PublicKey,
                0, // Decimals = 0 (NFT için)
                _payer.PublicKey,
                _payer.PublicKey
            ))
            .Build(builders);

        var txResult = await _rpcClient.SendTransactionAsync(transaction, false, Solana.Unity.Rpc.Types.Commitment.Confirmed);
        if (txResult.WasSuccessful)
        {
            Debug.Log("Mint account created: " + mintAccount.PublicKey);
            Debug.Log(mintAccount.PublicKey);
            Debug.Log(mintAccount.PrivateKey);
        }
        else
        {
            Debug.LogError("Failed to create mint account: " + txResult.Reason);
        }
    }

    private byte[] SerializeMetadata(dynamic metadata)
    {
        var nameBytes = Encoding.UTF8.GetBytes(metadata.name);
        var symbolBytes = Encoding.UTF8.GetBytes(metadata.symbol);
        var uriBytes = Encoding.UTF8.GetBytes(metadata.uri);

        var nameLength = (byte)nameBytes.Length;
        var symbolLength = (byte)symbolBytes.Length;
        var uriLength = (byte)uriBytes.Length;

        var serializedData = new List<byte>();

        // Metadata programının beklediği formata uygun verileri ekleyin
        serializedData.Add(nameLength);             // Name uzunluğu
        serializedData.AddRange(nameBytes);         // Name
        serializedData.Add(symbolLength);           // Symbol uzunluğu
        serializedData.AddRange(symbolBytes);       // Symbol
        serializedData.Add(uriLength);              // URI uzunluğu
        serializedData.AddRange(uriBytes);          // URI

        return serializedData.ToArray();
    }

    private async void CreateMetadataAccount()
    {
        var metadataProgramId = new PublicKey("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");
        Account mintAccount = new Account("ynMpX3DEMi5VHL8zzsocY8bSMbgirqoDmHaCtqHV6MN5wDji7TkimPhiaEziAYedWEwBogiRV1aaeLrxGwLiWtB", "GAeRvqGVML2XRktuLmQoiaueVXxp1r9h89Luq9q5esvs");
       
        PublicKey metadataKey = null;
        byte bump = 0;
        // Metadata hesabını hesapla
        bool result = PublicKey.TryFindProgramAddress(
            new List<byte[]>
            {
            Encoding.UTF8.GetBytes("metadata"),
            metadataProgramId.KeyBytes,
            mintAccount.PublicKey.KeyBytes
            },
            metadataProgramId,
            out metadataKey,out bump
        );
        if(!result)
        {
            Debug.Log("Error about trying to find program address.");
            return;
        }


        var rentExemption = await _rpcClient.GetMinimumBalanceForRentExemptionAsync(TokenProgram.MintAccountDataSize);
        if (!rentExemption.WasSuccessful)
        {
            Debug.LogError("Failed to get rent exemption amount.");
            return;
        }


        Debug.Log(metadataKey);
        // Metadata bilgileri (örneğin IPFS bağlantısını kullanın)
        var metadataUri = "https://bermuda.gs/nft/fireball.json";
        var builders = new List<Account>();
        builders.Add(_payer);
        builders.Add(mintAccount);
        var transaction = new TransactionBuilder()
            .SetRecentBlockHash(await GetRecentBlockHash())
            .SetFeePayer(_payer)
            .AddInstruction(SystemProgram.CreateAccount(
                _payer,
                mintAccount,
                rentExemption.Result,
                512,
                metadataProgramId
            ))
            .AddInstruction(new TransactionInstruction
            {
                ProgramId = metadataProgramId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.ReadOnly(mintAccount.PublicKey, true), // Mint hesabı
                    AccountMeta.Writable(metadataKey, false),           // Metadata hesabı (Writable)
                    AccountMeta.Writable(_payer.PublicKey, true)       // Payer hesabı (Writable)
                },
                Data = SerializeMetadata(new
                {
                    name = "Fireball",
                    symbol = "CSNFIRE",
                    uri = metadataUri
                }) // Metadata'yı byte array'e dönüştürerek yazın
            })
            .Build(builders);

        var txResult = await _rpcClient.SendTransactionAsync(transaction, false, Solana.Unity.Rpc.Types.Commitment.Confirmed);

        if (txResult.WasSuccessful)
        {
            
            Debug.Log("Metadata account created successfully : " + txResult.Result);
        }
        else
        {
            Debug.LogError("Failed to create metadata account: " + txResult.Reason);
            Debug.LogError("Transaction error details: " + txResult.RawRpcResponse);
        }
    }
}
