namespace CurrenciesLib.Cryptos
{
	public class CryptoTRX : Crypto
	{
		private CryptoNetworks network;
		public override ushort Precision { get => 2; set { } }
		public CryptoTRX(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, 6)
		{
			if (network == CryptoNetworks.Standard) network = CryptoNetworks.TRC20;
			this.network = network;
		}
	}
}
