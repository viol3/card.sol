Unity + Solana NFT Örnekleri
Bu proje, Unity oyunlarına Solana Blockchain entegrasyonu yaparak NFT oluşturma ve kullanma işlevselliği sağlamaktadır. Projede, kullanıcıların cüzdanlarına bağlanabileceği, NFT'lerini görüntüleyebileceği ve yeni NFT'ler basabileceği temel bir yapı sunulmaktadır.

> Not: Tamamen deneysel ve bilgilendirme amaçlı bir repodur. 

# Önemli Scriptler
# 1. NFTManager.cs
NFT'leri Listeleme: Kullanıcının cüzdanındaki tüm NFT'leri listelemek için FetchNFTs() fonksiyonu kullanılır.
NFT Sahiplik Kontrolü: Bir NFT'nin kullanıcıya ait olup olmadığını IsOwningNft() fonksiyonu ile kontrol edebilirsiniz.
NFT Oluşturma: Yeni bir NFT oluşturmak için MintNFT() fonksiyonu bulunmaktadır.
# 2. WalletManager.cs
Cüzdan Yönetimi:
Yeni hesap oluşturma: CreateAccount() fonksiyonu.
Giriş yapma: Login() fonksiyonu.

# SOL İşlemleri:
Bakiyeyi kontrol etme: CheckBalance() fonksiyonu.
Airdrop talebi: RequestAirdrop() fonksiyonu.
SOL transferi: TransferSOL() fonksiyonu.
Kurulum ve Kullanım
Unity Projesini Kurun:

Bu projeyi bir Unity projesine dahil edin.
Solana Unity SDK ve gerekli bağımlılıkların kurulu olduğundan emin olun.

# Cüzdan Girişi:

WalletManager üzerinden yeni bir cüzdan oluşturun veya mevcut bir cüzdanla giriş yapın.
NFT Oluşturma ve Görüntüleme:

NFTManager kullanarak cüzdanınızdaki NFT'leri görüntüleyin veya yeni NFT oluşturun.
Örnek Kullanım
Yeni bir NFT oluşturmak için:

```csharp
NFTManager.Instance.MintNFT(WalletManager.Instance.GetRpcClient(), WalletManager.Instance.GetAccount());
```
Kullanıcının bir NFT'ye sahip olup olmadığını kontrol etmek için:

```csharp
bool ownsNft = NFTManager.Instance.IsOwningNft("Homing Fire");
Debug.Log("Sahip mi? " + ownsNft);
```

İletişim
Eğer bu proje ile ilgili sorularınız varsa, lütfen GitHub Issues sekmesinden iletişime geçin.
