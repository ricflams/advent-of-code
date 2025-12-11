using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Net;

namespace AdventOfCode.Y2024.Day16
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Reindeer Maze";
		public override int Year => 2024;
		public override int Day => 16;

		public override void Run()
		{
			// Run("test1").Part1(7036).Part2(45);
			// Run("test2").Part1(11048).Part2(64);

			Run("test1").Part1(7036).Part2(45);
			Run("test2").Part1(11048).Part2(64);

			Run("input").Part1(147628).Part2(670);

			// 148628 too high
			// 147628

			Run("extra").Part1(83432).Part2(467);
		}


		private static int MoveCost(Pose p1, Pose p2) => 1 + (p1.Direction == p2.Direction ? 0 : p1.Direction == p2.Direction.TurnAround() ? 2000 : 1000);


		private class ReindeerMaze : Graph<Pose>
		{
			private readonly Pose _start;
			private readonly Pose _end;
			public readonly Maze Maze;

			public ReindeerMaze(CharMap map)
			{
				Maze = new Maze(map)
					.WithEntry(map.AllPointsWhere(c => c == 'S').Single())
					.WithExit(map.AllPointsWhere(c => c == 'E').Single());

				_start = Pose.From(Maze.Entry, Direction.Right);
				_end = Pose.From(Maze.Exit, Direction.Up);

				Build(Maze);
			}

			public int ShortestPath()
			{
				return ShortestPathDijkstra(this[_start], this[_end]);
			}

			private void Build(Maze maze)
			{
				var root = AddNode(_start);

				// foreach (var d in Directions.All.Where(d => d != _start.Direction))
				// {
				// 	AddNodes(_start, Pose.From(maze.Entry, d), d == _start.Direction.TurnAround() ? 2000 : 1000);
				// }

				AddNode(_end);

				// not necessary?!
				// foreach (var d in Directions.All.Where(d => d != _end.Direction))
				// {
				// 	AddNodes(_end, Pose.From(maze.Exit, d), 0);
				// }

				var walked = new HashSet<(Pose, Pose)>();
				//walked.Add((_start, _start));

				// foreach (var dir in DirectionExtensions.LookAroundDirection())
				// {
				// 	var pose = Pose.From(maze.Entry, dir);
				// 	walked.Add((pose.Copy(), pose.Copy()));
				// }
				// BuildGraph(root, _start, 0);
				foreach (var dir in Directions.All)
				{
					var p = Pose.From(maze.Entry + dir, dir);
					if (maze.IsWalkable(p.Point))
					{
						BuildGraph(root, p, MoveCost(_start, p));
					}
				}

				void BuildGraph(Node origin, Pose pos, int weight)
				{
					// if (walked.Contains((origin.Id, pos)))
					// {
					// 	return;
					// }
					while (true)
					{
						walked.Add((origin.Id, pos));

						var v = this[pos];
						if (v != null)
						{
							SetWeight(origin, v, weight);
							return;
						}

						var routes = Directions
							.AllExcept(pos.Direction.TurnAround())
							.Select(dir => Pose.From(pos.Point + dir, dir))
							.Where(p => !walked.Contains((origin.Id, p)) && maze.IsWalkable(p.Point) || this[p] != null && this[p] != origin)
							.ToArray();

						switch (routes.Length)
						{
							case 0: // Dead end - no edge here
								return;
							case 1: // Only one way, so move forward
								weight += MoveCost(pos, routes[0]);
								pos = routes[0];
								break;
							default: // Forks, so place vertex here and take each road
								var fork = AddNode(pos);
								AddNodes(origin.Id, fork.Id, weight);
								foreach (var p in routes)
								{
									BuildGraph(fork, p, MoveCost(pos, p));
								}
								return;
						}
					}
				}
			}




		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var graph = new ReindeerMaze(map);
			var steps = graph.ShortestPath();

			return steps;
		}




		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var graph = new ReindeerMaze(map);
			var maze = graph.Maze;

			//var steps = graph.ShortestPath();

			// var maze = new Maze(map)
			// 	.WithEntry(map.AllPointsWhere(c => c == 'S').Single()); // or Point.From(1, 1);
			// maze.Exit = map.AllPointsWhere(c => c == 'E').Single();

			// TODO: improve performance

			// var graph = BuildWeightedGraphFromMaze(maze);
			// //graph.WriteAsGraphwiz();

			var pstart = Pose.From(maze.Entry, Direction.Right);
			var pexit = Pose.From(maze.Exit, Direction.Up);

			var nstart = graph[pstart];
			var nexit = graph[pexit];

			var minsteps = graph.ShortestPath();

			var seen = new HashSet<Point>();
			var seensteps = new Dictionary<Pose, int>();
			seen.Add(pstart.Point);


			foreach (var dir in Directions.All)
			{
				var p = Pose.From(maze.Entry + dir, dir);
				if (maze.IsWalkable(p.Point))
				{
					Fill(p, MoveCost(pstart, p));
				}
			}

			var hasEnd = seen.Contains(pexit.Point);
			//map.ConsoleWrite();

			//map.ConsoleWrite((p, c) => seen.Contains(p) ? 'O' : c);

			return seen.Count;



			bool Fill(Pose p0, int step)
			{
				if (p0.Point == maze.Exit)
				{
					//seen.Add(p0.Point);
					return true;
				}
				if (step + p0.Point.ManhattanDistanceTo(maze.Exit) > minsteps)
					return false;

				if (seensteps.TryGetValue(p0, out var n) && n < step)
					return false;
				seensteps[p0.Copy()] = step;

				var ok = false;
				foreach (var dir in DirectionExtensions.LookAroundDirection())
				{
					if (p0.Direction == dir.TurnAround())
						continue;
					var p = Pose.From(p0.Point + dir, dir);
					if (seensteps.TryGetValue(p0, out var n2) && n2 < step)
						continue;
					if (maze.IsWalkable(p.Point))
					{
						if (Fill(p, step + MoveCost(p0, p)))
						{
							seen.Add(p0.Point);
							seen.Add(p.Point);
							ok = true;
						}
					}
				}
				// if (ok)
				// 	seen.Add(p0.Point);
				return ok;
			}
		}

	}
}
