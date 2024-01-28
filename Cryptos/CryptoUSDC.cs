namespace CurrenciesLib.Cryptos
{
	public class CryptoUSDC : Crypto
	{
		public CryptoUSDC(CryptoNetworks network) : base(CryptoCurrencies.USDT, network, 16)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.ERC20;
		}
	}
}
