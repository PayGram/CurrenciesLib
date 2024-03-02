namespace CurrenciesLib.Cryptos
{
	public class CryptoETH : Crypto
	{
		public static ushort DefaultDecimals = 18;
		public CryptoETH() : base(CryptoCurrencies.ETH, CryptoNetworks.Standard, DefaultDecimals)
		{

		}
	}
}
