namespace CurrenciesLib.Cryptos
{
	public class CryptoBTC : Crypto
	{
		public static ushort DefaultDecimals = 8;
		public CryptoBTC() : base(CryptoCurrencies.BTC, CryptoNetworks.Standard, DefaultDecimals)
		{

		}
	}
}
