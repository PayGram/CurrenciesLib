using System;

namespace CurrenciesLib.ConversionProviders
{
	public class TimedQuote : Quote
	{
		public DateTime UpdatedAtUTC { get; set; }

		public override string ToString()
		{
			return $"{base.ToString()}@{UpdatedAtUTC:dd/MM H:m:ss}";
		}
	}
}
