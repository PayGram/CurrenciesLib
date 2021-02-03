namespace CurrenciesLib.Cryptos
{
	public class CryptoBTC : Crypto
	{

		public override ushort Precision { get => 8; set { } }
		public CryptoBTC() : base(CryptoCurrencies.BTC, CryptoNetworks.Standard, 8)
		{

		}
	}
}
