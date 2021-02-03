namespace CurrenciesLib.Cryptos
{
	public class CryptoXMR : Crypto
	{
		decimal _div = 1000000000000;
		public CryptoXMR() : base(CryptoCurrencies.XMR, CryptoNetworks.Standard, 12)
		{

		}
	}
}
