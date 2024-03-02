namespace CurrenciesLib.Cryptos
{
	public class CryptoLTC : Crypto
	{
		public static ushort DefaultDecimals = 8;
		public CryptoLTC() : base(CryptoCurrencies.LTC, CryptoNetworks.Standard, DefaultDecimals)
		{

		}
	}
}
