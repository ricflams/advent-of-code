using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2018.Day21
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Chronal Conversion";
		public override int Year => 2018;
		public override int Day => 21;

		public void Run()
		{
			//Run("test1").Part1(0).Part2(0);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var reg0 = 0;
			while (true)
			{
				Console.WriteLine(reg0);
				var computer = new Computer(input);
				computer.Regs[0] = reg0;
				if (computer.Run())
					break;
				reg0++;
			}

			return reg0;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}


		/////////////////////////////////////////////////////////////////////////////////////////////////////




		internal class Computer
		{
			public enum Opcode
			{
				addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr
			}

			public readonly int[] Regs = new int[6];
			public readonly int IpRegister;
			public readonly Ins[] Instructions;

			public int NumberOfExecutedInstructions { get; private set; }

			public Computer(string[] input)
			{
				// Example:
				// #ip 3
				// addi 3 16 3
				// seti 1 2 5
				IpRegister = input[0].RxMatch("#ip %d").Get<int>();
				Instructions = input
					.Skip(1)
					.Select(s =>
					{
						var (opcode, a, b, c) = s.RxMatch("%s %d %d %d").Get<string, int, int, int>();
						return new Ins
						{
							Opcode = Enum.Parse<Opcode>(opcode),
							A = a,
							B = b,
							C = c
						};
					})
					.ToArray();
			}

			public class Ins
			{
				public Opcode Opcode { get; init; }
				public int A { get; init; }
				public int B { get; init; }
				public int C { get; init; }
				public override string ToString() => $"{Opcode} {A} {B} {C}";
			}

			public bool Run()
			{
				var states = new HashSet<string>();

				for (var ip = 0; ip >= 0 && ip < Instructions.Length; ip++)
				{
					var ins = Instructions[ip];
					var (opcode, a, b, c) = (ins.Opcode, ins.A, ins.B, ins.C);

					Regs[IpRegister] = ip;

					switch (opcode)
					{
						case Opcode.addr: Regs[c] = Regs[a] + Regs[b]; break;
						case Opcode.addi: Regs[c] = Regs[a] + b; break;
						case Opcode.mulr: Regs[c] = Regs[a] * Regs[b]; break;
						case Opcode.muli: Regs[c] = Regs[a] * b; break;
						case Opcode.banr: Regs[c] = Regs[a] & Regs[b]; break;
						case Opcode.bani: Regs[c] = Regs[a] & b; break;
						case Opcode.borr: Regs[c] = Regs[a] | Regs[b]; break;
						case Opcode.bori: Regs[c] = Regs[a] | b; break;
						case Opcode.setr: Regs[c] = Regs[a]; break;
						case Opcode.seti: Regs[c] = a; break;
						case Opcode.gtir: Regs[c] = a > Regs[b] ? 1 : 0; break;
						case Opcode.gtri: Regs[c] = Regs[a] > b ? 1 : 0; break;
						case Opcode.gtrr: Regs[c] = Regs[a] > Regs[b] ? 1 : 0; break;
						case Opcode.eqir: Regs[c] = a == Regs[b] ? 1 : 0; break;
						case Opcode.eqri: Regs[c] = Regs[a] == b ? 1 : 0; break;
						case Opcode.eqrr: Regs[c] = Regs[a] == Regs[b] ? 1 : 0; break;
						default:
							throw new Exception($"Unknown opcode {opcode}");
					}

					ip = Regs[IpRegister];

					NumberOfExecutedInstructions++;

					var state = $"{ip}-{Regs[0]}-{Regs[1]}-{Regs[2]}-{Regs[3]}-{Regs[4]}-{Regs[5]}";
					if (states.Contains(state))
						return false;
					states.Add(state);
				}

				return true;
			}
		}




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
