using CurrenciesLib.ConversionProviders;
using CurrenciesLib.Cryptos;
using CurrenciesLib.Fiats;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CurrenciesLib
{
	public abstract class Currency : ICurrency
	{
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public abstract string Symbol { get; }
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public abstract CurrencyTypes CurrencyType { get; }
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public bool IsFixedRateUSD => HowManyForOneUSD != decimal.MinValue;
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public bool IsCrypto => CurrencyType == CurrencyTypes.Crypto;
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public virtual Currencies CurrencyId { get; protected set; }
		/// <summary>
		/// <inheritdoc/>
		/// if <= 0, it will be assigned to decimal.minvalue
		/// </summary>
		public decimal HowManyForOneUSD { get => _howManyForOneUSD <= 0 ? decimal.MinValue : _howManyForOneUSD; set { _howManyForOneUSD = value; } }
		decimal _howManyForOneUSD;
		public abstract ushort Precision { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="howManyForOneUSD">The fixed rate against USD. if <= 0, it will be assigned to decimal.minvalue</param>
		public Currency(decimal howManyForOneUSD = decimal.MinValue)
		{
			HowManyForOneUSD = howManyForOneUSD;
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="destCurrency"><inheritdoc/></param>
		/// <param name="amount"><inheritdoc/></param>
		/// <returns>Rertuns decimal.MinValue if the conversion could not be done through the specified CurrencyConvertionProvider</returns>
		/// <exception cref="NullReferenceException">If destCurrency is null</exception>
		public virtual decimal ConvertTo(Currencies destCurrency, decimal amount)
		{
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;
			return ConversionProviderFactory.GetConversionProvider().Convert(this.CurrencyId, destCurrency, amount, null);
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="destCurrency"><inheritdoc/></param>
		/// <param name="amount"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		public virtual async Task<decimal> ConvertToAsync(Currencies destCurrency, decimal amount)
		{
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;
			return await ConversionProviderFactory.GetConversionProvider().ConvertAsync(this.CurrencyId, destCurrency, amount, null);
		}
		/// <summary>
		/// Asynchronously Converts an amount from the source currency to the destination currency using a specified conversionProvider
		/// </summary>
		/// <param name="source">The currency to convert the amount from</param>
		/// <param name="dest">The currency to convert the amount to</param>
		/// <param name="amount">The amount to convert. If amount == decimal.MinValue or decimal.MaxValue, the same will be returned</param>
		/// <returns>Returns The converted amount expressed in dest currency or decimal.MinValue if the conversion could not be done 
		/// through the specified CurrencyConvertionProvider</returns>
		/// <exception cref="NullReferenceException">If destCurrency or conversionProvider is null</exception>
		public decimal ConvertTo(ICurrencyConversionProvider conversionProvider, Currencies destCurrency, decimal amount)
		{
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;
			if (conversionProvider == null)
				throw new NullReferenceException("conversionProvider cannot be null");

			return conversionProvider.Convert(this.CurrencyId, destCurrency, amount, null);
		}
		/// <summary>
		/// Asynchronously Converts an amount from the source currency to the destination currency using a specified conversionProvider
		/// </summary>
		/// <param name="source">The currency to convert the amount from</param>
		/// <param name="dest">The currency to convert the amount to</param>
		/// <param name="amount">The amount to convert. If amount == decimal.MinValue or decimal.MaxValue, the same will be returned</param>
		/// <returns>The converted amount expressed in dest currency</returns>
		public async Task<decimal> ConvertToAsync(ICurrencyConversionProvider conversionProvider, Currencies destCurrency, decimal amount)
		{
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;
			if (conversionProvider == null)
				throw new NullReferenceException("conversionProvider cannot be null");

			return await conversionProvider.ConvertAsync(this.CurrencyId, destCurrency, amount, null);
		}
		/// <summary>
		/// Gets a crypto or a fiat currency by its symbol 
		/// </summary>
		/// <param name="symbol">Symbol is the string representing one of the <see cref="Currencies"/>.
		/// If the symbol represents a crypto currency it might be followed by the <see cref="Crypto.CRYPTO_NETWORK_SEPARATOR"/> and the network name 
		/// which is one of <see cref="CryptoCurrencies"/> for example USDT_ERC20</param>
		/// <returns>The currency associated with this symbol</returns>
		public static Currency GetBySymbol(string symbol)
		{
			Currency value = Crypto.GetBySymbolAndNetwork(symbol);
			if (value != null)
				return value;
			return FiatCurrency.GetBySymbol(symbol);
		}
		/// <summary>
		/// Gets a currency from its currency type. IF a crypto is passed, Standard network will be the default value 
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static Currency GetBySymbol(Currencies symbol)
		{
			Currency value = Crypto.GetBySymbolAndNetwork((CryptoCurrencies)symbol);
			if (value != null)
				return value;
			return FiatCurrency.GetBySymbol((FiatCurrencies)symbol);
		}
		/// <summary>
		/// Gets the currency type from its string representation
		/// </summary>
		/// <param name="symbolAndNetwork">The currency and eventually the network.</param>
		/// <returns></returns>
		public static Currencies GetCurrency(string symbolAndNetwork)
		{
			if (string.IsNullOrWhiteSpace(symbolAndNetwork)) return Currencies.UNKNOWN;
			string[] nameNetwork = symbolAndNetwork.Split(Crypto.CRYPTO_NETWORK_SEPARATOR);
			if (nameNetwork.Length == 0) return Currencies.UNKNOWN;
			string name = nameNetwork[0];
			if (Enum.TryParse<Currencies>(name, true, out Currencies res) && Enum.IsDefined(typeof(Currencies), res))
				return res;
			return Currencies.UNKNOWN;
		}
		public static bool CheckIsCrypto(Currencies c)
		{
			return Enum.IsDefined(typeof(CryptoCurrencies), (int)c);
		}
		public static bool CheckIsFiat(Currencies c)
		{
			return Enum.IsDefined(typeof(FiatCurrencies), (int)c);
		}
		public static bool operator ==(Currency lhs, object rhs)
		{
			// Check for null on left side.
			if (Object.ReferenceEquals(lhs, null))
			{
				if (Object.ReferenceEquals(rhs, null))
				{
					// null == null = true.
					return true;
				}

				// Only the left side is null.
				return false;
			}

			if (rhs is ICurrency == false)
				return false;

			// Equals handles case of null on right side.
			return lhs.Equals(rhs);
		}
		public static bool operator !=(Currency lhs, object rhs)
		{
			return !(lhs == rhs);
		}
		/// <summary>
		/// Converts one currency to another one
		/// </summary>
		/// <param name="srcCurrency">The source currency to convert from</param>
		/// <param name="destCurrency">The target currency to convert to</param>
		/// <param name="amount">The amount expressed in source currency</param>
		/// <returns>The amount expressed in destCurrency or decimal.MinValue when it was not possible to do the conversion</returns>
		public static async Task<decimal> ConvertAsync(Currencies srcCurrency, Currencies destCurrency, decimal amount)
		{
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;
			return await ConversionProviderFactory.GetConversionProvider().ConvertAsync(srcCurrency, destCurrency, amount, null);
		}
		/// <summary>
		/// Converts one currency to another one
		/// </summary>
		/// <param name="srcCurrency">The source currency to convert from</param>
		/// <param name="destCurrency">The target currency to convert to</param>
		/// <param name="amount">The amount expressed in source currency</param>
		/// <returns>The amount expressed in destCurrency or decimal.MinValue when it was not possible to do the conversion</returns>
		public static decimal Convert(Currencies srcCurrency, Currencies destCurrency, decimal amount)
		{
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;
			return ConversionProviderFactory.GetConversionProvider().Convert(srcCurrency, destCurrency, amount, null);
		}
		public override int GetHashCode()
		{
			CryptoNetworks net = CryptoNetworks.Standard;
			if (IsCrypto)
				net = ((ICrypto)this).Network;
			return HashCode.Combine(CurrencyId, net, IsCrypto);
		}
		public bool Equals(ICurrency curr)
		{
			if (curr == null) return false;
			int myhash = GetHashCode();
			int otherhash = curr.GetHashCode();
			return myhash == otherhash;
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ICurrency);
		}
		public override string ToString()
		{
			return IsCrypto ? ((ICrypto)this).SymbolAndNetwork : Symbol;
		}

		public static string[] SupportedCurrenciesToArray()
		{
			return Enum.GetNames(typeof(Currencies)).Where(x => !x.StartsWith(Currencies.UNKNOWN.ToString())).ToArray();
		}

		public static string SupportedCurrencies()
		{
			return string.Join(", ", SupportedCurrenciesToArray());
		}
		/// <summary>
		/// Gets the multiplier that represents the less significant digits of a currency
		/// whose amounts are usually long
		/// For example, for VND, the multiplier will be 1000 because the last 3 digits are the least significative 
		/// on the amounts in this currency
		/// </summary>
		/// <param name="curr">The curreny whose multiplier is needed</param>
		/// <returns></returns>
		public static double GetBaseMultiplier(Currencies curr)
		{
			switch (curr)
			{
				case Currencies.BTC:
					return 0.0001;

				case Currencies.ETH:
					return 0.001;

				case Currencies.VND:
					return 1000;

				default:
					return 1;
			}
		}
	}
}
