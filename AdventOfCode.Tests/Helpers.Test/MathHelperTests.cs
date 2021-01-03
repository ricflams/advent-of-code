using Xunit;
using AdventOfCode.Helpers;

namespace AdventOfCode.Helpers.UnitTests
{
	public class MathHelperTests
	{
		[Fact]
		public void TestAllCombinations()
		{
			foreach (var x in MathHelper.AllCombinations<int>(new [] { new int[]{1, 2, 3, 4}, new int[]{10, 11, 12}, new int[]{99}, new int[]{33,44} } ))
			{
				System.Console.WriteLine(string.Join(" ", x));
			}
		}
	}
}