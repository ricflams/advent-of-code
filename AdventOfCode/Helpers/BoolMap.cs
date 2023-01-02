using System.Collections.Generic;

namespace AdventOfCode.Helpers
{
	public class BoolMap : HashSet<int>
	{
		public void Set  (int x, int y) =>      Add(Size * (x+Offset) + y+Offset);
		public bool IsSet(int x, int y) => Contains(Size * (x+Offset) + y+Offset);

		private readonly int Size = (int)System.Math.Sqrt(int.MaxValue);
		private readonly int Offset = (int)System.Math.Sqrt(int.MaxValue) / 2;
	}
}