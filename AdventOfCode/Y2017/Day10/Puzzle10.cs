using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.Strings;

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
			RunPart1For("test1", 12);
			RunFor("input", 1980, "899124dac21012ebc32e2f4d11eaec55");
		}

		protected override int Part1(string[] input)
		{
			var N = input[0].RxMatch("size:%d").Get<int>();
			var lengths = input[1].AsIntArray();

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
			var N = input[0].RxMatch("size:%d").Get<int>();
			var lengths = input[1]
				.Select(c => (int)c)
				.Concat(new [] { 17, 31, 73, 47, 23 })
				.Select(x => (byte)x)
				.ToArray();

			var pos = 0;
			var skip = 0;
			var list = Enumerable.Range(0, N).Select(x => (byte)x).ToArray();
			for (var round = 0; round < 64; round++)
			{
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
			}

			var densehash = new byte[16];
			for (var i = 0; i < 16; i++)
			{
				var block = i*16;
				densehash[i] = list[block];
				for (var j = 1; j < 16; j++)
				{
					densehash[i] ^= list[block+j];
				}
			}

			var hash = MathHelper.FormatAsHex(densehash);

			return hash;
		}
	}
}
