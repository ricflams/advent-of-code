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
using System.ComponentModel.DataAnnotations;

namespace AdventOfCode.Y2025.Day10.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2025;
		public override int Day => 10;

		public override void Run()
		{
			Run("test1").Part1(7).Part2(33);
			//Run("test2").Part1(0).Part2(0);

			// 129783 too high
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

			var sum = 0;
			var count = 0;
			foreach (var m in machines)//.Skip(112))
			{
				var buttons = m.ButtonNumbers;
				var joltages = m.Joltages;
				var orgJoltages = m.Joltages.ToArray();
				var org = new int[buttons.Length, joltages.Length];
				var matrice = new int[buttons.Length, joltages.Length];
				for (var bi = 0; bi < buttons.Length; bi++)
                {
                    for (var ji = 0; ji < joltages.Length; ji++)
					{
						org[bi, ji] = buttons[bi].Contains(ji) ? 1 : 0;
						matrice[bi, ji] = buttons[bi].Contains(ji) ? 1 : 0;
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
				Console.WriteLine($"Machine {count++}");
                Print(matrice, joltages);
				Normalize(matrice, joltages);
				Console.WriteLine("to");
				Print(matrice, joltages);

				var bounds = FindFreeVariablesUpperBounds(matrice, joltages);
				for (var x = 0; x < matrice.Width(); x++)
				{
					Console.Write($"{bounds[x], 4}");
				}
				Console.WriteLine();

				var clicks = MinimumSumOfClicks(matrice, joltages, bounds);
				foreach (var r in _theResult)
				{
					Console.Write($"{r,4}");
				}
				Console.WriteLine();
				Console.WriteLine($"clicks = {clicks}");

				if (!IsSolved(org, _theResult, orgJoltages))
				{
					throw new Exception();
				}

				sum += clicks;

			}

			/// (0,1,2,3,4) (1,2,4) (1,2,3) {149,166,166,162,153}
			///  
			///      0   1   2   3   4
			/// a *  1   1   1   1   1
			/// b *  0   1   1   0   1
			/// c *  0   1   1   1   0
			/// =   149 166 166 162 153
			///

			return sum;
		}

		static private int? FirstNonzeroColumn(int[,] m, int row)
		{
			var w = m.Width();
			for (var x = 0; x < w; x++)
				if (m[x, row] != 0)
					return x;
			return null;
		}

		static private int[] FindFreeVariablesUpperBounds(int[,] m, int[] result)
		{
			var (w, h) = (m.Width(), m.Height());

			var free = Enumerable.Repeat(true, w).ToArray();
			for (var row = 0; row < h; row++)
			{
				var col = FirstNonzeroColumn(m, row);
				if (col is {} c2)
					free[c2] = false;
			}

			var bounds = new int[w];
			for (var x = 0; x < w; x++)
			{
				if (!free[x])
					continue;
				//var lowest = 0;
				//for (var y = 0; y < h; y++)
				//{
				//	var c = m[x, y];
				//	if (c <= 0)
				//		continue;

				//	//if (result[y] % c != 0)
				//	//	continue;

				//	var bound = result[y] / c;
				//	if (lowest == 0 || bound < lowest)
				//		lowest = bound;
				//}

				//if (lowest == 0)
				//	throw new Exception("can't happen");

				//bounds[x] = lowest;

				bounds[x] = 200;

			}

			return bounds;
		}

		public static int MinimumSumOfClicks(int[,] m, int[] result, int[] bounds)
		{
			var (w, h) = (m.Width(), m.Height());

			_theResult = null;
			var min = MinimumSumOfClicks(m, result, bounds, new int[w], w - 1, h - 1);

			if (min.Value == 0)
				throw new Exception();

			return min.Value;
		}

		private static int[] _theResult;

		public static int? MinimumSumOfClicks(int[,] m, int[] result, int[] bounds, int[] inputs, int col, int row)
		{
			var (w, h) = (m.Width(), m.Height());

			//Console.WriteLine($"{string.Join(" ", inputs.Select(i => i.ToString()))} col={col} row={row}");

			if (col < 0 || row < 0)
			{
				//Console.Write($"[{inputs.Sum()}]");
				//Console.WriteLine($"{string.Join(" ", inputs.Select(i => i.ToString()))} col={col} row={row}");

				var isSolved = IsSolved(m, inputs, result);
                if (!isSolved)
                {
					throw new Exception();
                }

				var sum = inputs.Sum();
				if (_theResult == null || sum < _theResult.Sum())
					_theResult = inputs.ToArray();

				return sum;
			}

			if (bounds[col] == 0)
			{
				// Calculate result[col] straight up
				var sum = 0;
				for (var x = col + 1; x < w; x++)
				{
					sum += m[x, row] * inputs[x];
				}

				if (m[col, row] == 0 && sum == 0 && result[row] == 0)
				{
					return MinimumSumOfClicks(m, result, bounds, inputs, col, row - 1);
				}


				sum = result[row] - sum;

				if (sum < 0)
				{
					//Console.Write($"-");
					return null;
				}

				if (sum == 0)
				{
					inputs[col] = 0;
				}
				else if (m[col, row] == 0)
				{
					//Console.Write($"x");
					return null;
				}
				else
				{
					if (sum % m[col, row] != 0)
					{
						//Console.Write($"?");
						return null; // unsolvable: no integer can produce the result
					}

					var c = sum / m[col, row];
					inputs[col] = c;
				}

				var c2 = MinimumSumOfClicks(m, result, bounds, inputs, col - 1, row - 1);
				if (!c2.HasValue)
				{
					//Console.Write($"/");
					return null;
				}
				return c2.Value;
			}

			// Iterate all options, finding the smallest sum
			int? smallest = null;
			for (var t = 0; t < bounds[col]; t++)
			{
				inputs[col] = t;
				var c2 = MinimumSumOfClicks(m, result, bounds, inputs, col - 1, row);
				if (c2 is { } c3)
				{
					if (smallest == null || c3 < smallest)
						smallest = c3;
				}
			}
			return smallest;
		}


		static private bool IsSolved(int[,] m, int[] input, int[] output)
		{
			var (w, h) = (m.Width(), m.Height());

			for (var y = 0; y < h; y++)
			{
				var sum = 0;
				for (var x = 0; x < w; x++)
				{
					sum += m[x, y] * input[x];
				}
				if (sum != output[y])
					return false;
			}
			return true;
		}

		void Normalize(int[,] m, int[] result)
		{
            var (w, h) = (m.Width(), m.Height());

			var row = 0;
			for (var col = 0; col < w && row < h; col++)
			{

				var bestPivot = -1;
				for (var y = row; y < h; y++)
				{
					if (m[col, y] < 0)
					{
						for (var xx = col; xx < w; xx++)
						{
							m[xx, y] *= -1;
						}
						result[y] *= -1;

						//Console.WriteLine($"x={x}: Negated row {y}:");
						//Print(m, result);
					}

					if (m[col, y] > 0 && (bestPivot < 0 || m[col, y] < m[col, bestPivot]))
					{
						bestPivot = y;
					}
				}

				if (bestPivot < 0)
				{
					//Console.WriteLine($"x={x}: #########");
					continue;
				}

				if (bestPivot > row)
				{
					// swap rows i,j
					for (var xx = col; xx < w; xx++)
						(m[xx, row], m[xx, bestPivot]) = (m[xx, bestPivot], m[xx, row]);
					(result[row], result[bestPivot]) = (result[bestPivot], result[row]);

					//Console.WriteLine($"x={x}: Swapped best pivot row {bestPivot} and {x}:");
					//Print(m, result);
				}

				var coeff = m[col, row];

				//if (coeff > 1)
				//{
				//	m[col, row] /= coeff;
				//	result[row] /= coeff;
				//}

				for (var yy = 0; yy < h; yy++)
				{
					if (yy == row)
						continue;
					var sub = m[col, yy];
					if (sub == 0)
						continue;
					for (var xx = 0; xx < w; xx++)
					{
						//var old = m[xx, yy];
						m[xx, yy] = m[xx, yy] * coeff - m[xx, row] * sub;
						//if (xx < col && m[xx, yy] != old)
						//	Console.WriteLine($"################# {old} => {m[xx, yy]} at col={col} row={yy}");
					}
					result[yy] = result[yy] * coeff - result[row] * sub;

					//Console.WriteLine($"x={x}: Subtracted row {x} from row {yy}:");
     //               Print(m, result);
                }

				var gcd = new List<int>();
				for (var x = 0; x < w; x++)
				{
					if (m[x, row] != 0)
						gcd.Add(m[x, row]);
				}
				if (result[row] != 0)
					gcd.Add(result[row]);
				if (gcd.Any())
				{
					var d = (int)MathHelper.GreatestCommonFactor(gcd.Select(x => (long)x).ToArray());
					if (d > 1)
					{
						for (var x = 0; x < w; x++)
						{
							m[x, row] /= d;
						}
						result[row] /= d;
					}
				}


				row++;
			}
		}

		void Print(int[,] m, int[] result)
		{
            var (w, h) = (m.Width(), m.Height());
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    Console.Write($"{m[x, y],4}");
                }
                Console.WriteLine($" = {result[y],4}");
            }
            Console.WriteLine();
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
