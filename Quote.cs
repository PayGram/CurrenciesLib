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
		/// <summary>
		/// True if this quote is inferred from other quotes, false if it was directly obtained from a conversion provider. Inferred quotes are less reliable than non-inferred ones, so they should be used only when no other quote is available.
		/// </summary>
		public bool IsInferred { get; set; } = true;
		/// <summary>
		/// Spread applied when user buys the QuoteCurrency (e.g., 0.02 = 2%)
		/// </summary>
		public decimal SpreadBuy { get; set; }
		/// <summary>
		/// Spread applied when user sells the QuoteCurrency (e.g., 0.02 = 2%)
		/// </summary>
		public decimal SpreadSell { get; set; }

		public override string ToString()
		{
			return $"{BaseCurrency}/{QuoteCurrency}:{Midpoint}";
		}
	}

}
