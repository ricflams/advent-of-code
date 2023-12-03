using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2018.Day08
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Memory Maneuver";
		public override int Year => 2018;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(138).Part2(66);
			Run("input").Part1(49180).Part2(20611);
		}

		protected override int Part1(string[] input)
		{
			var index = 0;
			var entries = input[0].ToIntArray();
			int Next() => entries[index++];

			// For n nodes, sum up the total of the childnodes' metadata
			// and the node's own metadata
			int ValueForNodes(int n)
			{
				var value = 0;
				for (var i = 0; i < n; i++)
				{
					var childs = Next();
					var metadata = Next();
					value += ValueForNodes(childs);
					for (var j = 0; j < metadata; j++)
					{
						value += Next();
					}
				}
				return value;
			}

			// Sum up values for the single root node
			var value = ValueForNodes(1);
			return value;
		}

		protected override int Part2(string[] input)
		{
			var index = 0;
			var entries = input[0].ToIntArray();
			int Next() => entries[index++];

			// Find the value of n nodes. The value of each node is the sum
			// of metadata if there are no childs; else it is the sum of the
			// childs indexed validly by the metadata (using offset 1, not 0)
			int[] ValueForNodes(int n) =>
				Enumerable.Range(0, n)
					.Select(_ =>
					{
						var childs = Next();
						var metadata = Next();
						var childvalues = ValueForNodes(childs);
						return Enumerable.Range(0, metadata)
							.Select(_ => Next())
							.Sum(m =>
								childs == 0 ? m :
								m > 0 && m <= childs ? childvalues[m - 1] : 0);

					})
					.ToArray();

			// The root node will just produce one single value
			var value = ValueForNodes(1).Single();
			return value;
		}
	}
}
