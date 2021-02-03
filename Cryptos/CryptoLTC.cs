namespace CurrenciesLib.Cryptos
{
	public class CryptoLTC : Crypto
	{
		//decimal _div = 100000000;
		public override ushort Precision { get => 8; set { } }
		public CryptoLTC() : base(CryptoCurrencies.LTC, CryptoNetworks.Standard, 8)
		{

		}
	}
}
