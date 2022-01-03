using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day22
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 22";
		public override int Year => 2021;
		public override int Day => 22;

		public void Run()
		{
			//Run("test1").Part1(39).Part2(39);

			//Run("test2").Part1(590784).Part2(0);
			//Run("test2.5").Part1(590784).Part2(590784);
			//Run("test1").Part2(39);
			//Run("test2.5").Part2(590784);
						Run("test3").Part2(2758514936282235);

		//				Run("input").Part1(596989).Part2(0);

		}

		protected override long Part1(string[] input)
		{
			var space = new bool[101,101,101];
			foreach (var s in input)
			{
				// on x=10..12,y=10..12,z=10..12
				var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
				var xx1 = Math.Max(x1, -50);
				var xx2 = Math.Min(x2, 50);
				var yy1 = Math.Max(y1, -50);
				var yy2 = Math.Min(y2, 50);
				var zz1 = Math.Max(z1, -50);
				var zz2 = Math.Min(z2, 50);
				var on = set == "on";
				for (var x = xx1; x <= xx2; x++)
				{
					for (var y = yy1; y <= yy2; y++)
					{
						for (var z = zz1; z <= zz2; z++)
						{
							space[x + 50, y + 50, z + 50] = on;
						}
					}
				}
			}

			var n = 0;
			foreach (var b in space)
			{
				if (b) n++;
			}

			return n;
		}

		internal class Point3D
		{
			public Point3D(int x, int y, int z) => (X, Y, Z) = (x, y, z);
			public int X;
			public int Y;
			public int Z;
			public override string ToString() => $"({X},{Y},{Z})";

			public Point3D Diff(Point3D other) => new Point3D(other.X - X, other.Y - Y, other.Z - Z);
			public Point3D Offset(Point3D offset) => new Point3D(X + offset.X, Y + offset.Y, Z + offset.Z);
		}

		internal class Cube
		{
			public Cube(int x1, int x2, int y1, int y2, int z1, int z2)
			{
				Top = new Point3D(x1, y1, z1);
				Bot = new Point3D(x2, y2, z2);
				Corners = new Point3D[]
				{
					new Point3D(x1, y1, z1),
					new Point3D(x1, y1, z2),
					new Point3D(x1, y2, z1),
					new Point3D(x1, y2, z2),
					new Point3D(x2, y1, z1),
					new Point3D(x2, y1, z2),
					new Point3D(x2, y2, z1),
					new Point3D(x2, y2, z2)
				};
			}
			public readonly Point3D Top;
			public readonly Point3D Bot;
			public readonly Point3D[] Corners;
			public List<Cube> Holes = new List<Cube>();
			public long Size => (long)(Bot.X - Top.X + 1) * (Bot.Y - Top.Y + 1) * (Bot.Z - Top.Z + 1);
			//public long Size => (long)(Bot.X - Top.X + 1) * (Bot.Y - Top.Y + 1) * (Bot.Z - Top.Z + 1) - Cardinality(1, Holes);

			public override string ToString() => $"size={Size} #holes={Holes.Count}";

			//public long CountDots()
			//{
			//	var dots = Size - Holes.Select(h => h.Size).Sum();
			//	return dots;
			//}


			public bool Contains(Point3D p)
			{
				if (p.X < Top.X || p.X > Bot.X) return false;
				if (p.Y < Top.Y || p.Y > Bot.Y) return false;
				if (p.Z < Top.Z || p.Z > Bot.Z) return false;
				return true;
			}

			public bool HasOverlap(Cube other, out Cube overlap)
			{
				overlap = other.Corners.Any(p => Contains(p)) ? Overlap(other) : null;
				if (overlap != null)
                {
					foreach (var h in other.Holes)
					{
						if (overlap.HasOverlap(h, out var overlap2))
						{
							overlap.Holes.Add(overlap2);
						}
					}
				}
				return overlap != null;
			}

			public Cube Overlap(Cube other)
			{
				var x1 = Math.Max(Top.X, other.Top.X);
				var x2 = Math.Min(Bot.X, other.Bot.X);
				var y1 = Math.Max(Top.Y, other.Top.Y);
				var y2 = Math.Min(Bot.Y, other.Bot.Y);
				var z1 = Math.Max(Top.Z, other.Top.Z);
				var z2 = Math.Min(Bot.Z, other.Bot.Z);
				return new Cube(x1, x2, y1, y2, z1, z2);
			}
			//public void OverlapWith(Cube other, bool on)
			//{
			//	if (HasOverlap(other, out var overlap))
			//	{
			//		foreach (var (o, on2) in Overlaps)
			//		{
			//			if (on != on2)
			//				o.OverlapWith(overlap, !on);
			//		}
			//		Overlaps.Add((overlap, on));
			//	}
			//}
		}

		protected override long Part2(string[] input)
		{
			var cubes = new List<Cube>();
//			var extras = new List<Cube>();

			foreach (var s in input)
			{
				// on x=10..12,y=10..12,z=10..12
				var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
				var on = set == "on";
				var cube = new Cube(x1, x2, y1, y2, z1, z2);

				if (on)
				{
					//foreach (var c in cubes.Concat(extras).ToArray())
					//{
					//	if (cube.HasOverlap(c, out var newUnion))
					//	{
					//		//// If c has any holes then they're not part of this union
					//		//foreach (var hc in c.Holes)
					//		//{
					//		//	if (newUnion.HasOverlap(hc, out var holeoverlap))
					//		//	{
					//		//		newUnion.Holes.Add(holeoverlap);
					//		//	}
					//		//}
					//		extras.Add(newUnion);
					//	}
					//}
					cubes.Add(cube);
				}
				else
                {
					foreach (var c in cubes)//.Concat(extras))
					{
						if (cube.HasOverlap(c, out var overlap))
						{
							c.Holes.Add(overlap);
						}
					}
				}


			}

			//var values = Enumerable.Range(1, 8);
			//foreach (var x in MathHelper.Combinations(values, 1))
   //         {
			//	Console.WriteLine(string.Join(" ", x));
   //         }

			var grandtotal = Cardinality(1, cubes);
			var grandtotal2 = Cardinality2(cubes);
            if (grandtotal != grandtotal2)
                throw new Exception();
            return grandtotal;

			var allholes = cubes.SelectMany(c => c.Holes).ToList();
			var dots = cubes.Select(c => Cardinality(1, c.Holes)).Sum();

			//var dots = 0L;
			//foreach (var cube in cubes)
			//{
			//	//	Console.Write($"holes={cube.Holes.Count} size={cube.Size}");
			//	var holecount = Cardinality(1, cube.Holes);
			//	dots += holecount;
			//}

			//var n = grandtotal - CountDots(extras);
			var n = grandtotal - dots;
			return n;
		}

		//private static long CountDots(List<Cube> cubes)
		//{
		//	//var overlapcounts = new List<int>();
		//	//for (var i = 0; i < cubes.Count; i++)
		//	//{
		//	//	var overlaps = 0;
		//	//	for (var j = i + 1; j < cubes.Count; j++)
		//	//	{
		//	//		if (cubes[i].HasOverlap(cubes[j], out var o))
		//	//		{
		//	//			overlaps++;
		//	//		}
		//	//	}
		//	//	overlapcounts.Add(overlaps);
		//	//}
		//	//foreach (var o in overlapcounts.OrderBy(o => o))
		//	//         {
		//	//	Console.WriteLine($"  overlapcount={o}");
		//	//}

		//	var dots = 0L;
		//	foreach (var cube in cubes.OrderBy(x => x.Holes.Count))
		//	{
		//		//	Console.Write($"holes={cube.Holes.Count} size={cube.Size}");
		//		var holecount = Cardinality(1, cube.Holes);
		//		var dotcount = cube.Size - holecount;
		//		if (holecount < 0)
		//			;
		//		Console.WriteLine($"  #holes={holecount} dots={dotcount}");
		//		dots += dotcount;
		//	}
		//	return dots;
		//}

		//private static IEnumerable<Cube> Unions(IEnumerable<Cube> unions, IEnumerable<Cube> set)
		//      {
		//	var overlaps = unions
		//		.SelectMany((s1,i) => set
		//			.Skip(i)
		//			.Select(s2 => s1.HasOverlap(s2, out var o) ? o : null)
		//			.Where(o => o != null)
		//		);
		//	return overlaps;
		//}

		//private static long UnionCardinality(List<Cube> set)
		//      {
		//	var unions = set.SelectMany((s1, i) => set.Skip(i + 1).Select(s2 => s1.HasOverlap(s2, out var o) ? o : null).Where(o => o != null)).ToList();
		//	var sum = unions.Select(o => o.Size).Sum();
		//	return sum == 0 ? 0 : sum - UnionCardinality(unions);
		//}

		private static long Cardinality2(IEnumerable<Cube> cubes)
        {
			var set = cubes.ToArray();
			var allcubes = Enumerable.Range(0, set.Length);
			return Cardinality(1);

			long Cardinality(int depth)
			{
				var sum = 0L;
				foreach (var indexes in MathHelper.Combinations(allcubes, depth))
				{
					var union = set[indexes.First()];
					foreach (var c in indexes.Skip(1).Select(i => set[i]))
					{
						if (!union.HasOverlap(c, out union))
							break;
					}
					sum += union?.Size ?? 0;
				}
				return sum == 0 ? 0 : sum - Cardinality(depth + 1);
			}
		}




		private static long Cardinality(int lookat, List<Cube> set)
		{
			if (lookat == 1)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					sum += set[i].Size;
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}
			if (lookat == 2)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							sum += o.Size;
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}
			if (lookat == 3)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									sum += o2.Size;
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}
			if (lookat == 4)
			{
				var sum = 0L;
				int os1 = 0, os2 = 0, os3 = 0;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							os1++;
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									os2++;
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											os3++;
											sum += o3.Size;
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}
			if (lookat == 5)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													sum += o4.Size;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			if (lookat == 6)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													for (var k4 = k3 + 1; k4 < set.Count; k4++)
													{
														if (o4.HasOverlap(set[k4], out var o5))
														{
															sum += o5.Size;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			if (lookat == 7)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													for (var k4 = k3 + 1; k4 < set.Count; k4++)
													{
														if (o4.HasOverlap(set[k4], out var o5))
														{
															for (var k5 = k4 + 1; k5 < set.Count; k5++)
															{
																if (o5.HasOverlap(set[k5], out var o6))
																{
																	sum += o6.Size;
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			if (lookat == 8)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													for (var k4 = k3 + 1; k4 < set.Count; k4++)
													{
														if (o4.HasOverlap(set[k4], out var o5))
														{
															for (var k5 = k4 + 1; k5 < set.Count; k5++)
															{
																if (o5.HasOverlap(set[k5], out var o6))
																{
																	for (var k6 = k5 + 1; k6 < set.Count; k6++)
																	{
																		if (o6.HasOverlap(set[k6], out var o7))
																		{
																			sum += o7.Size;
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			if (lookat == 9)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													for (var k4 = k3 + 1; k4 < set.Count; k4++)
													{
														if (o4.HasOverlap(set[k4], out var o5))
														{
															for (var k5 = k4 + 1; k5 < set.Count; k5++)
															{
																if (o5.HasOverlap(set[k5], out var o6))
																{
																	for (var k6 = k5 + 1; k6 < set.Count; k6++)
																	{
																		if (o6.HasOverlap(set[k6], out var o7))
																		{
																			for (var k7 = k6 + 1; k7 < set.Count; k7++)
																			{
																				if (o7.HasOverlap(set[k7], out var o8))
																				{
																					sum += o8.Size;
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			if (lookat == 10)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													for (var k4 = k3 + 1; k4 < set.Count; k4++)
													{
														if (o4.HasOverlap(set[k4], out var o5))
														{
															for (var k5 = k4 + 1; k5 < set.Count; k5++)
															{
																if (o5.HasOverlap(set[k5], out var o6))
																{
																	for (var k6 = k5 + 1; k6 < set.Count; k6++)
																	{
																		if (o6.HasOverlap(set[k6], out var o7))
																		{
																			for (var k7 = k6 + 1; k7 < set.Count; k7++)
																			{
																				if (o7.HasOverlap(set[k7], out var o8))
																				{
																					for (var k8 = k7 + 1; k8 < set.Count; k8++)
																					{
																						if (o8.HasOverlap(set[k8], out var o9))
																						{
																							sum += o9.Size;
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			if (lookat == 11)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													for (var k4 = k3 + 1; k4 < set.Count; k4++)
													{
														if (o4.HasOverlap(set[k4], out var o5))
														{
															for (var k5 = k4 + 1; k5 < set.Count; k5++)
															{
																if (o5.HasOverlap(set[k5], out var o6))
																{
																	for (var k6 = k5 + 1; k6 < set.Count; k6++)
																	{
																		if (o6.HasOverlap(set[k6], out var o7))
																		{
																			for (var k7 = k6 + 1; k7 < set.Count; k7++)
																			{
																				if (o7.HasOverlap(set[k7], out var o8))
																				{
																					for (var k8 = k7 + 1; k8 < set.Count; k8++)
																					{
																						if (o8.HasOverlap(set[k8], out var o9))
																						{
																							for (var k9 = k8 + 1; k9 < set.Count; k9++)
																							{
																								if (o9.HasOverlap(set[k9], out var o10))
																								{
																									sum += o10.Size;
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			if (lookat == 12)
			{
				var sum = 0L;
				for (var i = 0; i < set.Count; i++)
				{
					for (var j = i + 1; j < set.Count; j++)
					{
						if (set[i].HasOverlap(set[j], out var o))
						{
							for (var k = j + 1; k < set.Count; k++)
							{
								if (o.HasOverlap(set[k], out var o2))
								{
									for (var k2 = k + 1; k2 < set.Count; k2++)
									{
										if (o2.HasOverlap(set[k2], out var o3))
										{
											for (var k3 = k2 + 1; k3 < set.Count; k3++)
											{
												if (o3.HasOverlap(set[k3], out var o4))
												{
													for (var k4 = k3 + 1; k4 < set.Count; k4++)
													{
														if (o4.HasOverlap(set[k4], out var o5))
														{
															for (var k5 = k4 + 1; k5 < set.Count; k5++)
															{
																if (o5.HasOverlap(set[k5], out var o6))
																{
																	for (var k6 = k5 + 1; k6 < set.Count; k6++)
																	{
																		if (o6.HasOverlap(set[k6], out var o7))
																		{
																			for (var k7 = k6 + 1; k7 < set.Count; k7++)
																			{
																				if (o7.HasOverlap(set[k7], out var o8))
																				{
																					for (var k8 = k7 + 1; k8 < set.Count; k8++)
																					{
																						if (o8.HasOverlap(set[k8], out var o9))
																						{
																							for (var k9 = k8 + 1; k9 < set.Count; k9++)
																							{
																								if (o9.HasOverlap(set[k9], out var o10))
																								{
																									for (var k10 = k9 + 1; k10 < set.Count; k10++)
																									{
																										if (o10.HasOverlap(set[k10], out var o11))
																										{
																											sum += o11.Size;
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return sum == 0 ? 0 : sum - Cardinality(lookat + 1, set);
			}

			return 0;
		}


		//for (var i = 0; i < cube.Overlaps.Count; i++)
		//{
		//	var (overlap, on) = cube.Overlaps[i];
		//	if (on)
		//	{
		//		overlappedDots += overlap.Size;
		//	}
		//	else
		//	{
		//		dots -= overlap.Size;
		//		for (var j = 0; j < i; j++)
		//		{
		//			var (o2, on2) = cube.Overlaps[j];
		//			if (o2.OverlapsWith(overlap))
		//			{
		//				if (on2)
		//				{
		//					overlappedDots -= o2.Overlap(overlap).Size;
		//				}
		//				else
		//				{
		//					for (var k = 0; k < j; k++)
		//					{
		//						var (o3, on3) = cube.Overlaps[k];
		//						if (o3.OverlapsWith(o2))
		//						{
		//							if (!on2)
		//							{
		//								overlappedDots -= o3.Overlap(o2).Size;
		//							}
		//							else
		//							{

		//								//overlappedDots += o2.Overlap(overlap).Size;
		//							}
		//						}
		//					}

		//					//overlappedDots += o2.Overlap(overlap).Size;
		//				}
		//			}
		//		}
		//	}
		//}
	}
}
