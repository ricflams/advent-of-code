using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2020.Day17
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 17;

		public void Run()
		{

			//   .#.
			//   ..#
			//   ###

			RunFor("test1", 112, 848);
			//RunFor("test2", null, null);
			RunFor("input", 240, 1180);
		}

		protected override int Part1(string[] input)
		{
			return CountActivePoints(input, 3);
			//var cube = CharMap3D.FromArray(input, '.');

			//var pt = cube.AllPoints(ch => ch == '#').ToArray();

			//var zzz = pt.First().LookAround().Count();

			//for (var i = 0; i < 6; i++)
			//{

			//	var (min, max) = cube.SpanningCube();
			//	min = Point3D.From(min.X - 1, min.Y - 1, min.Z - 1);
			//	max = Point3D.From(max.X + 1, max.Y + 1, max.Z + 1);

			//	var cube2 = new CharMap3D();
			//	foreach (var pos in Point3D.Explore(min, max))
			//	{
			//		var ch = cube[pos];
			//		var actives = pos.LookAround().Count(p => cube[p] == '#');
			//		if (ch == '#')
			//		{
			//			cube2[pos] = actives == 2 || actives == 3 ? '#' : '.';
			//		}
			//		else
			//		{
			//			cube2[pos] = actives == 3 ? '#' : '.';
			//		}
			//	}

			//	cube = cube2;

			//	//cube = cube.Transform((pos, ch) =>
			//	//{

			//	//	//If a cube is active and exactly 2 or 3 of its neighbors are also active, the cube remains active. Otherwise, the cube becomes inactive.
			//	//	//If a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active. Otherwise, the cube remains inactive.

			//	//	var actives = pos.LookAround().Count(p => cube[p] == '#');
			//	//	if (ch == '#')
			//	//	{
			//	//		return actives == 2 || actives == 3 ? '#' : '.';
			//	//	}
			//	//	return actives == 3 ? '#' : '.';
			//	//});

			//	var n = cube.AllPoints(ch => ch == '#').Count();
			//	Console.WriteLine(n);
			//}

			//var result = cube.AllPoints(ch => ch == '#').Count();

			//return result;
		}




		internal class Space
		{
			private readonly int _dimensions;
			public Space(int dimensions)
			{
				_dimensions = dimensions;
				if (_dimensions > 4)
					throw new Exception("Max 4 dimensions supported");
			}
			public HashSet<uint> Active = new HashSet<uint>();
			private static uint IdFrom(sbyte[] p) =>
				(uint)p[0] << 24 & 0xff000000 |
				(uint)p[1] << 16 & 0xff0000 |
				(uint)p[2] << 8 & 0xff00 |
				(uint)p[3] & 0xff;
			private static sbyte[] FromId(uint p) => new sbyte[] 
			{
				(sbyte)(p >> 24 & 0xff),
				(sbyte)(p >> 16 & 0xff),
				(sbyte)(p >> 8 & 0xff),
				(sbyte)(p & 0xff)
			};
			public void MergeWith(Space other) => Active.UnionWith(other.Active);
			public bool IsSet(uint p) => Active.Contains(p);
			public bool Set(uint p) => Active.Add(p);
			public bool Set(sbyte[] p) => Active.Add(IdFrom(p));
			public Space NeighboursOf(uint p)
			{
				var space = new Space(_dimensions);
				foreach (var delta in MathHelper.PlusZeroMinusSequence(_dimensions))
				{
					var neighbor = FromId(p);
					for (var i = 0; i < delta.Length; i++)
					{
						neighbor[i] += (sbyte)delta[i];
					}
					space.Set(neighbor);
				}
				return space;
			}
		}

		private int CountActivePoints(string[] input, int dimensions)
		{
			var points = CharMap
				.FromArray(input, '.')
				.AllPoints(ch => ch == '#');

			//var hyperpoints = pt.Select(p => new[] { p.X, p.Y, 0, 0 }).ToList();

			var space = new Space(dimensions);
			foreach (var p in points)
			{
				space.Set(new[] { (sbyte)p.X, (sbyte)p.Y, (sbyte)0, (sbyte)0 });
			}

			//for (var i = 1; i < 5; i++)
			//{
			//	Console.WriteLine("Base " + i);
			//	foreach (var v in MathHelper.PlusZeroMinusSequence(i))
			//	{
			//		var xx = string.Join(" ", v.Select(x => x.ToString()));
			//		Console.WriteLine(xx);
			//	}
			//}


			for (var i = 0; i < 6; i++)
			{


				var pointsToInvestigate = new Space(dimensions);
				foreach (var p in space.Active)
				{
					var neighbours = space.NeighboursOf(p);
					pointsToInvestigate.MergeWith(neighbours);
				}

				var newSpace = new Space(dimensions);

				foreach (var p in pointsToInvestigate.Active)
				{
					var neighbours = space.NeighboursOf(p);
					var isActive = space.IsSet(p);
					var actives = neighbours.Active.Count(x => space.IsSet(x));
					if (isActive)
					{
						actives--;
					}
					if (isActive && (actives == 2 || actives == 3) || !isActive && actives == 3)
					{
						newSpace.Set(p);
					}
				}

				space = newSpace;

				//var min1 = hyperpoints.Min(x => x[0]) - 1;
				//var max1 = hyperpoints.Max(x => x[0]) + 1;
				//var min2 = hyperpoints.Min(x => x[1]) - 1;
				//var max2 = hyperpoints.Max(x => x[1]) + 1;
				//var min3 = hyperpoints.Min(x => x[2]) - 1;
				//var max3 = hyperpoints.Max(x => x[2]) + 1;
				//var min4 = hyperpoints.Min(x => x[3]) - 1;
				//var max4 = hyperpoints.Max(x => x[3]) + 1;

				//var hyperpoints2 = new List<int[]>();

				////Console.WriteLine($"Check");

				//for (var p1 = min1; p1 <= max1; p1++)
				//{
				//	for (var p2 = min2; p2 <= max2; p2++)
				//	{
				//		for (var p3 = min3; p3 <= max3; p3++)
				//		{
				//			for (var p4 = min4; p4 <= max4; p4++)
				//			{

				//				var actives = 0;

				//				for (var dx = -1; dx <= 1; dx++)
				//				{
				//					for (var dy = -1; dy <= 1; dy++)
				//					{
				//						for (var dz = -1; dz <= 1; dz++)
				//						{
				//							for (var dt = -1; dt <= 1; dt++)
				//							{
				//								if (dx == 0 && dy == 0 && dz == 0 && dt == 0)
				//									continue;

				//								/// Neighbor
				//								if (hyperpoints.Any(hp => hp[0] == p1 + dx && hp[1] == p2 + dy && hp[2] == p3 + dz && hp[3] == p4 + dt))
				//								{
				//									actives++;
				//								}
				//							}
				//						}
				//					}
				//				}

				//				var isActive = hyperpoints.Any(hp => hp[0] == p1 && hp[1] == p2 && hp[2] == p3 && hp[3] == p4);

				//				//Console.WriteLine($"isActive={isActive}  actives={actives}");

				//				if (isActive && (actives == 2 || actives == 3) || !isActive && actives == 3)
				//				{
				//					hyperpoints2.Add(new[] { p1, p2, p3, p4 });
				//					//Console.WriteLine($"  active={p1},{p2},{p3},{p4}");
				//				}

				//			}

				//		}

				//	}
				//}

				//hyperpoints = hyperpoints2;
				//Console.WriteLine($"  count={hyperpoints.Count()}");
				//Console.WriteLine();
			}

			//var result2 = hyperpoints.Count();
			return space.Active.Count();

		}

		protected override int Part2(string[] input)
		{
			return CountActivePoints(input, 4);
		}
	}




	//  internal class Puzzle : ComboPart<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//  	protected override int Year => 2020;
	//  	protected override int Day => 17;
	//  
	//  	public void Run()
	//  	{
	//  		RunFor("test1", null, null);
	//  		//RunFor("test2", null, null);
	//  		//RunFor("input", null, null);
	//  	}
	//  
	//  	protected override (int, int) Part1And2(string[] input)
	//  	{
	//  
	//  
	//  
	//  
	//  
	//  		return (0, 0);
	//  	}
	//  }

}
