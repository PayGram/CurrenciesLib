using System;

namespace CurrenciesLib.Cryptos
{
	public class CryptoPGD : Crypto
	{
		private CryptoNetworks network;
		//decimal _div = 100;
		public override ushort Precision { get => 2; set { } }
		public CryptoPGD(CryptoNetworks network) : base(CryptoCurrencies.PGD, network, 2, 1)
		{
			if (network != CryptoNetworks.ERC20 && network != CryptoNetworks.Standard && network != CryptoNetworks.TRC20)
				throw new Exception($"{network} is not supported for PGD");
			this.network = network;
		}
	}
}
