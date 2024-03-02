namespace CurrenciesLib.Cryptos
{
	public class CryptoTRX : Crypto
	{
		public static ushort DefaultDecimals = 6;
		public CryptoTRX(CryptoNetworks network) : base(CryptoCurrencies.TRX, network, DefaultDecimals)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.TRC20;
		}
	}
}
