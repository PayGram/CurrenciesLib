using System;
using System.Linq;

namespace CurrenciesLib.Cryptos
{
	public class Crypto : Currency, ICrypto
	{
		public readonly static Crypto ETH = new CryptoETH();// (CryptoCurrencies.ETH, CryptoNetworks.Standard, 18);
		public readonly static Crypto BTC = new CryptoBTC();//(CryptoCurrencies.BTC, CryptoNetworks.Standard, 8);
		public readonly static Crypto LTC = new CryptoLTC();//(CryptoCurrencies.LTC, CryptoNetworks.Standard, 8);
		public readonly static Crypto USDT = new CryptoUSDT(CryptoNetworks.Standard);//(CryptoCurrencies.USDT, CryptoNetworks.Standard, 2);
		public readonly static Crypto XMR = new CryptoXMR();//(CryptoCurrencies.XMR, CryptoNetworks.Standard, 12);
        public readonly static Crypto XRP = new CryptoXRP();//(CryptoCurrencies.XRP, CryptoNetworks.Standard, 6);
        public readonly static Crypto PHPT = new CryptoPHPT(CryptoNetworks.BEP20);//(CryptoCurrencies.XRP, CryptoNetworks.Standard, 6);


        public CryptoNetworks Network { get; set; }
		public CryptoCurrencies CryptoCurrencyId { get; set; }
		public override CurrencyTypes CurrencyType => CurrencyTypes.Crypto;
		/// <summary>
		/// Returns a representation of this Crypto currency on the form of any <see cref="Currencies"/>
		/// </summary>
		/// <returns></returns>
		public override string Symbol => CryptoCurrencyId.ToString();

		/// <summary>
		/// Returns a representation of this Crypto currency on the form of any <see cref="Currencies"/>_<see cref="CryptoCurrencies"/> such as USDT_ERC20
		/// </summary>
		public string SymbolAndNetwork => $"{CryptoCurrencyId}{CRYPTO_NETWORK_SEPARATOR}{Network}";

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public override ushort Precision { get; set; }

		public Crypto(CryptoCurrencies crypto, CryptoNetworks network, ushort precision, decimal howManyForOneUSD = decimal.MinValue)
			: base(howManyForOneUSD)
		{
			base.CurrencyId = (Currencies)crypto;
			this.CryptoCurrencyId = crypto;
			this.Network = network;
			this.Precision = precision;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="satoshi"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		public virtual decimal GetValueFromSatoshi(decimal satoshi)
		{
			if (Precision == 0) return satoshi;
			return satoshi / (decimal)Math.Pow(10, Precision);
		}

		public const char CRYPTO_NETWORK_SEPARATOR = '_';

		/// <summary>
		/// Gets a cryptocurrency by its name.
		/// </summary>
		/// <param name="cryptoAndNetwork">The name of the crypto, any of <see cref="CryptoCurrencies"/>.
		/// Or the name of the crypto and the name of the network separated by an <seealso cref="CRYPTO_NETWORK_SEPARATOR"/>, example: USDT_ERC20 </param>
		/// <returns>If the network is specified but it is not found, null is returned</returns>
		public static Crypto GetBySymbolAndNetwork(string cryptoAndNetwork)
		{
			if (string.IsNullOrEmpty(cryptoAndNetwork)) return null;
			var values = cryptoAndNetwork.Split(new char[] { CRYPTO_NETWORK_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

			if (values.Length == 0) return null;// should not happen
			string crypto = values[0];
			if (Enum.TryParse<CryptoCurrencies>(crypto, true, out CryptoCurrencies ccrypto) == false || !Enum.IsDefined(typeof(CryptoCurrencies), ccrypto))
				return null;

			CryptoNetworks cnetwork = CryptoNetworks.Standard;
			string network = null;
			if (values.Length > 1)
				network = values[1];
			if (network != null && Enum.TryParse<CryptoNetworks>(network, true, out CryptoNetworks newnet))
				cnetwork = newnet;
			else if (network != null)
				return null;

			return GetBySymbolAndNetwork(ccrypto, cnetwork);
		}

		/// <summary>
		/// Gets a cryptocurrency by its name and its network.
		/// If it is not supported by the platform, it will return null
		/// </summary>
		/// <param name="crypto"></param>
		/// <param name="network"></param>
		/// <returns></returns>
		public static Crypto GetBySymbolAndNetwork(CryptoCurrencies crypto, CryptoNetworks network = CryptoNetworks.Standard)
		{
			switch (crypto)
			{
				case CryptoCurrencies.BTC:
					return new Crypto(crypto, network, BTC.Precision);
				case CryptoCurrencies.ETH:
					return new Crypto(crypto, network, ETH.Precision);
				case CryptoCurrencies.LTC:
					return new Crypto(crypto, network, LTC.Precision);
				case CryptoCurrencies.USDT:
					return new Crypto(crypto, network, USDT.Precision);
				case CryptoCurrencies.XMR:
					return new Crypto(crypto, network, XMR.Precision);
				case CryptoCurrencies.XRP:
					return new Crypto(crypto, network, XRP.Precision);
				case CryptoCurrencies.PHPT:
					return new Crypto(crypto, network, PHPT.Precision);
			}
			return null;
		}

		public static new Currency GetBySymbol(Currencies symbol)
		{
			return Crypto.GetBySymbolAndNetwork((CryptoCurrencies)symbol);
		}

		/// <summary>
		/// Gets the currency type from its string representation
		/// </summary>
		/// <param name="symbolAndNetwork">The symbol and eventually the network separated by <see cref="Crypto.CRYPTO_NETWORK_SEPARATOR"/></param>
		/// <returns>The CryptoCurrencies value or CryptoCurrencies.Unknown if nothing was found</returns>
		public static new CryptoCurrencies GetCurrency(string symbolAndNetwork)
		{
			Currencies c = Currency.GetCurrency(symbolAndNetwork);
			if (Enum.IsDefined(typeof(CryptoCurrencies), (int)c)) return (CryptoCurrencies)c;
			return CryptoCurrencies.UNKNOWN;
		}
		public static string[] SupportedCryptoCurrenciesToArray()
		{
			return Enum.GetNames(typeof(CryptoCurrencies)).Where(x => !x.StartsWith(CryptoCurrencies.UNKNOWN.ToString())).ToArray();
		}

		public static string SupportedCryptoCurrencies()
		{
			return string.Join(", ", SupportedCryptoCurrenciesToArray());
		}
	}
}
