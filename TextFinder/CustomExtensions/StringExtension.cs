using System;

namespace TextFinder.CustomExtensions
{
	public static class StringExtension
	{
		public static bool Contains(this string containerString, string containsString, StringComparison comparison)
		{
			return containerString?.IndexOf(containsString, comparison) >= 0;
		}
	}
}