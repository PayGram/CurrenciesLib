using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrenciesLib.ConversionProviders
{
	public static class ConversionProviderFactory
	{
		public const ulong DEFAULT_QUOTE_EXPIRATION_MILLIS = 5 * 60 * 1000;
		readonly static List<ICurrencyConversionProvider> providers;
		readonly static RateGraph rateGraph;

		/// <summary>
		/// The graph-based rate provider. Backward-compatible: callers that used CacheConversionProvider
		/// can use this property with the same UpdateCache/Quotes API.
		/// </summary>
		public static RateGraph CacheConversionProvider => rateGraph;

		public static bool UseCache
		{
			get => providers.Contains(rateGraph);
			set
			{
				if (value && providers.Contains(rateGraph) == false)
					providers.Insert(0, rateGraph);
				else if (value == false)
					providers.Remove(rateGraph);
			}
		}

		/// <summary>
		/// The number of milliseconds after which a quote is no longer valid. Setting this value
		/// will update all the ConversionProviders managed by this factory. Must be greater than 0, otherwise
		/// it will be set to <see cref="DEFAULT_QUOTE_EXPIRATION_MILLIS"/>
		/// </summary>
		public static ulong QuotesValidForMillis
		{
			get => _expMillis;
			set
			{
				if (_expMillis == 0)
					_expMillis = DEFAULT_QUOTE_EXPIRATION_MILLIS;
				else
					_expMillis = value;

				rateGraph.CacheExpirationMillis = _expMillis;
			}
		}
		static ulong _expMillis;

		static ConversionProviderFactory()
		{
			_expMillis = DEFAULT_QUOTE_EXPIRATION_MILLIS;
			providers = new List<ICurrencyConversionProvider>();
			//RegisterConversionProvider(new USDFixedRateConversionProvider());
			rateGraph = new RateGraph();
			RegisterConversionProvider(rateGraph);
		}

		public static void AddToCache(Quote q, DateTime updatedAtUtc)
		{
			rateGraph.UpdateCache(q, updatedAtUtc);
		}

		public static void RegisterConversionProvider(ICurrencyConversionProvider prov)
		{
			providers.Add(prov);
		}

		public static void UnRegisterConversionProvider(ICurrencyConversionProvider prov)
		{
			providers.Remove(prov);
		}

		public static ICurrencyConversionProvider GetConversionProvider(int i = 0)
		{
			return providers.Count > i ? providers[i] : null;
		}

		public static ICurrencyConversionProvider GetConversionProviderSkipCache()
		{
			return GetConversionProvider(1);
		}

		public static ICurrencyConversionProvider GetNext(ICurrencyConversionProvider curr)
		{
			if (curr == null) return null;
			int idxCurr = providers.IndexOf(curr);
			if (idxCurr + 1 < providers.Count)
				return providers[idxCurr + 1];
			return null;
		}

		public static ICurrencyConversionProvider GetPrevious(ICurrencyConversionProvider curr)
		{
			if (curr == null) return null;
			int idxCurr = providers.IndexOf(curr);
			if (idxCurr - 1 > 0)
				return providers[idxCurr - 1];
			return null;
		}
	}
}
