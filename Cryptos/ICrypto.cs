namespace CurrenciesLib.Cryptos
{
	public interface ICrypto : ICurrency
	{
		CryptoNetworks Network { get; }
		/// <summary>
		/// Gets the amount in the main currency corresponding to the passed satoshi.
		/// If precision is 0, satoshi itself will be returned
		/// </summary>
		/// <param name="satoshi">The value to convert to the main currency</param>
		/// <returns>the amount in the main currency corresponding to the passed satoshi.
		/// If precision is 0, satoshi itself will be returned</returns>
		decimal GetValueFromSatoshi(decimal satoshi);
		string SymbolAndNetwork { get; }
	}
}
