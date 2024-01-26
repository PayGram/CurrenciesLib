﻿namespace CurrenciesLib.Cryptos
{
	public class CryptoPHPT : Crypto
	{
		private CryptoNetworks network;
		public override ushort Precision { get => 2; set { } }
		public CryptoPHPT(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, 2)
		{
			if (network == CryptoNetworks.Standard) network = CryptoNetworks.BEP20;
			this.network = network;
		}
	}
}
