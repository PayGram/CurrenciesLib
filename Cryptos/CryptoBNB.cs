namespace CurrenciesLib.Cryptos
{
	public class CryptoBNB : Crypto
	{
		public CryptoBNB(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, 18)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.BEP20;
		}
	}
}
