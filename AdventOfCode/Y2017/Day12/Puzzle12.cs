using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2017.Day12
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Digital Plumber";
		public override int Year => 2017;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(6).Part2(2);
			Run("input").Part1(130).Part2(189);
		}

		protected override int Part1(string[] input)
		{
			var graph = new Village(input);

			// Count the number of vertices reachable from id 0
			var program0 = graph[0];
			var groupsize = graph.NodesReachableFrom(program0).Length;

			return groupsize;
		}

		protected override int Part2(string[] input)
		{
			var graph = new Village(input);

			// Pick out vertice-clusters, group by group, adding all the group's vertices
			// to one big common seen-set until eventually there are none left unseen,
			// while counting each group.
			var n = 0;
			var seen = new HashSet<int>();
			while (true)
			{
				var unseen = graph.Nodes.FirstOrDefault(v => !seen.Contains(v.Id));
				if (unseen == null)
					break;
				foreach (var v in graph.NodesReachableFrom(unseen))
				{
					seen.Add(v.Id);
				}
				n++;
			}
			return n;
		}


		internal class Village : Graph<int>
		{
			public Village(string[] input)
			{
				foreach (var line in input)
				{
					var (id, relations) = line.RxMatch("%d <-> %*").Get<int, string>();
					foreach (var relation in relations.ToIntArray())
					{
						AddNodes(id, relation, 1);
					}
				}
			}
		}
	}
}
