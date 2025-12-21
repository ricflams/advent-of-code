using System;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2025.Day10
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Factory";
		public override int Year => 2025;
		public override int Day => 10;

		public override void Run()
		{
			Run("test1").Part1(7).Part2(33);
			Run("input").Part1(449).Part2(17848);
			Run("extra").Part1(558).Part2(20317);
		}

		protected override long Part1(string[] input)
		{
			var machines = input.Select(Machine.ParseFrom).ToArray();

			var sum = machines.Sum(MinimumButtonPressesForLights);

			return sum;
		}

		private static int MinimumButtonPressesForLights(Machine m)
		{
			// Walk all combinations of on/off for each button, recording the minimum pushes
			var min = int.MaxValue;
			FewestPushesForLights(0, 0, 0u);
			return min;

			void FewestPushesForLights(int pos, int pushes, uint lights)
			{
				if (pushes >= min)
					return;
				if (pos == m.Buttons.Length)
				{
					if (lights == m.Goal)
					{
						min = pushes;
					}
					return;
				}
				FewestPushesForLights(pos + 1, pushes, lights);
				FewestPushesForLights(pos + 1, pushes + 1, lights ^ m.Buttons[pos]);
			}
		}

		protected override long Part2(string[] input)
		{
			var machines = input.Select(Machine.ParseFrom).ToArray();

			var sum = 0;
			foreach (var m in machines)
			{
				var buttons = m.ButtonNumbers;
				var results = m.Joltages;
				var matrix = new int[buttons.Length, results.Length];
				for (var bi = 0; bi < buttons.Length; bi++)
                {
                    for (var ji = 0; ji < results.Length; ji++)
					{
						matrix[bi, ji] = buttons[bi].Contains(ji) ? 1 : 0;
					}
				}

				var equations = new LinearEquations<int>(matrix, results);
				var normalized = equations.GetNormalized();
				var freeVariables = normalized.GetFreeVariables();
				var bounds = equations.GetFreeVariablesUpperBounds(freeVariables);
				var clicks = MinimumButtonPressesForJoltages(normalized, bounds);
				sum += clicks;
			}

			return sum;
		}

		private static int MinimumButtonPressesForJoltages(LinearEquations<int> eq, int[] bounds)
		{
			var (m, result) = (eq.Matrix, eq.Result);
			var (w, h) = (m.Width(), m.Height());
			var inputs = new int[w];
			return MinimumSumOfClicks(w - 1, h - 1).Value;

			int? MinimumSumOfClicks(int col, int row)
			{
				if (col < 0)
				{
					Debug.Assert(row < 0);
					Debug.Assert(eq.IsSolvedFor(inputs));
					return inputs.Sum();
				}

				// No bounds means we can cauculate the input-value directly, if it
				// can be calculated at all, that is; if not then this is  a dead end.
				if (bounds[col] == 0)
				{
					// Calculate result[col] straight up
					var sumToTheRight = 0;
					for (var x = col + 1; x < w; x++)
					{
						sumToTheRight += m[x, row] * inputs[x];
					}

					// All are zeros: no info here so move up to the next row
					if (m[col, row] == 0 && sumToTheRight == 0)
					{
						Debug.Assert(result[row] == 0);
						return MinimumSumOfClicks(col, row - 1);
					}

					var target = result[row] - sumToTheRight;

					// If equation can only be solved by a negative number then it can't be
					// solved and the factors that has lead us here are not good candidates.
					if (target < 0)
					{
						return null;
					}

					// The input-value must be an integer so the target must be divisible by
					// the factor at this place; otherwise this equation is unsolveable and we
					// should abort this attempt.
					if (target % m[col, row] != 0)
					{
						return null; 
					}

					// So far so good; the target can be reached by setting this input
					inputs[col] = target / m[col, row];

					return MinimumSumOfClicks(col - 1, row - 1);
				}

				// Iterate all possible values for this input up to and including the bounds,
				// and find the smallest sum
				int? smallest = null;
				for (var trial = 0; trial <= bounds[col]; trial++)
				{
					inputs[col] = trial;
					var clicks = MinimumSumOfClicks(col - 1, row);
					if (clicks is {} value)
					{
						if (smallest == null || value < smallest)
							smallest = value;
					}
				}
				return smallest;
			}
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
	}
}
