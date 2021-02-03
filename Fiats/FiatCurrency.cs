using System;
using System.ComponentModel;
using System.Linq;

namespace CurrenciesLib.Fiats
{
	public class FiatCurrency : Currency
	{
		public readonly static FiatCurrency USD = new FiatCurrency(FiatCurrencies.USD, 1);
		public readonly static FiatCurrency AED = new FiatCurrency(FiatCurrencies.AED, 3.6725m);

		public FiatCurrencies FiatCurrencyId { get; set; }

		public override CurrencyTypes CurrencyType => CurrencyTypes.Fiat;
		public override string Symbol => FiatCurrencyId.ToString();
		public override ushort Precision { get; set; }

		public FiatCurrency(FiatCurrencies currency, decimal howManyForOneUSD = decimal.MinValue)
			: base(howManyForOneUSD)
		{
			Precision = 2;
			FiatCurrencyId = currency;
			base.CurrencyId = (Currencies)currency;
		}

		/// <summary>
		/// Gets a fiat currency by its symbol
		/// </summary>
		/// <param name="symbol">a strying representation of any <see cref="FiatCurrencies"/></param>
		/// <returns></returns>
		public static new Currency GetBySymbol(string symbol)
		{
			if (Enum.TryParse<FiatCurrencies>(symbol, true, out FiatCurrencies curr) == false || !Enum.IsDefined(typeof(FiatCurrencies), curr))
				return null;
			return GetBySymbol(curr);
		}

		/// <summary>
		/// Gets a FiatCurrency representing the type passed
		/// </summary>
		/// <param name="curr">The type of the fiat currency to get</param>
		/// <returns></returns>
		public static Currency GetBySymbol(FiatCurrencies curr)
		{
			switch (curr)
			{
				case FiatCurrencies.AED:
				case FiatCurrencies.CNY:
				case FiatCurrencies.EUR:
				case FiatCurrencies.GBP:
				case FiatCurrencies.PHP:
				case FiatCurrencies.USD:
				case FiatCurrencies.VND:
					return new FiatCurrency(curr);
				default:
					return null;
			}
		}

		public static new Currency GetBySymbol(Currencies symbol)
		{
			return FiatCurrency.GetBySymbol((FiatCurrencies)symbol);
		}

		/// <summary>
		/// Gets the currency type from its string representation
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static new FiatCurrencies GetCurrency(string symbol)
		{
			Currencies c = Currency.GetCurrency(symbol);
			if (Enum.IsDefined(typeof(FiatCurrencies), (int)c)) return (FiatCurrencies)c;
			return FiatCurrencies.UNKNOWN;
		}
		public static string[] SupportedFiatCurrenciesToArray()
		{
			return Enum.GetNames(typeof(FiatCurrencies)).Where(x => !x.StartsWith(FiatCurrencies.UNKNOWN.ToString())).ToArray();
		}

		public static string SupportedFiatCurrencies()
		{
			return string.Join(", ", SupportedFiatCurrenciesToArray());
		}
	}
}