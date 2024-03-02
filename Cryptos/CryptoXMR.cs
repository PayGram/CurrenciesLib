namespace CurrenciesLib.Cryptos
{
	public class CryptoXMR : Crypto
	{
		public static ushort DefaultDecimals = 12;
		public CryptoXMR() : base(CryptoCurrencies.XMR, CryptoNetworks.Standard, DefaultDecimals)
		{
		}
	}
}
