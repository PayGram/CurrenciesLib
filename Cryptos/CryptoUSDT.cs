﻿namespace CurrenciesLib.Cryptos
{
	public class CryptoUSDT : Crypto
	{

		public CryptoUSDT(CryptoNetworks network) : base(CryptoCurrencies.USDT, network, 16)
		{
			if (network == CryptoNetworks.Standard) 
				Network = CryptoNetworks.ERC20;
		}
	}
}
