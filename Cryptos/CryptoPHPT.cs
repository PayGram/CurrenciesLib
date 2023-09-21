using System;

namespace CurrenciesLib.Cryptos
{
    public class CryptoPHPT : Crypto
    {
        private CryptoNetworks network;
        //decimal _div = 100;
        public override ushort Precision { get => 2; set { } }
        public CryptoPHPT(CryptoNetworks network) : base(CryptoCurrencies.PHPT, network, 2, 1)
        {
			//if (network != CryptoNetworks.BEP20 && network != CryptoNetworks.Standard && network != CryptoNetworks.BEP20)
			//    throw new Exception($"{network} is not supported for PHPT");
			if (network == CryptoNetworks.Standard) network = CryptoNetworks.BEP20;
			this.network = network;
        }
    }
}
