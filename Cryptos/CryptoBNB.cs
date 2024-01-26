namespace CurrenciesLib.Cryptos
{
	public class CryptoBNB : Crypto
	{
		private CryptoNetworks network;
		public override ushort Precision { get => 18; set { } }
		public CryptoBNB(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, 18)
		{
			if (network == CryptoNetworks.Standard) network = CryptoNetworks.BEP20;
			this.network = network;
		}
	}
}
