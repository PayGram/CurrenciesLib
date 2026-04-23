using Microsoft.VisualBasic;
using System;

namespace CurrenciesLib.ConversionProviders
{
	public class TimedQuote : Quote
	{
		// if an exchange rate pair has never been updated externally, we consider it fixed and we won't update it. this is useful for pairs like PHP/PHPT that are managed internally with spreads and should not be updated externally
		public readonly static DateTime IsFixedIfUpdatedAt = new DateTime(2200, 1, 1);
		public DateTime UpdatedAtUTC { get; set; }
		public bool IsFixed => IsFixedIfUpdatedAt == UpdatedAtUTC;
		public override string ToString()
		{
			return $"{base.ToString()}@{UpdatedAtUTC:dd/MM H:m:ss}";
		}
	}
}
