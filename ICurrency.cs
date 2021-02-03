using System.Threading.Tasks;

namespace CurrenciesLib
{
	public interface ICurrency
	{
		/// <summary>
		/// The symbol of the currency, usually 3 or 4 characters
		/// </summary>
		string Symbol { get; }
		/// <summary>
		/// Whether a crypto or a fiat currency
		/// </summary>
		CurrencyTypes CurrencyType { get; }
		/// <summary>
		/// True if its change rate against USD is fixed
		/// </summary>
		bool IsFixedRateUSD { get; }
		/// <summary>
		/// How many units of this currency are required to buy one USD. If the currency doesn't have a fixed change rate with USD, 
		/// it will be decimal.MinValue
		/// </summary>
		decimal HowManyForOneUSD { get; }
		/// <summary>
		/// Converts the amount expressed in this currency to the destination currency
		/// </summary>
		/// <param name="destCurrency">The destination currency to convert the amount to</param>
		/// <param name="amount">The amount expressed in this currency to convert to. If amount == decimal.MinValue or decimal.MaxValue, the same will be returned</param>
		decimal ConvertTo(Currencies destCurrency, decimal amount);
		/// <summary>
		/// Asynchronously Converts the amount expressed in this currency to the destination currency
		/// </summary>
		/// <param name="destCurrency">The destination currency to convert the amount to</param>
		/// <param name="amount">The amount expressed in this currency to convert to. If amount == decimal.MinValue or decimal.MaxValue, the same will be returned</param>
		Task<decimal> ConvertToAsync(Currencies destCurrency, decimal amount);
		/// <summary>
		/// The precision need for this currency. 
		/// </summary>
		ushort Precision { get; }
		/// <summary>
		/// Gets whether this currency is a cryptocurrency
		/// </summary>
		bool IsCrypto { get; }
		/// <summary>
		/// The currency represented by this instance
		/// </summary>
		Currencies CurrencyId { get; }
		/// <summary>
		/// Returns true if this instance equals curr
		/// </summary>
		/// <param name="curr"></param>
		/// <returns></returns>
		bool Equals(ICurrency curr);
	}
}
