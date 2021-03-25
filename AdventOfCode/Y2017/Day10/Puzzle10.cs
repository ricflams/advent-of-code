using System.Linq;
using AdventOfCode.Helpers.Byte;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2017.Day10
{
	internal class Puzzle : Puzzle<int, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Knot Hash";
		public override int Year => 2017;
		public override int Day => 10;

		public void Run()
		{
			Run("input").Part1(1980).Part2("899124dac21012ebc32e2f4d11eaec55");
		}

		protected override int Part1(string[] input)
		{
			var lengths = input[0].ToIntArray();
			var N = 256;

			var pos = 0;
			var skip = 0;

			var list = Enumerable.Range(0, N).ToArray();
			foreach (var len in lengths)
			{
				// Reverse len
				for (var i = 0; i < len/2; i++)
				{
					var a = (pos+i) % N;
					var b = (pos+len-1-i) % N;
					(list[a], list[b]) = (list[b], list[a]);
				}
				pos += len + skip++;
			}

			var result = list[0] * list[1];
			return result;
		}

		protected override string Part2(string[] input)
		{
			var message = input[0];
			var hash = Common.KnotHash.Hash(message);
			return hash.FormatAsHex();
		}
	}
}
