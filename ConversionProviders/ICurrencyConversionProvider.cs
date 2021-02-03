using System.Threading.Tasks;

namespace CurrenciesLib.ConversionProviders
{
	public interface ICurrencyConversionProvider
	{
		/// <summary>
		/// Converts an amount from the source currency to the destination currency
		/// </summary>
		/// <param name="source">The currency to convert the amount from</param>
		/// <param name="dest">The currency to convert the amount to</param>
		/// <param name="amount">The amount to convert. If amount == decimal.MinValue or decimal.MaxValue, the same will be returned</param>
		/// <returns>The converted amount expressed in dest currency</returns>
		decimal Convert(Currencies source, Currencies dest, decimal amount, ConversionBag convStatus);

		/// <summary>
		/// Asynchronously Converts an amount from the source currency to the destination currency
		/// </summary>
		/// <param name="source">The currency to convert the amount from</param>
		/// <param name="dest">The currency to convert the amount to</param>
		/// <param name="amount">The amount to convert. If amount == decimal.MinValue or decimal.MaxValue, the same will be returned</param>
		/// <returns>The converted amount expressed in dest currency</returns>
		Task<decimal> ConvertAsync(Currencies source, Currencies dest, decimal amount, ConversionBag convStatus);

		/// <summary>
		/// When implemented in a derived class it gets whether the currency conversion provider
		/// is able to convert from source currency to dest currency. 
		/// </summary>
		/// <param name="source">The currency to convert from</param>
		/// <param name="dest">The currency to convert to</param>
		/// <returns>False if it is impossible for this provider, now and ever,
		/// to do the conversion between the two currencies. True if the conversion is possible, even though
		/// it is not guaranteed; this is the case when the conversion happens taking the rates from an internet service
		/// provider that, for reasons unknown at the time of calling the method, will be offline when we try to 
		/// do the conversion. In this case CanConvert will return true (theorically possible converting), but Convert
		/// will eventually return decimal.MinValue</returns>
		bool CanConvert(Currencies source, Currencies dest, ConversionBag convStatus);

		/// <summary>
		/// When implemented in derived class gets a quote for the given pair or null if not found, 
		/// conditions of convertibility have already been checked
		/// The returned Quote will have BaseCurrency=source and QuoteCurrency=dest
		/// </summary>
		/// <param name="source">The currency to convert from</param>
		/// <param name="dest">The currency to convert to</param>
		Task<TimedQuote> GetQuoteAsync(Currencies source, Currencies dest, ConversionBag convStatus);

		/// <summary>
		/// When implemented in derived class, asynchronously gets a quote for the given pair or null if not found, 
		/// conditions of convertibility have already been checked.
		/// The returned Quote will have BaseCurrency=source and QuoteCurrency=dest
		/// </summary>
		/// <param name="source">The currency to convert from</param>
		/// <param name="dest">The currency to convert to</param>
		TimedQuote GetQuote(Currencies source, Currencies dest, ConversionBag convStatus);
	}
}
