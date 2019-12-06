using System.Collections.Generic;

namespace AdventOfCode2019.Intcode
{
	internal class Data
	{
		public List<int> Input { get; } = new List<int>();
		public List<int> Output { get; } = new List<int>();

		public string FormattedOutput => string.Join(" ", Output);
	}
}
