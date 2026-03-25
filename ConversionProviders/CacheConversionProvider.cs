using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CurrenciesLib.ConversionProviders
{
	public class CacheConversionProvider : ConversionProvider
	{
		readonly object sync = new object();
		/// <summary>
		/// Gets or sets the Time after which a quote is no longer valid. default <see cref="ConversionProviderFactory.DEFAULT_QUOTE_EXPIRATION_MILLIS"/>
		/// </summary>
		public ulong CacheExpirationMillis { get; set; }

		/// <summary>
		/// the key is the base currency and the value is a dictionari of quote currency (dest currency) and quotes that we can convert to
		/// </summary>
		readonly Dictionary<Currencies, Dictionary<Currencies, TimedQuote>> quotes = new Dictionary<Currencies, Dictionary<Currencies, TimedQuote>>();

		public List<TimedQuote> Quotes => quotes.Values.SelectMany(q => q.Values).ToList();

		public CacheConversionProvider(ulong cacheExpMillis = ConversionProviderFactory.DEFAULT_QUOTE_EXPIRATION_MILLIS)
		{
			CacheExpirationMillis = cacheExpMillis;
		}

		public override bool CanConvert(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			if (base.CanConvert(source, dest, convStatus) == false) return false;

			lock (sync)
			{
				bool containsQuote = quotes.ContainsKey(source) && quotes[source].ContainsKey(dest);
				bool isValid = containsQuote && quotes[source][dest].UpdatedAtUTC.AddMilliseconds(CacheExpirationMillis) >= DateTime.UtcNow;
				return containsQuote && isValid;
			}
		}

		protected override TimedQuote getQuote(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			var ret = quotes[source][dest];
			Debug.WriteLine($"Cache quote: {ret}");
			return ret;
		}

		public void UpdateCache(Quote quote, DateTime updatedAtUtc, bool inferOpposite = true)
		{
			if (quote.Midpoint == 0) return;
			var source = quote.BaseCurrency;
			var dest = quote.QuoteCurrency;

			lock (sync)
			{
				// add or update the quote
				var newq = new TimedQuote() { BaseCurrency = source, QuoteCurrency = dest, Midpoint = quote.Midpoint, UpdatedAtUTC = updatedAtUtc, IsInferred = quote.IsInferred, SpreadBuy = quote.SpreadBuy, SpreadSell = quote.SpreadSell };
				UpdateCache(newq);

				if (inferOpposite)
				{
					// add or update the opposite quote (swap buy/sell spread for opposite direction)
					newq = new TimedQuote() { BaseCurrency = dest, QuoteCurrency = source, Midpoint = 1 / quote.Midpoint, UpdatedAtUTC = updatedAtUtc, IsInferred = quote.IsInferred, SpreadBuy = quote.SpreadSell, SpreadSell = quote.SpreadBuy };
					UpdateCache(newq);
				}
			}
		}

		/// <summary>
		/// must be called from a synchronized statement
		/// </summary>
		/// <param name="source"></param>
		/// <param name="dest"></param>
		/// <param name="newq"></param>
		/// <returns></returns>
		private void UpdateCache(TimedQuote newq)
		{
			if (newq.Midpoint == 0) return;
			var source = newq.BaseCurrency;
			var dest = newq.QuoteCurrency;

			if (quotes.ContainsKey(source))
			{
				if (quotes[source].ContainsKey(dest))
				{
					var existing = quotes[source][dest];
					if (existing.UpdatedAtUTC < newq.UpdatedAtUTC)
					{
						existing.UpdatedAtUTC = newq.UpdatedAtUTC;
						existing.Midpoint = newq.Midpoint;
						existing.SpreadBuy = newq.SpreadBuy;
						existing.SpreadSell = newq.SpreadSell;
					}
				}
				else
				{
					quotes[source].Add(dest, newq);
				}
			}
			else
			{
				quotes.Add(source, new Dictionary<Currencies, TimedQuote>());
				quotes[source].Add(dest, newq);
			}
		}
	}
}
