namespace CurrenciesLib.Cryptos
{
	public class CryptoPHPT : Crypto
	{
		public static ushort DefaultDecimals = 18;
		public CryptoPHPT(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, DefaultDecimals)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.BEP20;
		}
	}
}
