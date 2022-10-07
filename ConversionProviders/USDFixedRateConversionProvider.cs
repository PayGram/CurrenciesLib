using System;
using System.Collections.Generic;

namespace CurrenciesLib.ConversionProviders
{
	public class USDFixedRateConversionProvider : ConversionProvider
	{
		Dictionary<Currencies, Dictionary<Currencies, Quote>> FixedConversions;

		public USDFixedRateConversionProvider()
		{
			FixedConversions = new Dictionary<Currencies, Dictionary<Currencies, Quote>>();

			var usd = new Dictionary<Currencies, Quote>();
			usd.Add(Currencies.AED, new Quote() { BaseCurrency = Currencies.USD, QuoteCurrency = Currencies.AED, Midpoint = 3.6725m });
			//usd.Add(Currencies.PGD, new Quote() { BaseCurrency = Currencies.USD, QuoteCurrency = Currencies.PGD, Midpoint = 1 });
			usd.Add(Currencies.USDT, new Quote() { BaseCurrency = Currencies.USD, QuoteCurrency = Currencies.USDT, Midpoint = 1 });

			FixedConversions.Add(Currencies.USD, usd);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <param name="amount"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		protected override TimedQuote getQuote(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			var quote = FixedConversions.ContainsKey(source) ? FixedConversions[source][dest] : FixedConversions[dest][source];

			// conditions of convertibility have been done already
			return new TimedQuote()
			{
				BaseCurrency = source,
				QuoteCurrency = dest,
				Midpoint = source == quote.BaseCurrency ? quote.Midpoint : 1 / quote.Midpoint,
				UpdatedAtUTC = DateTime.UtcNow
			};
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <returns></returns>
		public override bool CanConvert(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			return base.CanConvert(source, dest, convStatus)
				&& (FixedConversions.ContainsKey(source) && FixedConversions[source].ContainsKey(dest)
					|| FixedConversions.ContainsKey(dest) && FixedConversions[dest].ContainsKey(source));
		}
	}
}
