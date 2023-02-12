using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day12
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Passage Pathing";
		public override int Year => 2021;
		public override int Day => 12;

		public void Run()
		{
			Run("test1").Part1(10).Part2(36);

			Run("test2").Part1(19).Part2(103);
			Run("test3").Part1(226).Part2(3509);

			Run("test9").Part1(4304).Part2(118242);

			Run("input").Part1(3713).Part2(91292);

			// todo: clean
		}

		protected override long Part1(string[] input)
		{
			var graph = new Graph(input);

			var start = graph.Vertices.Single(x => x.Key == "start").Value;
			var end = graph.Vertices.Single(x => x.Key == "end").Value;

			var seenpaths = new HashSet<string>();
			var seensmallcaves = new HashSet<string>();

			VisitFrom(start, seensmallcaves, start.Value);

			void VisitFrom(Graph.Vertex v, HashSet<string> seensmallcaves, string path)
			{
				if (v == end)
					return;
				foreach (var e in v.Edges)
				{
					if (e == start)
						continue;
					var name = e.Value;
					var seensmallcaves2 = new HashSet<string>(seensmallcaves);
					if (name.All(char.IsLower))
					{
						if (seensmallcaves2.Contains(name))
							continue;
						seensmallcaves2.Add(name);
					}
					var path2 = $"{path},{name}";
					if (seenpaths.Contains(path2))
						continue;
					seenpaths.Add(path2);
					//Console.WriteLine(path2);
					VisitFrom(e, seensmallcaves2, path2);
				}
			}


			return seenpaths.Count(x => x.EndsWith("end"));
		}

		protected override long Part2(string[] input)
		{
			var graph = new Graph(input);

			var start = graph.Vertices.Single(x => x.Key == "start").Value;
			var end = graph.Vertices.Single(x => x.Key == "end").Value;

			var seenpaths = new HashSet<string>();
			var seensmallcaves = new Dictionary<string, int>();

			VisitFrom(start, seensmallcaves, start.Value);
			var paths = seenpaths.Where(x => x.EndsWith(",end")).ToArray();

			return paths.Length;

			void VisitFrom(Graph.Vertex v, Dictionary<string, int> seensmallcaves, string path)
			{
				if (v == end)
					return;
				foreach (var e in v.Edges)
				{
					if (e == start)
						continue;
					var name = e.Value;
					var seensmallcaves2 = new Dictionary<string, int>(seensmallcaves);
					if (name.All(char.IsLower))
					{
						var seentwice = seensmallcaves2.Values.Any(x => x > 1);
						if (!seensmallcaves2.TryGetValue(name, out var cv))
						{
							cv = seensmallcaves2[name] = 0;
						}
						else if (seentwice)
						{
							continue;
						}

						seensmallcaves2[name] = seensmallcaves2[name]+1;
					}
					var path2 = $"{path},{name}";
					if (seenpaths.Contains(path2))
						continue;
					seenpaths.Add(path2);
					//Console.WriteLine(path2);
					VisitFrom(e, seensmallcaves2, path2);
				}
			}
		}


		internal class Graph : BaseUnitGraph<string>
		{
			public Graph(string[] input)
			{
				foreach (var line in input)
				{
					var (from, to) = line.RxMatch("%s-%s").Get<string, string>();
					AddEdge(from, to);
				}
			}
		}
	}
}
