using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day18
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 18";
		public override int Year => 2022;
		public override int Day => 18;

		public void Run()
		{
			//Run("test1").Part1(10);
		//	Run("test2").Part1(64).Part2(58);
			Run("input").Part1(3576).Part2(0);
			// 3402 not right
			// 3358 too high
			// 3307 too high
		}

		protected override long Part1(string[] input)
		{
			var cubes = input.Select(s => s.ToIntArray()).Select(c => new Point3D(c[0], c[1], c[2])).ToArray();

			var sides = cubes.Length * 6;
			for (var i = 0; i < cubes.Length; i++)
			{
				for (var j = i+1; j < cubes.Length; j++)
				{
					var (c1, c2) = (cubes[i], cubes[j]);
					var dx = Math.Abs(c1.X - c2.X);
					var dy = Math.Abs(c1.Y - c2.Y);
					var dz = Math.Abs(c1.Z - c2.Z);
					if (dx+dy+dz == 1)
					{
						//Console.WriteLine($"{c1} {c2}", c1, c2);
						sides -= 2;
					}
				}
			}

			return sides;
		}

		protected override long Part2(string[] input)
		{
			var cubes = input.Select(s => s.ToIntArray()).Select(c => new Point3D(c[0], c[1], c[2])).ToArray();

			var sides = cubes.Length * 6;
			for (var i = 0; i < cubes.Length; i++)
			{
				for (var j = i+1; j < cubes.Length; j++)
				{
					var (c1, c2) = (cubes[i], cubes[j]);
					var dx = Math.Abs(c1.X - c2.X);
					var dy = Math.Abs(c1.Y - c2.Y);
					var dz = Math.Abs(c1.Z - c2.Z);
					if (dx+dy+dz == 1)
					{
						//Console.WriteLine($"{c1} {c2}", c1, c2);
						sides -= 2;
					}
				}
			}

			// find all embedded holes
			var displacements = new[] { new Point3D(-1,0,0), new Point3D(1,0,0), new Point3D(0,-1,0), new Point3D(0,1,1), new Point3D(0,0,-1), new Point3D(0,0,1) };
			//var holes = new HashSet<long>();
			var singlehole = new HashSet<Point3D>();
			var ends = new HashSet<Point3D>();
			var edge = new HashSet<Point3D>();
			var allcubes = new HashSet<Point3D>(cubes);
			foreach (var c in cubes)
			{
				foreach (var d in displacements)
				{
					var space = c.Add(d);
					// var neighbours = cubes.Count(x =>
					// {
					// 	var dx = Math.Abs(space.X - x.X);
					// 	var dy = Math.Abs(space.Y - x.Y);
					// 	var dz = Math.Abs(space.Z - x.Z);
					// 	return dx+dy+dz == 1;
					// });
					if (allcubes.Contains(space))
						continue;

					var neighbourCubes = cubes.Where(x =>
					{
						var dx = Math.Abs(space.X - x.X);
						var dy = Math.Abs(space.Y - x.Y);
						var dz = Math.Abs(space.Z - x.Z);
						return dx+dy+dz == 1;
					}).ToArray();
					var neighbours = neighbourCubes.Count();

					if (neighbours == 6)
					{
						//var key = ((long)space.X << 16) + ((long)space.Y << 8) + space.Z;
						//holes.Add(key);
						singlehole.Add(space);
					}
					else if (neighbours == 5)
					{
						ends.Add(space);
					}
					if (neighbours > 1)
						edge.Add(space);
				}
			}

			var externals = sides - singlehole.Count() * 6;

			var spaces = singlehole.ToArray();
			var spaces2 = singlehole.ToArray().OrderBy(x => x.X).ThenBy(x => x.Y).ThenBy(x => x.Z);
			var ends2 = ends.ToArray().OrderBy(x => x.X).ThenBy(x => x.Y).ThenBy(x => x.Z);
			// foreach (var s in spaces2)
			// 	Console.WriteLine($"space {s}");
			// foreach (var s in ends2)
			// 	Console.WriteLine($"end {s}");

			var min = new Point3D(100, 100, 100);
			var max = new Point3D(-100, -100, -100);
			Console.WriteLine();
			Console.WriteLine("x,y,z");
			foreach (var c in cubes)
			{
				//Console.WriteLine($"{c.X},{c.Y},{c.Z}");
				if (c.X < min.X)
					min = min with { X = c.X };
				if (c.Y < min.Y)
					min = min with { Y = c.Y };
				if (c.Z < min.Z)
					min = min with { Z = c.Z };
				if (c.X > max.X)
					max = max with { X = c.X };
				if (c.Y > max.Y)
					max = max with { Y = c.Y };
				if (c.Z > max.Z)
					max = max with { Z = c.Z };
			}
			Console.WriteLine();

			var world = new HashSet<Point3D>();
			for (var x = min.X-2; x <= max.X+1; x++)
			{
				for (var y = min.Y-2; y <= max.Y+1; y++)
				{
					for (var z = min.Z-2; z <= max.Z+1; z++)
					{
						world.Add(new Point3D(x, y, z));
					}
				}
			}

			Console.WriteLine("all cubes");
			foreach (var c in allcubes.OrderBy(x => x.X).ThenBy(x => x.Y).ThenBy(x => x.Z))
				Console.WriteLine(c);
			Console.WriteLine();

			var outside = min.Add(new Point3D(-1, -1, -1));
			var rim = new HashSet<Point3D>();
			FindRim(outside);
			void FindRim(Point3D p)
			{
				if (p == new Point3D(4, 3, 11))
					;
				rim.Add(p);
				var nexts = displacements
					.Select(d => p.Add(d))
					.Where(c =>
					{
						var wc = world.Contains(c);
						var rc = rim.Contains(c);
						var ac = allcubes.Contains(c);
						return wc && !rc &&  !ac;
					})
					.ToArray();
					// .Where(c => world.Contains(c))
					// .Where(c => !rim.Contains(c))
					// .Where(c => !allcubes.Contains(c));
				foreach (var next in nexts)
					FindRim(next);
			}			

			var world1 = world.Count();
			Console.WriteLine(world.Count());
			RemoveEmptySpace(outside);
			Console.WriteLine(world.Count());
			Console.WriteLine(world1 - world.Count());
			Console.WriteLine(rim.Count());
			foreach (var c in allcubes)
				world.Remove(c);
			foreach (var c in singlehole)
				world.Remove(c);
			Console.WriteLine(world.Count());
			foreach (var c in world.ToArray())
			{
				var empties = displacements.Select(d => c.Add(d)).Count(c => world.Contains(c));
				if (empties == 0)
				{
					// how?!
					Console.WriteLine($"why empty at {c}");
				}
			}

			// https://chart-studio.plotly.com/create/

			Console.WriteLine();
			foreach (var s in world)
			{
				Console.WriteLine($"{s.X},{s.Y},{s.Z}");
			}			

			void RemoveEmptySpace(Point3D p)
			{
				if (p == new Point3D(4, 3, 11))
					;
				world.Remove(p);
				var nexts = displacements
					.Select(d => p.Add(d))
					.Where(c => world.Contains(c))
					.Where(c => !allcubes.Contains(c))
					.ToArray();
				foreach (var next in nexts)
					RemoveEmptySpace(next);
			}


			// var allspaces = new HashSet<Point3D>();
			// for (var x = min.X+1; x < max.X; x++)
			// {
			// 	for (var y = min.Y+1; y < max.Y; y++)
			// 	{
			// 		for (var z = min.Z+1; z < max.Z; z++)
			// 		{
			// 			var p = new Point3D(x, y, z);
			// 			if (cubes.Contains(p))
			// 				continue;

			// 			// var touches = displacements.Select(d => p.Add(d)).Any(c => allcubes.Contains(c));
			// 			// if (!touches)
			// 			// 	continue;
			// 			var touchesSpace = displacements.Select(d => p.Add(d)).Any(c => holes.Contains(c));
			// 			if (!touchesSpace)
			// 				continue;

			// 			allspaces.Add(p);
			// 		}
			// 	}
			// }

			// foreach (var s in allspaces)
			// {
			// 	Console.WriteLine($"{s.X},{s.Y},{s.Z}");
			// }

			// foreach (var h in holes)
			// {
			// 	var paths = displacements.Select(d => h.Add(d)).Where(c => !allcubes.Contains(c)).ToArray();
			// 	if (paths.Length > 0)
			// 	{
			// 		Console.WriteLine($"{h.X},{h.Y},{h.Z}");
			// 		foreach (var p in paths)
			// 			Console.WriteLine($"{p.X},{p.Y},{p.Z}");
			// 	}
			// }			
			
			// for (var i = 0; i < spaces.Length; i++)
			// {
			// 	for (var j = i+1; j < spaces.Length; j++)
			// 	{
			// 		var (c1, c2) = (spaces[i], spaces[j]);
			// 		var dx = Math.Abs(c1.X - c2.X);
			// 		var dy = Math.Abs(c1.Y - c2.Y);
			// 		var dz = Math.Abs(c1.Z - c2.Z);
			// 		if (dx+dy+dz == 1)
			// 		{
			// 			//Console.WriteLine($"{c1} {c2}", c1, c2);
			// 			externals += 2;
			// 		}
			// 	}
			// }

			var internalsides = 0;
			// foreach (var e1 in ends)
			// {
			// 	var empty = displacements.Select(d => e1.Add(d)).Where(c => !allcubes.Contains(c)).ToArray();
			// 	if (empty.Length == 1)
			// 	{
			// 		// possible internal path, follow it
			// 		var from = e1;
			// 		var p = empty.Single();
			// 		Console.WriteLine($"Follow {from}");
			// 		var seen = 2;
			// 		while (true)
			// 		{
			// 			Console.WriteLine($"  Look into {p}");
			// 			if (ends.Contains(p))
			// 				break;
			// 			var nexts = displacements
			// 				.Select(d => p.Add(d))
			// 				.Where(c => c != from)
			// 				.Where(c => !allcubes.Contains(c))
			// 				.ToArray();
			// 			if (nexts.Length > 1)
			// 			{
			// 				// open space, stop
			// 				seen = 0;
			// 				break;
			// 			}
			// 			var next = nexts.Single();
			// 			seen++;
			// 			from = p;
			// 			p = next;
			// 		}
			// 		if (seen > 0)
			// 		{
			// 			internalsides += seen * 4 + 2;
			// 		}
					
			// 	}
			// }

			return externals - internalsides / 2;
		}

	}

	public record Point3D(int X, int Y, int Z)
	{
		public override int GetHashCode()
		{
			return X * 1000000 + Y * 1000 + Z;
		}
		public override string ToString() => $"{X},{Y},{Z}";
	}

	internal static class Extensions
	{
		public static Point3D Add(this Point3D a, Point3D b)
		{
			return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
	}
}
