using Nethereum.Hex.HexConvertors.Extensions;

namespace CurrenciesLib.Cryptos
{
	public static class CryptoHelper
	{
		/// <summary>
		/// Check if a given address is valid for a certain network
		/// </summary>
		/// <param name="network"></param>
		/// <param name="address"></param>
		/// <returns>If network is Standard, returns false</returns>
		public static bool IsValidAddress(CryptoNetworks network, string address)
		{
			switch (network)
			{
				case CryptoNetworks.ERC20:
				case CryptoNetworks.BEP20:
					return IsERC20ValidAddress(address);
				case CryptoNetworks.TRC20:
					return IsTRC20ValidAddress(address);
				case CryptoNetworks.Standard:
				default:
					return false;
			}
		}

		private static bool IsTRC20ValidAddress(string address)
		{
			if (string.IsNullOrWhiteSpace(address)) return false;
			return address.StartsWith("T") && address.Length == 34;
		}

		private static bool IsERC20ValidAddress(string address)
		{
			if (string.IsNullOrWhiteSpace(address)) return false;
			return address.HasHexPrefix() && address.Length == 42 &&
				   address.IsHex();
		}
	}
}
