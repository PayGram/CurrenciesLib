namespace CurrenciesLib.Cryptos
{
	public class CryptoBNB : Crypto
	{
		public static ushort DefaultDecimals = 18;
		public CryptoBNB(CryptoNetworks network) : base(CryptoCurrencies.BNB, network, DefaultDecimals)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.BEP20;
		}
	}
}
