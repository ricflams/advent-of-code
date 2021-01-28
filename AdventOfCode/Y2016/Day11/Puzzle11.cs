using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2016.Day11
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Radioisotope Thermoelectric Generators";
		public override int Year => 2016;
		public override int Day => 11;

		public void Run()
		{
			//RunPart1For("test1", 11);
			//RunFor("input", 37, 61);
			RunPart2For("input", 61);
		}



		protected override int Part1(string[] input)
		{
			// return Solve(input);
			return SolveMcts(input);

			// var floors0 = Floors.Create(input);

			// var memo = new HashSet<ulong>();
			// var queue = new Queue<(ulong, int)>();
			// queue.Enqueue((floors0, 0));
			// var result = 0;
			// var maxstep = 0;
			// var loops = 0;
			// var dups = 0;
			// while (queue.Any())
			// {
			// 	loops++;
			// 	var (floors, steps) = queue.Dequeue();
			// 	if (memo.Contains(floors))
			// 	{
			// 		dups++;
			// 		continue;
			// 	}
			// 	memo.Add(floors);

			// 	if (steps > maxstep)
			// 	{
			// 		maxstep = steps;
			// 		Console.WriteLine($"step={steps} queue={queue.Count()} loops={loops} dups={dups}");
			// 		// Floors.WriteToConsole(floors);
			// 		// Console.WriteLine();
			// 	}

			// 	if (Floors.AllMovedTo4thFloor(floors))
			// 	{
			// 		Console.WriteLine("############### BAM1: " + steps);
			// 		result = steps;
			// 		break;
			// 	}
			// 	var moves = Floors.ValidMoves(floors).Where(m => !memo.Contains(m));
			// 	//floors.WriteToConsole();
			// 	//Console.WriteLine($"Step {steps}: found {moves.Count()} queue={queue.Count()}:");
			// 	foreach (var m in moves)
			// 	{
			// 		//m.WriteToConsole();
			// 		queue.Enqueue((m, steps + 1));
			// 	}
			// 	//Console.WriteLine();
			// 	//Console.WriteLine();
			// }

			// return result;
		}

		protected override int Part2(string[] input)
		{
			var extra = new []
			{
				"An elerium generator.",
				"An elerium-compatible microchip.",
				"A dilithium generator.",
				"A dilithium-compatible microchip."
			};
			input[0] += string.Concat(extra);
			// var modinput = new string[input.Length];
			// input.CopyTo(modinput, 0);
			// modinput[0] = modinput[0] + "an elerium generator, an elerium-compatible microchip, a dilithium generator, a dilithium-compatible microchip.";
			//return Solve(input);

			//return SolveRecursive(input);
			return SolveMcts(input);
			//return Solve(input);
		}

		private int SolveRecursive(string[] input)
		{
			var BIGVALUE = 100000;
			var maxsteps = 200;//Solve(input);

			var floors0 = Floors.Create(input);
			var seen = new Dictionary<ulong, int>();
			var progress = 0;

			SolveFor(floors0, 0);
			return maxsteps;

			void SolveFor(ulong floors, int steps)
			{
				if (progress++ % 100000 == 0)
					Console.Write(".");
				if (seen.TryGetValue(floors, out var oldsteps) && steps >= oldsteps)
				{
					return;
				}
				seen[floors] = steps;
				if (Floors.AllMovedTo4thFloor(floors))
				{
					Console.Write($"#{steps}#");
					if (steps < maxsteps)
					{
						Console.Write($"!{steps}!");
						maxsteps = steps;
					}
					return;
				}
				if (steps >= maxsteps)
					return;// BIGVALUE;
				foreach (var m in Floors.ValidMoves(floors))
				{
					SolveFor(m, steps + 1);
				}
			}

		}

		class Node
		{
			public static readonly double C = Math.Sqrt(2);
			public Node[] Children { get; set; }
			public Node Parent { get; set; }
			public int Visited { get; set; }
			public int Wins { get; set; }
			public ulong Floors { get; set; }
			public int Steps { get; set; }
			public double Uct
			{
				get
				{
					if (Parent == null || Visited == 0)
						return 1000000;
					return (double)Wins / Visited + C*Math.Sqrt(Math.Log(Parent.Visited) / Visited);
				}
			}
		}

		private int SolveMcts(string[] input)
		{
			var Random = new Random();
			var floors0 = Floors.Create(input);

			var root = new Node
			{
				Parent = null,
				Visited = 0,
				Wins = 0,
				Floors = floors0,
				Steps = 0
			};

			var minsteps = 10000000;
			var solutionheight = 0;
			// var seen = new Dictionary<ulong, int>();

			for (var i = 0; i < 1000000; i++)
			{
				var leaf = Traverse(root);
				var result = Rollout(leaf);
				BackPropagate(root, leaf, result);
			}

			return minsteps;

			Node Traverse(Node node)
			{
				if (node.Visited == 0)
				{
					var moves = Floors.ValidMoves(node.Floors);
					node.Children = moves.Select(m => new Node
					{
						Parent = node,
						Visited = 0,
						Wins = 0,
						Floors = m,
						Steps = node.Steps + 1
					})
					.ToArray();
				}
				var pick = node.Children.OrderByDescending(n => n.Uct).FirstOrDefault();
				return pick;
			}

			int Rollout(Node n)
			{
				var floors = n.Floors;
				for (var i = 0; i < 500 && n.Steps + i < minsteps; i++)
				{
					var height = Floors.Height(floors);
					if (n.Steps + i + (solutionheight - height)*1.8 >= minsteps)
						break;
					// if (seen.TryGetValue(floors, out var steps) && steps < n.Steps + i)
					// {
					// 	Console.Write("x");
					// 	return 0;
					// }
					if (Floors.AllMovedTo4thFloor(floors))
					{
						minsteps = n.Steps + i;
						solutionheight = Floors.Height(floors);
						// seen[floors] = minsteps;
						Console.Write($"[{minsteps}]");
						return 1;
					}
					var moves = Floors.ValidMoves(floors).ToArray();
					if (moves.Length == 0)
					{
						//Console.Write("-");
						return 0;
					}
					if (Random.NextDouble() < .6)
					{
						floors = moves.OrderByDescending(Floors.Height).First();
					}
					else
					{
						floors = moves.PickRandom();
					}
				}
				//Console.Write(".");
				return 0;
			}

			void BackPropagate(Node root, Node node, int win)
			{
				while (node != root)
				{
					node.Visited++;
					node.Wins += win;
					node = node.Parent;
				}
			}

		}

		private int Solve(string[] input)
		{
			var floors0 = Floors.Create(input);
			var seen = new HashSet<ulong>();
			//var queued = new HashSet<ulong>();
			var backlog = new List<(ulong, int, int)>();
			var result = 0;

			var maxstep = 0;
			//var maxlifted = 0;
			var seen1 = 0;
			var seen2 = 0;

			backlog.Add((floors0, 0, 0));
			while (backlog.Any())
			{

				//Console.Write($"<{backlog.Count} {backlog.Count(x => !seen.Contains(x.Item1))}>");
				var queue = backlog.OrderByDescending(x => x.Item3).Take(100000).ToArray();


				//var queue = new Queue<(ulong, int, int)>(climbers);
				backlog.Clear();

				foreach (var (floors, steps, _) in queue)
				//while (queue.Any())
				{
					//var (floors, steps, _) = queue.Dequeue();
					if (seen.Contains(floors))
					{
						seen1++;
						continue;
					}
					seen.Add(floors);
					// seen.Add(floors>>16);
					// seen.Add(floors>>32);

					// if (memo.Contains(floors.Identity))
					// {
					// 	continue;
					// }
					// memo.Add(floors.Identity);
					// var lifted = floors.Lifted;
					// if (lifted > maxlifted)
					// {
					// 	maxlifted = lifted;
					// }
					// if (lifted < maxlifted - 7)
					// {
					// 	continue;
					// }

					if (steps > maxstep)
					{
						maxstep = steps;
						Console.Write($"[{steps} {queue.Count()}]");
						//Floors.WriteToConsole(floors);
						//Console.WriteLine();
					}

					if (Floors.AllMovedTo4thFloor(floors))
					{
						Console.WriteLine();
						Console.WriteLine("############### BAM2: " + steps);
						Console.WriteLine($"seen1={seen1} seen2={seen2}");
						result = steps;
						break;
					}
					//var moves = floors.ValidMoves().Where(m => m.Lifted >= maxlifted-7 && !m.Identities.Any(id => memo.Contains(id)));
					var moves = Floors.ValidMoves(floors);
					//Floors.WriteToConsole(floors);
					//Console.WriteLine($"Step {steps}: found {moves.Count()} queue={queue.Count()}:");
					foreach (var m in moves)
					{
						//Floors.WriteToConsole(m);
						//if (memo.Contains(m))
						if (seen.Contains(m))// || queued.Contains(m))
						{
							seen2++;
							continue;
						}
						//queue.Enqueue((m, steps + 1, Floors.Height(m)));
						backlog.Add((m, steps + 1, Floors.Height(m)));
						//queued.Add(m);
						// queued.Add(m>>16);
						// queued.Add(m>>24);

						// foreach (var id in m.Identities)
						// {
						// 	memo.Add(id);
						// }
						//memo.Add(m);


					}
					// Console.WriteLine();
					// Console.WriteLine();
				}


			}

			return result;

		}
	}

	internal static class Floors
	{
		private const int Levels = 4;
		// private readonly ulong _objects;
		// private readonly int _elevator;

		// public Floors(ulong objects, int elevator)
		// {
		// 	_objects = objects;
		// 	_elevator = elevator;
		// }

		private static readonly uint[][] BitsInMask;
		private static readonly int[] NumberOfSetBits;
		static Floors()
		{
			BitsInMask = new uint[1<<8][];
			NumberOfSetBits = new int[1<<8];
			for (var i = 0U; i < 1<<8; i++)
			{
				BitsInMask[i] = MathHelper.Bits(i).ToArray();
				NumberOfSetBits[i] = MathHelper.NumberOfSetBits(i);
			}
		}

		public static ulong Create(string[] input)
		{
			// The first floor contains a strontium generator, a strontium-compatible microchip, a plutonium generator, and a plutonium-compatible microchip.
			// The second floor contains a thulium generator, a ruthenium generator, a ruthenium-compatible microchip, a curium generator, and a curium-compatible microchip.
			// The third floor contains a thulium-compatible microchip.
			// The fourth floor contains nothing relevant.
			var rxGenerator = new Regex(@"\w+(?= generator)");
			var rxMicrochip = new Regex(@"\w+(?=-compatible microchip)");
			var names = rxGenerator
				.Matches(string.Join("", input))
				.Select(m => m.Value)
				.ToArray();

			var N = names.Length;
			if (N > 8)
				throw new Exception($"Max 8 names supported");

			var objects =
				DetectObjectsOnFloor(input[0]) |
				DetectObjectsOnFloor(input[1]) << 16 |
				DetectObjectsOnFloor(input[2]) << 32;

			return IdentityOf(objects, 0);

			ulong DetectObjectsOnFloor(string s)
			{
				var generators = rxGenerator.Matches(s).Select(m => m.Value).ToArray();
				var microchips = rxMicrochip.Matches(s).Select(m => m.Value).ToArray();
				ulong objects = 0;
				for (var i = 0; i < names.Length; i++)
				{
					if (generators.Contains(names[i]))
					{
						objects |= 1U << i;
					}
					if (microchips.Contains(names[i]))
					{
						objects |= 1U << (i + 8);
					}
				}
				return objects;
			}
		}

		public static bool AllMovedTo4thFloor(ulong identity) => (identity & 0xffffffffffff) == 0;

		public static ulong IdentityOf(ulong objects, int elevator) => objects | 0x8000UL << (elevator*16);

		public static (ulong, int) Decompose(ulong identity) =>
			(identity & 0x7fff7fff7fff7fff,
				(identity & 0x8000UL<<48) != 0 ? 3 :
				(identity & 0x8000UL<<32) != 0 ? 2 :
				(identity & 0x8000UL<<16) != 0 ? 1 :
				0
			);

		// public ulong Identity => IdentityOf(_objects);
		
		// public ulong[] Identities => new ulong[]
		// {
		// 	IdentityOf(_objects),
		// 	IdentityOf(_objects >> 16),
		// 	IdentityOf(_objects >> 24)
		// };
		// public static bool Identities => new ulong[]
		// {
		// 	IdentityOf(_objects),
		// 	IdentityOf(_objects >> 16),
		// 	IdentityOf(_objects >> 24)
		// };		

		public static int Height(ulong identity)
		{
			var height = 0;
			for (var i = 1; i < Levels; i++)
			{
				identity >>= 16;
				var bits = NumberOfSetBits[identity & 0xff] + NumberOfSetBits[identity>>8 & 0xff];
				height += bits * i;
			}
			return height;
		}


		public static void WriteToConsole(ulong identity)
		{
			var (objects, elevator) = Decompose(identity);
			for (var floor = Levels - 1; floor >= 0; floor--)
			{
				Console.Write($"F{floor + 1} ");
				if (floor == elevator)
					Console.Write("E ");
				else
					Console.Write(". ");
				var objectsOnFloor = ObjectsOnFloor(objects, floor);
				var generators = objectsOnFloor & 0xff;
				var microchips = objectsOnFloor >> 8;
				for (var i = 0; i < 8; i++)
				{
					if ((generators & 1<<i) != 0)
						Console.Write($"{i}G ");
					else
						Console.Write($".  ");
				}
				for (var i = 0; i < 8; i++)
				{
					if ((microchips & 1<<i) != 0)
						Console.Write($"{i}M ");
					else
						Console.Write($".  ");
				}
				Console.WriteLine();
			}
		}

		private static uint ObjectsOnFloor(ulong objects, int floor) => (uint)((objects >> (floor*16)) & 0xffff);
		private static bool HasObjectsOnFloorOrBelow(ulong objects, int floor) => ((ulong.MaxValue >> (3-floor)*16) & objects) != 0;

		public static IEnumerable<ulong> ValidMoves(ulong identity)
		{
			var (objects, elevator) = Decompose(identity);
			// if (IdentityOf(objects, elevator) != identity)
			// {
			// 	throw new Exception("Bad");
			// }

			if (elevator < Levels-1)
			{
				// return moves going up
				foreach (var floor in ValidNextMoves(elevator + 1))
				{
					yield return floor;
				}
			}
			if (elevator > 0 && HasObjectsOnFloorOrBelow(objects, elevator - 1))
			{
				// return moves going down
				foreach (var floor in ValidNextMoves(elevator - 1))
				{
					yield return floor;
				}
			}

			IEnumerable<ulong> ValidNextMoves(int dest)
			{
				var here = ObjectsOnFloor(objects, elevator);
				var there = ObjectsOnFloor(objects, dest);
				var ghere = here & 0xff;
				var mhere = here >> 8;
				var gthere = there & 0xff;

				// var gherebits = MathHelper.Bits(ghere).ToArray();
				// var mherebits = MathHelper.Bits(mhere).ToArray();
				var gherebits = BitsInMask[ghere];
				var mherebits = BitsInMask[mhere];

				//var gtherebits = MathHelper.Bits(gthere).ToArray();
				//var mthere = (ushort)(there >> 8);

				var moveables = ValidMoveables();//.Distinct(); // we get duplets
				foreach (var m in moveables)
				{
					var o = objects;
					o ^= m << (elevator * 16);
					o |=  m << (dest * 16);
					yield return IdentityOf(o, dest);
				}

				IEnumerable<ulong> ValidMoveables()
				{
					bool MicrochipCanMove(uint microchip) => (gthere == 0 || (microchip & gthere) != 0);// && MicrochipShouldMove(microchip);
					bool GeneratorCanMove(uint generator) => (generator & mhere) == 0 || (generator & ghere) == generator;

					bool MicrochipShouldMove(uint microchip) => dest > elevator || (microchip & ghere) == 0;

					// Move 1 or 2 microchips
					foreach (var m1 in mherebits)
					{
						// Microchips can move to an empty floor or a floor where protected by its generator
						if (MicrochipCanMove(m1))
						{
							yield return m1 << 8;
						}
						foreach (uint m2 in mherebits)//.Where(x => x != m1))
						{
							if (MicrochipCanMove(m1) && MicrochipCanMove(m2))
							{
								yield return (m1 | m2) << 8;
							}
						}
					}
					// Move 1 or 2 generators
					foreach (uint g1 in gherebits)
					{
						// Generators can move unless protecting an otherwise threatened microchip
						if (GeneratorCanMove(g1))
						{
							yield return g1;
						}
						foreach (uint g2 in gherebits)//.Where(x => x != g1))
						{
							if (GeneratorCanMove(g1) && GeneratorCanMove(g2))
							{
								yield return g1 | g2;
							}
						}
					}
					// Move 1 microchip and 1 generator
					foreach (uint m in mherebits)
					{
						foreach (uint g in gherebits)
						{
							// Combo can move if both are allowed
							if (MicrochipCanMove(m) && GeneratorCanMove(g))
							{
								yield return g | (m << 8);
							}
						}						
					}					
				}
			}
		}
	}


}
