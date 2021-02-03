using System.Collections.Generic;

namespace CurrenciesLib.ConversionProviders
{
	/// <summary>
	/// Used during the conversion process to allow providers to shareinformation
	/// </summary>
	public class ConversionBag
	{
		public Dictionary<string, object> Bag { get; set; }

		public ConversionBag()
		{
			Bag = new Dictionary<string, object>();
		}
	}
}
