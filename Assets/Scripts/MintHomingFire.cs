using Solana.Unity.Rpc;
using Solana.Unity.Wallet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MintHomingFire : MonoBehaviour
{
    public void Mint()
    {
        IRpcClient client = WalletManager.Instance.GetRpcClient();
        Account payer = WalletManager.Instance.GetAccount();
        NFTManager.Instance.MintNFT(client, payer);
    }
}
