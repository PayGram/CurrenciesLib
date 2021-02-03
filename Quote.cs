namespace CurrenciesLib
{
	public class Quote
	{
		/// <summary>
		/// The first currency in the pair. The price of the base currency is always calculated in units of the quote currency.
		/// </summary>
		public Currencies BaseCurrency { get; set; }
		/// <summary>
		/// The second currency. The price of the base currency is always calculated in units of the quote currency.
		/// </summary>
		public Currencies QuoteCurrency { get; set; }
		/// <summary>
		/// The middle point between ask and bid
		/// </summary>
		public decimal Midpoint { get; set; }

		public override string ToString()
		{
			return $"{BaseCurrency}/{QuoteCurrency}:{Midpoint}";
		}
	}

}
