namespace CurrenciesLib.Cryptos
{
	public class CryptoETH : Crypto
	{
		//decimal _div = 1000000000000000000;
		public override ushort Precision { get => 18; set { } }
		public CryptoETH() : base(CryptoCurrencies.ETH, CryptoNetworks.Standard, 18)
		{

		}
	}
}
