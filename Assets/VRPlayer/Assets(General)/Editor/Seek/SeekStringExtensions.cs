namespace dlobo.Seek
{
	// Naive algorithms for string matching. They don't use KMP or other improved algorithms, but on simple strings beat .NET implementations by a lot, speed-wise.
	public static class StringExtensions
	{
		public static bool Contains_Fast(this string str, string pattern)
		{
			return IndexOf_Fast(str, pattern) != -1;
		}

		public static int IndexOf_Fast(this string str, string pattern)
		{
			int lastIndex = str.Length - pattern.Length;

			for (int i = 0; i <= lastIndex; i++)
			{
				int j;
				for (j = 0; j < pattern.Length; j++) {
					if (str[i+j] != pattern[j]) {
						break;
					}
				}

				if (j == pattern.Length) {
					return i;
				}
			}

			return -1;
		}

		public static bool Contains_IgnoreCase_Fast(this string str, string pattern)
		{
			return IndexOf_IgnoreCase_Fast(str, pattern) != -1;
		}

		public static int IndexOf_IgnoreCase_Fast(this string str, string pattern)
		{
			const int convertToUpper = - 'a' + 'A';

			for (int i = 0; i <= str.Length - pattern.Length; i++)
			{
				int j;
				for (j = 0; j < pattern.Length; j++)
				{
					char c1 = pattern[j];
					char c2 = str[i+j];

					if (c1 == c2) {
						continue;
					}

					if (c1 >= 'a' && c1 <= 'z') {
						if (c2 == c1 + convertToUpper) {
							continue;
						}
					} else if (c2 >= 'a' && c2 <= 'z') {
						if (c1 == c2 + convertToUpper) {
							continue;
						}
					}
					break;
				}

				if (j == pattern.Length) {
					return i;
				}
			}

			return -1;
		}

		public static bool IsMatch_Exact(this string str, string pattern, bool ignoreCase)
		{
			return ignoreCase ? str.Contains_IgnoreCase_Fast(pattern) : str.Contains_Fast(pattern);
		}

		public static bool IsMatch_Greedy(this string str, string pattern, bool ignoreCase)
		{
			int j = 0;

			if (pattern.Length == 0) {
				return true;
			}

			for (int i = 0; i < str.Length; i++) {
				if (pattern[j] == str[i] || (ignoreCase && char.ToLower(pattern[j]) == char.ToLower(str[i]))) {
					j++;
					if (j == pattern.Length) {
						return true;
					}
				}
			}

			return false;
		}
	}
}
