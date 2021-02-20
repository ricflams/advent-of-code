using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2017.Day12
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Digital Plumber";
		public override int Year => 2017;
		public override int Day => 12;

		public void Run()
		{
			RunFor("test1", 6, 2);
			RunFor("input", 130, 189);
		}

		protected override int Part1(string[] input)
		{
			var graph = new Graph(input);

			// Count the number of vertices reachable from id 0
			var program0 = graph.Vertices[0];
			var groupsize = graph.VerticesReachableFrom(program0).Length;

			return groupsize;
		}

		protected override int Part2(string[] input)
		{
			var graph = new Graph(input);

			// Pick out vertice-clusters, group by group, adding all the group's vertices
			// to one big common seen-set until eventually there are none left unseen,
			// while counting each group.
			var n = 0;
			var seen = new HashSet<int>();
			while (true)
			{
				var unseen = graph.Vertices.Values.FirstOrDefault(v => !seen.Contains(v.Value));
				if (unseen == null)
					break;
				foreach (var v in graph.VerticesReachableFrom(unseen))
				{
					seen.Add(v.Value);
				}
				n++;
			}
			return n;
		}

		internal class Graph : BaseUnitGraph<int>
		{
			public Graph(string[] input)
			{
				foreach (var line in input)
				{
					var (id, relations) = line.RxMatch("%d <-> %*").Get<int, string>();
					foreach (var relation in relations.ToIntArray())
					{
						AddEdge(id, relation);
					}
				}
			}			
		}
	}
}
