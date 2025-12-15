using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Numerics;
using System.Runtime.Serialization;

namespace AdventOfCode.Y2025.Day10
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2025;
		public override int Day => 10;

		public override void Run()
		{
//			Run("test1").Part1(7).Part2(33);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(449).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		private record Machine(uint Goal, uint[] Buttons, int[][] ButtonNumbers, int[] Joltages)
		{
			public static Machine ParseFrom(string line)
			{
				var (a, b, c) = line.RxMatch("[%*] %* {%*}").Get<string, string, string>();
				var goal = ToBits(a.ToCharArray().Select(c => c == '#').ToArray());
				var buttons = b.SplitSpace().Select(x => x[1..^1].SplitByComma().Select(int.Parse).Select(bit => 1u << bit).Select(x => (int)x).Sum()).Select(x => (uint)x).ToArray();
				var buttonNumbers = b.SplitSpace().Select(x => x[1..^1].SplitByComma().Select(int.Parse).ToArray()).ToArray();
				var joltages = c.SplitByComma().Select(int.Parse).ToArray();
				return new Machine(goal, buttons, buttonNumbers, joltages);
			}

			private static uint ToBits(bool[] goal)
			{
				var bits = 0u;
				for (var i = 0; i < goal.Length; i++)
				{
					if (goal[i])
						bits += (1u << i);
				}
				return bits;
			}
		}

		protected override long Part1(string[] input)
		{
			var machines = input.Select(Machine.ParseFrom).ToArray();

			var sum = machines.Sum(FewestPushesForLights);

			return sum;
		}

		private static int FewestPushesForLights(Machine m)
		{
			var seen = new Dictionary<uint, int>();

			FewestPushes(0, 0);
			return seen[m.Goal];

			void FewestPushes(uint lights, int pushes)
			{
				//Console.WriteLine($"Examine lights={Convert.ToString(lights, 2)} pushes={pushes}");
				if (seen.TryGetValue(lights, out var n) && n <= pushes)
					return;
				seen[lights] = pushes;
				if (lights == m.Goal)
				{
					//Console.WriteLine("bingo");
					return;
				}

				foreach (var b in m.Buttons)
				{
					FewestPushes(lights ^ b, pushes + 1);
				}
			}
		}



		protected override long Part2(string[] input)
		{
			var machines = input.Select(Machine.ParseFrom).ToArray();

			foreach (var m in machines)
			{
				var buttons = m.ButtonNumbers;
				var joltages = m.Joltages;
				var matrice = new int[joltages.Length, buttons.Length];
				for (var ji = 0; ji < joltages.Length; ji++)
				{
					for (var bi = 0; bi < buttons.Length; bi++)
					{
						matrice[ji, bi] = buttons[bi].Contains(ji) ? 1 : 0;
					}
				}

				//var (w, h) = (matrice.Width(), matrice.Height());
				//for (var y = 0; y < h; y++)
				//{
				//	for (var x = 0; x < w; x++)
				//	{
				//		Console.Write($"{matrice[x, y],4}");
				//	}
				//	Console.WriteLine();
				//}
				//Console.WriteLine("--------------");
				//foreach (var j in joltages)
				//{
				//	Console.Write($"{j,4}");
				//}

				Console.WriteLine();
				Console.WriteLine();
				var (w, h) = (matrice.Width(), matrice.Height());
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						Console.Write($"{matrice[x, y],4}");
					}
					Console.WriteLine($" = {joltages[x],4}");
				}
				Console.WriteLine();
			}

			/// (0,1,2,3,4) (1,2,4) (1,2,3) {149,166,166,162,153}
			///  
			///      0   1   2   3   4
			/// a *  1   1   1   1   1
			/// b *  0   1   1   0   1
			/// c *  0   1   1   1   0
			/// =   149 166 166 162 153
			///


			var sum = machines.Sum(FewestPushesForJoltages);

			return sum;
		}


		private static int FewestPushesForJoltages(Machine m)
		{
			var seen = new Dictionary<string, int>();

			var contributesTo = Enumerable.Range(0, m.Joltages.Length)
				.Select(bn => m.ButtonNumbers.Select((bns, i) => bns.Contains(bn) ? i : -1).Where(x => x != -1).ToArray())
				.ToArray();

			var zero = Enumerable.Repeat(0, m.Joltages.Length).ToArray();
			var goalId = JoltageId(m.Joltages);
			FewestPushes(zero, 0);
			return seen[goalId];

			static string JoltageId(int[] joltages) => string.Join(',', joltages);

			void FewestPushes(int[] joltages, int pushes)
			{
				//Console.WriteLine($"Examine lights={Convert.ToString(lights, 2)} pushes={pushes}");
				var id = JoltageId(joltages);
				if (seen.TryGetValue(id, out var n) && n <= pushes)
					return;
				seen[id] = pushes;
				if (id == goalId)
				{
					Console.WriteLine("bingo");
					return;
				}

				//for (var i = 0; i < m.Joltages.Length; i++)
				//{
				//	var remains = m.Joltages[i] - joltages[i];
				//	var contributors = contributesTo[i];
				//	for (var j = 0; j < m.Joltages.Length; j++)
				//	{
				//		// They all contribute to these joltages
				//		if (contributors.All(c => m.ButtonNumbers[c].Contains(j)))
				//		{
				//			var remainsForOther = m.Joltages[j] - joltages[j];
				//			if (remainsForOther > remains)
				//			{
				//				Console.Write('X');
				//				return;
				//			}
				//		}
				//	}

				//}
				//foreach (var bn in m.ButtonNumbers)
				//{

				//}

				//var allButtons = m.ButtonNumbers.SelectMany(bns => bns).Distinct().ToArray();
				//var allContributeTo = allButtons
				//	.Where(b => m.ButtonNumbers.All(bn => bn.Contains(b)))
				//	.ToArray();
				//if (allContributeTo.Length > 1)
				//{
				//	//Console.Write('.');
				//	var remaining = allContributeTo.Select(b => m.Joltages[b] - joltages[b]).ToArray();
				//	var isSimilar = remaining.Distinct().Count() == 1;
				//	if (!isSimilar)
				//	{
				//		//Console.Write('X');
				//		return;
				//	}
				//}

				//var bns = buttonNumbers
				//	.Where(bn => !bn.Any(b => joltages[b] == m.Joltages[b]))
				//	.ToArray();



				//var allContributeTo = bns.Where(bn => allButtons.All(b => bn.Contains(b))).ToArray();

				//foreach (var bn in bns)
				//{
				//	foreach (var b in bn)
				//	{
				//		var missing = m.Joltages[b] - joltages[b];
				//		var contributors = bns.Where(bn => bn.Contains(b)).ToArray();

				//		if (contributors.Length == bns.Length)
				//		{
				//			foreach (var bn2 in bns)
				//			{

				//			}
				//			Console.Write('.');
				//		}
				//	}
				//}



				foreach (var bn in m.ButtonNumbers)
				{
					var toobig = false;
					var nextJoltages = new int[m.Joltages.Length];
					for (var i = 0; i < m.Joltages.Length; i++)
					{
						nextJoltages[i] = joltages[i];
						if (bn.Contains(i))
						{
							nextJoltages[i]++;
							if (nextJoltages[i] > m.Joltages[i])
								toobig = true;
						}
					}
					if (toobig)
						continue;
					FewestPushes(nextJoltages, pushes + 1);
				}
			}
		}




	}
}
