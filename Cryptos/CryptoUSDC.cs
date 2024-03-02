namespace CurrenciesLib.Cryptos
{
	public class CryptoUSDC : Crypto
	{
		public static ushort DefaultDecimals = 16;
		public CryptoUSDC(CryptoNetworks network) : base(CryptoCurrencies.USDC, network, DefaultDecimals)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.ERC20;
		}
	}
}
