namespace CurrenciesLib.Cryptos
{
	public class CryptoFTN : Crypto
	{
		public static ushort DefaultDecimals = 18;
        public CryptoFTN(CryptoNetworks network) : base(CryptoCurrencies.FTN, network, DefaultDecimals)
		{
			if (network == CryptoNetworks.Standard)
				Network = CryptoNetworks.FNT20;
		}
	}
}
