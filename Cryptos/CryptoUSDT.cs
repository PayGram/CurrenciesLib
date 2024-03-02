namespace CurrenciesLib.Cryptos
{
	public class CryptoUSDT : Crypto
	{
		public static ushort DefaultDecimals = 6;
		public CryptoUSDT(CryptoNetworks network) : base(CryptoCurrencies.USDT, network, DefaultDecimals)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.ERC20;
		}
	}
}
