using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day07
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Bridge Repair";
		public override int Year => 2024;
		public override int Day => 7;

		public override void Run()
		{
			Run("test1").Part1(3749).Part2(11387);
			Run("input").Part1(12940396350192).Part2(106016735664498);
			Run("extra").Part1(14711933466277).Part2(286580387663654);
		}

		protected override long Part1(string[] input)
		{
			var tests = input.Select(Test.Parse);

			var sum = tests.Where(t => t.IsValid()).Sum(t => t.Sum);

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var tests = input.Select(Test.Parse);

			var sum = tests.Where(t => t.IsValidWithConcat()).Sum(t => t.Sum);

			return sum;
		}

		internal class Test
		{
			public long Sum { get; init; }
			public long[] Values { get; init; }

			public static Test Parse(string s)
			{
				var part = s.Split(':');
				return new Test
				{
					Sum = long.Parse(part[0]),
					Values = part[1].SplitSpace().Select(long.Parse).ToArray()
				};
			}

			public bool IsValid()
			{
				return IsValid(Values[0], 1);
				bool IsValid(long v, int pos)
				{
					if (v > Sum) return false;
					if (pos == Values.Length) return v == Sum;
					return IsValid(v*Values[pos], pos+1) || IsValid(v+Values[pos], pos+1);
				}
			}

			public bool IsValidWithConcat()
			{
				return IsValid(Values[0], 1);
				bool IsValid(long v, int pos)
				{
					if (v > Sum) return false;
					if (pos == Values.Length) return v == Sum;
					return IsValid(v * Values[pos], pos + 1)
						|| IsValid(v + Values[pos], pos + 1)
						|| IsValid(v.Concat(Values[pos]), pos + 1);
				}
			}
		}
	}
}
