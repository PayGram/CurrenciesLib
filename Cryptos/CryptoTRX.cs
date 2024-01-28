namespace CurrenciesLib.Cryptos
{
	public class CryptoTRX : Crypto
	{
		public CryptoTRX(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, 6)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.TRC20;
		}
	}
}
