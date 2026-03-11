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

		public override string ToString()
		{
			return $"{BaseCurrency}/{QuoteCurrency}:{Midpoint}";
		}
	}

}
