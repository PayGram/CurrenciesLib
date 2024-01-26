namespace CurrenciesLib.Cryptos
{
	public class CryptoUSDT : Crypto
	{
		private CryptoNetworks network;
		//decimal _div = 100;
		//public override ushort Precision { get => 2; set { } }

		public CryptoUSDT(CryptoNetworks network) : base(CryptoCurrencies.USDT, network, 2)
		{
			//if (network != CryptoNetworks.ERC20 && network != CryptoNetworks.Standard && network != CryptoNetworks.TRC20)
			//	throw new Exception($"{network} is not supported for USDT");
			if (network == CryptoNetworks.Standard) network = CryptoNetworks.ERC20;
			this.network = network;
		}
	}
}
