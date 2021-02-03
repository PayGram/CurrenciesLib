using System;
using System.Threading.Tasks;

namespace CurrenciesLib.ConversionProviders
{
	public abstract class ConversionProvider : ICurrencyConversionProvider
	{
		/// <summary>
		/// if set to true, when the conversion cannot be done, it will try to pass the conversion to the next ConversionProvider
		/// otherwise it will return decimal.MinValue if the conversion was not possible
		/// </summary>
		public bool TryNext { get; set; }

		public ConversionProvider(bool tryNext = true)
		{
			TryNext = tryNext;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"> <inheritdoc/></param>
		/// <param name="dest"> <inheritdoc/></param>
		/// <returns> <inheritdoc/></returns>
		public virtual bool CanConvert(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			return Currency.GetBySymbol(source) != null && Currency.GetBySymbol(dest) != null;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <param name="amount"><inheritdoc/></param>
		/// <returns> <inheritdoc/></returns>
		public decimal Convert(Currencies source, Currencies dest, decimal amount, ConversionBag convStatus)
		{
			if (source.Equals(dest)) return amount;
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;

			var quote = GetQuote(source, dest, convStatus);

			if (quote == null)
				return decimal.MinValue;

			return quote.BaseCurrency.Equals(source) ? quote.Midpoint * amount : amount / quote.Midpoint;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <param name="amount"><inheritdoc/></param>
		/// <returns> <inheritdoc/></returns>
		public async Task<decimal> ConvertAsync(Currencies source, Currencies dest, decimal amount, ConversionBag convStatus)
		{
			if (source.Equals(dest)) return amount;
			if (amount == decimal.Zero || amount == decimal.MinValue || amount == decimal.MaxValue) return amount;

			var quote = await GetQuoteAsync(source, dest, convStatus);

			if (quote == null)
				return decimal.MinValue;

			return quote.BaseCurrency.Equals(source) ? quote.Midpoint * amount : amount / quote.Midpoint;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <param name="convStatus"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		public virtual async Task<TimedQuote> GetQuoteAsync(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			if (source == dest)
				return new TimedQuote() { BaseCurrency = source, Midpoint = 1, QuoteCurrency = dest, UpdatedAtUTC = DateTime.UtcNow.AddYears(1) };

			if (CanConvert(source, dest, convStatus) == true)
			{
				TimedQuote quote = await getQuoteAsync(source, dest, convStatus);
				if (quote != null && quote.UpdatedAtUTC.AddMilliseconds(ConversionProviderFactory.QuotesValidForMillis) >= DateTime.UtcNow)
				{
					if (this is CacheConversionProvider == false) // if the provider who got us the quote is not the cache provider
						ConversionProviderFactory.AddToCache(quote, quote.UpdatedAtUTC); // add the quote to the cache
					return quote;
				}
			}

			if (TryNext == false)
				return null;

			var nextProv = ConversionProviderFactory.GetNext(this);

			if (nextProv == null)
				return null;

			return await nextProv.GetQuoteAsync(source, dest, convStatus);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <param name="convStatus"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		public virtual TimedQuote GetQuote(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			if (source == dest)
				return new TimedQuote() { BaseCurrency = source, Midpoint = 1, QuoteCurrency = dest, UpdatedAtUTC = DateTime.UtcNow.AddYears(1) };

			if (CanConvert(source, dest, convStatus) == true)
			{
				TimedQuote quote = getQuote(source, dest, convStatus);
				if (quote != null && quote.UpdatedAtUTC.AddMilliseconds(ConversionProviderFactory.QuotesValidForMillis) >= DateTime.UtcNow)
				{
					if (this is CacheConversionProvider == false) // if the provider who got us the quote is not the cache provider
						ConversionProviderFactory.AddToCache(quote, quote.UpdatedAtUTC); // add the quote to the cache
					return quote;
				}
			}

			if (TryNext == false)
				return null;

			return ConversionProviderFactory.GetNext(this)?.GetQuote(source, dest, convStatus);
		}

		/// <summary>
		/// When implemented in derived class gets a quote for the given pair or null if not found, 
		/// conditions of convertibility have already been checked
		/// The returned Quote will have BaseCurrency=source and QuoteCurrency=dest
		/// </summary>
		/// <param name="source">The currency to convert from</param>
		/// <param name="dest">The currency to convert to</param>
		protected abstract TimedQuote getQuote(Currencies source, Currencies dest, ConversionBag convStatus);

		/// <summary>
		/// When implemented in derived class, asynchronously gets a quote for the given pair or null if not found, 
		/// conditions of convertibility have already been checked
		/// The returned Quote will have BaseCurrency=source and QuoteCurrency=dest
		/// </summary>
		/// <param name="source">The currency to convert from</param>
		/// <param name="dest">The currency to convert to</param>
		protected virtual async Task<TimedQuote> getQuoteAsync(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			return getQuote(source, dest, convStatus);
		}
	}
}
