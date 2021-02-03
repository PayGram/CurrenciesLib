using System;
using System.Threading.Tasks;

namespace CurrenciesLib.ConversionProviders
{
	/// <summary>
	/// To convert source to dest, it will convert both source and dest to a default currency
	/// and then it will calculate the rate of each other inferring from the base currency
	/// </summary>
	public class ToDefaultCurrencyConversionProvider : ConversionProvider
	{
		/// <summary>
		/// The default currency that both base and quote currency will be converted to before 
		/// calculating the new quote
		/// </summary>
		public Currencies DefaultCurrency { get; set; }

		/// <param name="defaultCurrency">The default currency that both base and quote currency will be converted to before 
		/// calculating the new quote</param>
		public ToDefaultCurrencyConversionProvider(Currencies defaultCurrency)
		{
			if (Currency.GetBySymbol(defaultCurrency) == null)
				throw new NullReferenceException($"defaultCurrency {defaultCurrency} cannot be null");
			this.DefaultCurrency = defaultCurrency;
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
			// conditions of convertibility have been done already, but convStatus can be null
			if (convStatus == null)
				convStatus = new ConversionBag();

			string idKey = $"{source}{dest}{DefaultCurrency}{this.GetHashCode()}";
			convStatus.Bag.Add(idKey, "yes");

			var qs = ConversionProviderFactory.GetConversionProvider().GetQuote(source, DefaultCurrency, convStatus);
			if (qs == null) return null;
			var qd = ConversionProviderFactory.GetConversionProvider().GetQuote(dest, DefaultCurrency, convStatus);
			if (qd == null) return null;

			return new TimedQuote()
			{
				BaseCurrency = source,
				QuoteCurrency = dest,
				Midpoint = qs.Midpoint / qd.Midpoint,
				UpdatedAtUTC = qs.UpdatedAtUTC < qd.UpdatedAtUTC ? qs.UpdatedAtUTC : qd.UpdatedAtUTC
			};
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <param name="amount"><inheritdoc/></param>
		/// <returns><inheritdoc/></returns>
		protected override async Task<TimedQuote> getQuoteAsync(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			// conditions of convertibility have been done already, but convStatus can be null
			if (convStatus == null)
				convStatus = new ConversionBag();

			string idKey = $"{source}{dest}{DefaultCurrency}{this.GetHashCode()}";
			convStatus.Bag.Add(idKey, "yes");

			var qs = await ConversionProviderFactory.GetConversionProvider().GetQuoteAsync(source, DefaultCurrency, convStatus);
			if (qs == null) return null;
			var qd = await ConversionProviderFactory.GetConversionProvider().GetQuoteAsync(dest, DefaultCurrency, convStatus);
			if (qd == null) return null;

			return new TimedQuote()
			{
				BaseCurrency = source,
				QuoteCurrency = dest,
				Midpoint = qs.Midpoint / qd.Midpoint,
				UpdatedAtUTC = qs.UpdatedAtUTC < qd.UpdatedAtUTC ? qs.UpdatedAtUTC : qd.UpdatedAtUTC
			};
		}

		/// <summary>
		/// Will be able to convert only if both currencies are different from the BaseCurrency
		/// <inheritdoc/>
		/// </summary>
		/// <param name="source"><inheritdoc/></param>
		/// <param name="dest"><inheritdoc/></param>
		/// <returns></returns>
		public override bool CanConvert(Currencies source, Currencies dest, ConversionBag convStatus)
		{
			if (base.CanConvert(source, dest, convStatus) == false) return false;

			string idKey = $"{source}{dest}{DefaultCurrency}{this.GetHashCode()}";

			if (convStatus?.Bag.ContainsKey(idKey) ?? false)
				return false;

			return source != DefaultCurrency && dest != DefaultCurrency;
		}
	}
}
