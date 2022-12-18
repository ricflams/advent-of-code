using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day17
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 17";
		public override int Year => 2022;
		public override int Day => 17;

		public void Run()
		{
		//	Run("test1").Part1(3068).Part2(1514285714288);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(3133).Part2(0);

			// part 2
			// 1597719479975 too high
			// 1597719479974 too high
			// 1597719479973 too high
		}

		private readonly string[][] Rocks = new string[][]
		{
			new string[]{ "####" },
			new string[]{ ".#.", "###", ".#." },
			new string[]{ "..#", "..#", "###" },
			new string[]{ "#", "#", "#", "#" },
			new string[]{ "##", "##"}
		};

		protected override long Part1(string[] input)
		{
			var width = 7;
			var top = 0;

			Console.WriteLine(input[0].Length);
			var map = new CharMap('.');
			for (var x = 1; x < width+1; x++)
				map[x][0] = '-';
			map[0][0] = map[width+1][0] = '+';			

			void FillEmptyRow(int y)
			{
				for (var x = 1; x < width+1; x++)
					map[x][y] = '.';
				map[0][y] = map[width+1][y] = '|';
			}


			var ip = 0;
			var jets = input[0];
			for (var j = 0; j < 2022; j++)
			{
				//DrawMap();
				var rock = Rocks[j % 5];
				for (var i = 0; i < 3 + rock.Length; i++)
				{
					FillEmptyRow(top - 1 - i);
				}
				//DrawMap();


				var w = rock[0].Length;
				var x = 2 + 1; // [0] is wall
				var y = top - (3 + rock.Length);

	
				while (true)
				{
					var jet = jets[ip++ % jets.Length];
					if (jet == '<')
					{
						if (CanMove(rock, x-1, y))
							x--;
					}
					else
					{
						if (CanMove(rock, x+1, y))
							x++;
					}
					if (!CanMove(rock, x, y+1))
						break;
					y++;
				}
				Draw(rock, x, y);
				if (y < top)
					top = y;
			}

			void DrawMap()
			{
				Console.WriteLine();
				Console.WriteLine("Map:");
				map.ConsoleWrite();
			}

			bool CanMove(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#' && map[x0+x][y0+y] != '.')
							return false;
					}
				}
				return true;
			}

			void Draw(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#')
							map[x0+x][y0+y] = '#';
					}
				}
			}

			return -top;
		}

		protected override long Part2(string[] input)
		{
			var width = 7;
			var top = 0;

			Console.WriteLine(input[0].Length);
			var map = new CharMap('.');
			for (var x = 1; x < width+1; x++)
				map[x][0] = '-';
			map[0][0] = map[width+1][0] = '+';			

			void FillEmptyRow(int y)
			{
				for (var x = 1; x < width+1; x++)
					map[x][y] = '.';
				map[0][y] = map[width+1][y] = '|';
			}


			var seen = new Dictionary<string, (long Top, long Simul)>();

			var ip = 0;
			var jets = input[0];
			var heights = new List<long>();
			for (var j = 0L; j < 202200000; j++)
			{
				heights.Add(-top);


				//DrawMap();
				var rock = Rocks[j % 5];
				for (var i = 0; i < 3 + rock.Length; i++)
				{
					FillEmptyRow(top - 1 - i);
				}
				//DrawMap();


				var w = rock[0].Length;
				var x = 2 + 1; // [0] is wall
				var y = top - (3 + rock.Length);


				while (true)
				{
					var jet = jets[ip++ % jets.Length];
					if (jet == '<')
					{
						if (CanMove(rock, x-1, y))
							x--;
					}
					else
					{
						if (CanMove(rock, x+1, y))
							x++;
					}
					if (!CanMove(rock, x, y+1))
						break;
					y++;
				}
				Draw(rock, x, y);
				if (y < top)
					top = y;

				if (j > 0 && j % (5*jets.Length) == 0)
				{
					var roundtrip = j / (5*jets.Length);
					var seenx = new bool[width];
					var key = "";
					for (var yy = 0; !seenx.All(x => x); yy++)
					{
						for (var xx = 0; xx < width; xx++)
						{
							var ch = map[xx+1][top+yy];
							key += ch;
							if (ch == '#')
								seenx[xx] = true;
						}
					}
					var height = -top;
		//			Console.WriteLine($"{j} {height}: {key}");
					if (seen.TryGetValue(key, out var info))
					{
						var (oldj, oldheight) = info;
						var oldroundtrip = oldj / (5*jets.Length);
						var heightPerLoop = height - oldheight;
						var nrocks = 1000000000000 - oldj;
						var rocksPerLoop = j - oldj;
						var fullLoops = nrocks / rocksPerLoop;
						var remainRocks = nrocks % rocksPerLoop;

						var fullheightForLoops = fullLoops * heightPerLoop;
						var heightForRemainLoops = heights[(int)oldj + (int)remainRocks];

						var allRocks = oldj + fullLoops * rocksPerLoop + remainRocks;
						var allHeight = fullheightForLoops + heightForRemainLoops;

						// for (var d = 0; d >= -4; d--)
						// {
						// 	var hidx = j + d;
						// 	Console.WriteLine($"  height[{hidx}] = {heights[hidx]}");
						// }
						// for (var d = 4; d >= -4; d--)
						// {
						// 	var hidx = oldj + d;
						// 	Console.WriteLine($"  height[{hidx}] = {heights[hidx]}");
						// }						
						// for (var d = 0; d >= -4; d--)
						// {
						// 	var hidx = oldj + d;
						// 	Console.WriteLine($"  delta = {heights[j + d] - heights[oldj + d]}");
						// }		

						Console.WriteLine($"bingo! trip={roundtrip} ({oldroundtrip} {roundtrip-oldroundtrip}) j={j} height={height}   oldj={oldj} oldheight={oldheight} fullheight={allHeight} rocks={allRocks}");
						//Console.WriteLine($"{allHeight - 1514285714288} too big");

					//	return allHeight;
						// 15479 53417738 too big
						// 15479 50527958 not right 
						// 15479 44337225 too low
						// 1546368038743 "expected"
						// 1514285714288 part 2 for test
						// 154795 2878829 ??
						// 15479 53417738
					}
					seen[key] = (j, height);
				}


			}

			// if (Enumerable.Range(1, width).All(x => map[x][top] == '#'))
			// {
			// 	Console.WriteLine(top);
			// }

	//		DrawMap();

			// var lasty = 0;
			// for (var y = top; y < 0; y++)
			// {
			// 	if (Enumerable.Range(1, width).All(x => map[x][y] == '#'))
			// 	{
			// 		Console.WriteLine(y - lasty);
			// 		lasty = y;
			// 	}
			// }

			void DrawMap()
			{
				Console.WriteLine();
				Console.WriteLine("Map:");
				map.ConsoleWrite();
			}

			bool CanMove(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#' && map[x0+x][y0+y] != '.')
							return false;
					}
				}
				return true;
			}

			void Draw(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#')
							map[x0+x][y0+y] = '#';
					}
				}
			}

			return -top;
		}


		/////////////////////////////////////////////////////////////////////////////////////////////////////

		internal class Thing
		{
			//private readonly 
			public Thing(string[] lines)
			{
			}
		}

		class SomeGraph : Graph<HashSet<uint>> { }

		internal void Sample(string[] input)
		{
			{
				var v = input.Select(int.Parse).ToArray();
			}
			{
				var v = input[0].ToIntArray();
			}
			{
				var things = input
					.Skip(1)
					.GroupByEmptyLine()
					.Select(lines => new Thing(lines))
					.ToMutableArray();
			}
			{
				var map = new SparseMap<int>();
				foreach (var s in input)
				{
					var (x1, y1, x2, y2) = s.RxMatch("%d,%d -> %d,%d").Get<int, int, int, int>();
				}
			}
			{
				var map = CharMap.FromArray(input);
				var maze = new Maze(map)
					.WithEntry(map.FirstOrDefault(c => c == '0')); // or Point.From(1, 1);
				var dest = Point.From(2, 3);
				var graph = Graph<char>.BuildUnitGraphFromMaze(maze);
				var steps = graph.ShortestPathDijkstra(maze.Entry, dest);
			}
			{
				var map = new CharMap('#');
				var maze = new Maze(map).WithEntry(Point.From(1, 1));
				var graph = SomeGraph.BuildUnitGraphFromMaze(maze);
				var queue = new Queue<(SomeGraph.Vertex, uint, int)>();
				queue.Enqueue((graph.Root, 0U, 0));
				while (queue.Any())
				{
					var (node, found, steps) = queue.Dequeue();
					if (node.Value.Contains(found))
						continue;
					node.Value.Add(found);
					var ch = map[node.Pos];
					if (char.IsDigit(ch))
					{

					}
					foreach (var n in node.Edges.Keys.Where(n => !n.Value.Contains(found)))
					{
						queue.Enqueue((n, found, steps + 1));
					}
				}
			}
			{
				var ship = new Pose(Point.Origin, Direction.Right);
				foreach (var line in input)
				{
					var n = int.Parse(line.Substring(1));
					switch (line[0])
					{
						case 'N': ship.MoveUp(n); break;
						case 'S': ship.MoveDown(n); break;
						case 'E': ship.MoveRight(n); break;
						case 'W': ship.MoveLeft(n); break;
						case 'L': ship.RotateLeft(n); break;
						case 'R': ship.RotateRight(n); break;
						case 'F': ship.Move(n); break;
						default:
							throw new Exception($"Unknown action in {line}");
					}
				}
				var dist = ship.Point.ManhattanDistanceTo(Point.Origin);
			}
			{
				var departure = int.Parse(input[0]);
				var id = input[1]
					.Replace(",x", "")
					.Split(",")
					.Select(int.Parse)
					.Select(id => new
					{
						Id = id,
						Time = id - departure % id
					})
					.OrderBy(x => x.Time)
					.First();
			}
			{
				var map = CharMatrix.FromArray(input);
				for (var i = 0; i < 100; i++)
				{
					map = map.Transform((ch, adjacents) =>
					{
						var n = 0;
						foreach (var c in adjacents)
						{
							if (c == '|' && ++n >= 3)
								return '|';
						}
						return ch;
					});
				}
			}
		}

	}
}
