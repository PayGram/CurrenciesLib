using System;
using System.Linq;

namespace CurrenciesLib.Cryptos
{
	public class Crypto : Currency, ICrypto
	{
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
		public override ushort Decimals { get; set; }

		public Crypto(CryptoCurrencies crypto, CryptoNetworks network, ushort decimals, decimal howManyForOneUSD = decimal.MinValue)
			: base(howManyForOneUSD)
		{
			base.CurrencyId = (Currencies)crypto;
			this.CryptoCurrencyId = crypto;
			this.Network = network;
			this.Decimals = decimals;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="satoshi"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		public virtual decimal GetValueFromSatoshi(decimal satoshi)
		{
			if (Decimals == 0) return satoshi;
			return satoshi / (decimal)Math.Pow(10, Decimals);
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
					return new Crypto(crypto, network, CryptoBTC.DefaultDecimals);
				case CryptoCurrencies.ETH:
					return new Crypto(crypto, network, CryptoETH.DefaultDecimals);
				case CryptoCurrencies.LTC:
					return new Crypto(crypto, network, CryptoLTC.DefaultDecimals);
				case CryptoCurrencies.USDT:
					return new Crypto(crypto, network, CryptoUSDT.DefaultDecimals);
				case CryptoCurrencies.XMR:
					return new Crypto(crypto, network, CryptoXMR.DefaultDecimals);
				case CryptoCurrencies.XRP:
					return new Crypto(crypto, network, CryptoXRP.DefaultDecimals);
				case CryptoCurrencies.PHPT:
					return new Crypto(crypto, network, CryptoPHPT.DefaultDecimals);
				case CryptoCurrencies.BNB:
					return new Crypto(crypto, network, CryptoBNB.DefaultDecimals);
				case CryptoCurrencies.TRX:
					return new Crypto(crypto, network, CryptoTRX.DefaultDecimals);
				case CryptoCurrencies.USDC:
					return new Crypto(crypto, network, CryptoUSDC.DefaultDecimals);
				case CryptoCurrencies.FTN:
					return new Crypto(crypto, network, CryptoFTN.DefaultDecimals);
			}
			return null;
		}
		public bool IsValidAddress(string address)
		{
			if (string.IsNullOrWhiteSpace(address)) return false;
			return CryptoHelper.IsValidAddress(Network, address);
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
