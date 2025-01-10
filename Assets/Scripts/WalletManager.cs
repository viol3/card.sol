using Cysharp.Threading.Tasks;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using Solana.Unity.Wallet.Bip39;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    private InGameWallet _wallet;
    private Account _account;

    private const long SOL_LAMPORTS = 1000000000;

    private double _balance = 0;

    public static WalletManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        //await CreateAccount();
        await Login();
        //await RequestAirdrop();
        await CheckBalance();
        await NFTManager.Instance.FetchNFTs(_wallet.ActiveRpcClient, _account);
        //await TransferSOL("6sSG3bXrHpjHxmyKpQdNAqGWFuaj9pXpTYwjUu64Hb23", 0.15);
    }

    async UniTask CreateAccount()
    {
        Mnemonic mnemonic = new Mnemonic(WordList.English, WordCount.Twelve);
        _wallet = new InGameWallet(RpcCluster.DevNet, "https://api.devnet.solana.com", null, true);
        _account = await _wallet.CreateAccount(mnemonic.ToString(), "solanaboat123");
        Debug.Log(_account.PublicKey);
        Debug.Log(_account.PrivateKey);
        Debug.Log(mnemonic.ToString());
    }

    async UniTask Login()
    {
        _wallet = new InGameWallet(RpcCluster.DevNet, "https://api.devnet.solana.com", null, true);
        _account = await _wallet.Login("solanaboat123");
        Debug.Log(_account.PublicKey);
        Debug.Log(_account.PrivateKey);
        Debug.Log(_wallet.Mnemonic);
    }

    public Account GetAccount()
    {
        return _account;
    }

    public IRpcClient GetRpcClient()
    {
        return _wallet.ActiveRpcClient;
    }
    

    public double GetBalance()
    {
        return _balance;
    }

    async UniTask CheckBalance()
    {
        _balance = await _wallet.GetBalance();
        Debug.Log("Balance => " + _balance);
    }

    async UniTask RequestAirdrop()
    {
        RequestResult<string> result = await _wallet.RequestAirdrop();
        if (result.WasSuccessful)
        {
            Debug.Log("Airdrop succesful => " + result.Result);
        }
        else
        {
            Debug.LogWarning("Airdrop failed => " + result.Reason);
        }
    }

    

    public async UniTask<bool> TransferSOL(string targetAddress, double amount)
    {
        RequestResult<string> result = await _wallet.Transfer(new PublicKey(targetAddress), (ulong)(amount * SOL_LAMPORTS));
        if (result.WasSuccessful)
        {
            Debug.Log("Transfer succesful => " + result.Result);
            return true;
        }
        else
        {
            Debug.LogWarning("Transfer failed => " + result.Reason);
            return false;
        }
    }


}
