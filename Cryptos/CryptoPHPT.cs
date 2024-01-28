namespace CurrenciesLib.Cryptos
{
	public class CryptoPHPT : Crypto
	{
		public CryptoPHPT(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, 16)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.BEP20;
		}
	}
}
