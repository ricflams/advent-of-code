using Xunit;
using AdventOfCode.Helpers.Arrays;

namespace AdventOfCode.Helpers.UnitTests
{
	public class ArrayTests
	{
		[Theory]
		[InlineData("abc", 0, 0, "abc")]

		[InlineData("abcd", 0, 1, "bacd")]
		[InlineData("abcd", 0, 2, "bcad")]
		[InlineData("abcd", 0, 3, "bcda")]
		[InlineData("abcd", 1, 2, "acbd")]
		[InlineData("abcd", 1, 3, "acdb")]
		[InlineData("abcd", 2, 3, "abdc")]

		[InlineData("abcd", 3, 2, "abdc")]
		[InlineData("abcd", 3, 1, "adbc")]
		[InlineData("abcd", 3, 0, "dabc")]
		[InlineData("abcd", 2, 1, "acbd")]
		[InlineData("abcd", 2, 0, "cabd")]
		[InlineData("abcd", 1, 0, "bacd")]
		public void TestRelocate(string a, int from, int to, string expected)
		{
			var actual = new string(a.ToCharArray().Move(from, to));
			Assert.Equal(expected, actual);
		}
	}
}