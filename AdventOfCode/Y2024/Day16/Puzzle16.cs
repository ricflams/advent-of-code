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
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 16;

		public override void Run()
		{
			// Run("test1").Part1(7036).Part2(45);
			// Run("test2").Part1(11048).Part2(64);

			Run("test1").Part2(45).Part2(45);
			Run("test2").Part2(64).Part2(64);

			Run("input").Part1(147628).Part2(670);

			// 148628 too high
			// 147628

			Run("extra").Part1(83432).Part2(467);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var maze = new Maze(map)
				.WithEntry(map.AllPointsWhere(c => c == 'S').Single()); // or Point.From(1, 1);
			maze.Exit = map.AllPointsWhere(c => c == 'E').Single();



			var graph = BuildWeightedGraphFromMaze(maze);
			//graph.WriteAsGraphwiz();
		
			var pstart = Pose.From(maze.Entry, Direction.Up);
			var pexit = Pose.From(maze.Exit, Direction.Up);
			var steps = graph.ShortestPathDijkstra(pstart, pexit);		

			//steps += 1000;	

			//map.ConsoleWrite();

			return steps;
		}

		public static Graph<Pose> BuildWeightedGraphFromMaze(Maze maze)
		{
			var graph = new Graph<Pose>();

			var p0 = Pose.From(maze.Entry, Direction.Up);
			var pexit = Pose.From(maze.Exit, Direction.Up);

			var root = graph.AddNode(p0);
			if (maze.Exit != null)
			{
				graph.AddNode(pexit);
			}

			var walked = new HashSet<(Pose,Pose)>();
			
			foreach (var dir in DirectionExtensions.LookAroundDirection())
			{
				var pose = Pose.From(maze.Entry, dir);
				walked.Add((pose.Copy(), pose.Copy()));
			}
			foreach (var dir in DirectionExtensions.LookAroundDirection())
			{
				var p = Pose.From(maze.Entry + dir, dir);
				if (maze.IsWalkable(p.Point))
				{
					BuildGraph(root, p, 1);
				}
			}
			return graph;

			void BuildGraph(Graph<Pose>.Node origin, Pose pos, int weight)
			{
				if (walked.Contains((origin.Id, pos)))
				{
					return;
				}
				while (true)
				{
					walked.Add((origin.Id, pos.Copy()));

					var v = graph[pos];
					if (v != null)
					{
						graph.SetWeight(origin, v, weight);
						return;
					}

					var routes = DirectionExtensions.LookAroundDirection().Select(dir =>
					{
						if (dir == pos.Direction.TurnAround())
							return null;
						var p = Pose.From(pos.Point + dir, dir);
						if (!walked.Contains((origin.Id, p)) && maze.IsWalkable(p.Point) || graph[p] != null && graph[p] != origin)
							return p;
						return null;
					})
					.Where(x => x != null)
					.ToArray();

					switch (routes.Length)
					{
						case 0: // Dead end - no edge here
							return;
						case 1: // Only one way, so move forward
							weight++;
							if (pos.Direction != routes[0].Direction)
								weight += pos.Direction == routes[0].Direction.TurnAround() ? 2000 : 1000;
							pos = routes[0];
							break;
						default: // Forks, so place vertex here and take each road
							var fork = graph.AddNode(pos);
							// if (origin.Id == Point.From(3, 11))
							// 	;						
							// if (fork.Id == Point.From(3, 11))
							// 	;		

							if (pos.Point == Point.From(9, 7))
								;
							graph.AddNodes(origin.Id, fork.Id, weight);
							foreach (var p in routes)
							{
								var weight2 = 1;
								if (pos.Direction != p.Direction)
									weight2 += pos.Direction == p.Direction.TurnAround() ? 2000 : 1000;
								if (p.Point == Point.From(10, 7))
									;
								BuildGraph(fork, p, weight2);
							}
							return;
					}
				}
			}
		}

		
		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var maze = new Maze(map)
				.WithEntry(map.AllPointsWhere(c => c == 'S').Single()); // or Point.From(1, 1);
			maze.Exit = map.AllPointsWhere(c => c == 'E').Single();



			var graph = BuildWeightedGraphFromMaze(maze);
			//graph.WriteAsGraphwiz();
		
			var pstart = Pose.From(maze.Entry, Direction.Up);
			var pexit = Pose.From(maze.Exit, Direction.Up);

			var nstart = graph[pstart];
			var nexit = graph[pexit];

			var minsteps = graph.ShortestPathDijkstra(pstart, pexit);

			var seen = new HashSet<Point>();
			var seensteps = new Dictionary<Pose, int>();


			foreach (var dir in DirectionExtensions.LookAroundDirection())
			{
				var p = Pose.From(maze.Entry + dir, dir);
				if (maze.IsWalkable(p.Point))
				{
					Fill(p, 1);
				}
			}

			//steps += 1000;	

			//map.ConsoleWrite();

			return seen.Count + 2;

			bool Fill(Pose p0, int step)
			{
				if (p0.Point.ManhattanDistanceTo(maze.Exit) < 2)
					;
				if (p0.Point == maze.Exit)
					return true;
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
					if (maze.IsWalkable(p.Point))
					{
						var step2 = step + 1;
						if (p0.Direction != p.Direction)
							step2 += 1000;
						if (Fill(p, step2))
						{
							seen.Add(p.Point);
							ok = true;
						}
					}
				}
				return ok;
			}
		}
		
	}
}
