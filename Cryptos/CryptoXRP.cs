namespace CurrenciesLib.Cryptos
{
	public class CryptoXRP : Crypto
	{
		public static ushort DefaultDecimals = 6;
		public CryptoXRP() : base(CryptoCurrencies.XRP, CryptoNetworks.Standard, DefaultDecimals)
		{
		}
	}
}
