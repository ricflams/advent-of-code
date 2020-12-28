using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day23
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Crab Cups";
		protected override int Year => 2020;
		protected override int Day => 23;

		public void Run()
		{
			RunFor("test1", 67384529, 149245887792);
			RunFor("input", 34952786, 505334281774);
		}

		protected override long Part1(string[] input)
		{
			var cuparr = ShuffleCups(input[0], 100, input[0].Length);

			var n = 0;
			var digit = cuparr[0];
			for (var i = 0; i < cuparr.Length - 1; i++)
			{
				n = n * 10 + digit + 1; // +1 to go back from 0-based to 1-based
				digit = cuparr[digit];
			}

			return n;
		}

		protected override long Part2(string[] input)
		{
			var cuparr = ShuffleCups(input[0], 10_000_000, 1_000_000);
			var cup1 = cuparr[0];
			var cup2 = cuparr[cup1];
			var result = (long)(cup1 + 1) * (cup2 + 1); // +1 to go back fmom 0-based to 1-based
			return result;
		}

		private static int[] ShuffleCups(string input, int rounds, int N)
		{
			// Convert 1-9 into 0-8 to keep all calculations 0-based; easier for modulus
			var labels = input.Select(c => c - '1').ToArray();

			// Init with labels from input and start position from first label 0
			var cups = new int[N];
			var pos = labels[0];
			for (var i = 0; i < labels.Length - 1; i++)
			{
				cups[labels[i]] = labels[i + 1];
			}
			cups[labels.Last()] = pos;

			// Add padding, if any (part 2)
			if (N > labels.Length)
			{
				cups[labels.Last()] = labels.Length;
				for (var i = labels.Length; i < N; i++)
				{
					cups[i] = i + 1;
				}
				cups[N - 1] = pos;
			}

			// Loop
			for (var i = 0; i < rounds; i++)
			{
				var next1 = cups[pos];
				var next2 = cups[next1];
				var next3 = cups[next2];
				var dest = (pos - 1 + N) % N;
				while (dest == next1 || dest == next2 || dest == next3)
				{
					dest = (dest - 1 + N) % N;
				}
				cups[pos] = cups[next3];
				cups[next3] = cups[dest];
				cups[dest] = next1;
				pos = cups[pos];
			}
			return cups;
		}

		#region Originals
		private static long Part1Original(string[] input)
		{
			var cups = input[0].Select(c => c - '0').ToList();
			var N = cups.Count();

			var curpos = 0;
			for (var i = 0; i < 100; i++)
			{
				var cup = cups[curpos];
				var pickuppos1 = (curpos + 1) % N;
				var pickuppos2 = (curpos + 2) % N;
				var pickuppos3 = (curpos + 3) % N;
				var pickup1 = cups[pickuppos1];
				var pickup2 = cups[pickuppos2];
				var pickup3 = cups[pickuppos3];

				cups[pickuppos1] = cups[pickuppos2] = cups[pickuppos3] = -1;
				cups = cups.Where(x => x != -1).ToList();

				var lowest = cups.Min();
				var highest = cups.Max();

				var dest = cup - 1;
				if (dest < lowest)
					dest = highest;
				while (dest == pickup1 || dest == pickup2 || dest == pickup3)
				{
					dest = dest - 1;
					if (dest < lowest)
						dest = highest;
				}

				var destpos = cups.IndexOf(dest);
				cups.Insert(destpos + 1, pickup3);
				cups.Insert(destpos + 1, pickup2);
				cups.Insert(destpos + 1, pickup1);

				curpos = (cups.IndexOf(cup) + 1) % N;
			}


			var resultstring = "";
			var pos = cups.IndexOf(1);
			var n = cups.Count;
			for (var i = 1; i < n; i++)
			{
				pos = (pos + 1) % n;
				resultstring += cups[pos];
			}
			var result = long.Parse(resultstring);

			return result;
		}


		internal class Cup
		{
			public int Label { get; set; }
		}

		private static long Part2Original(string[] input)
		{
			var initialLabels = input[0].Select(c => c - '0');

			var rounds = 10_000_000;
			var N = 1_000_000;
			var labels = initialLabels.Concat(Enumerable.Range(initialLabels.Count() + 1, N - initialLabels.Count()));
			var cupByLabel = new Dictionary<int, LinkedListNode<Cup>>();
			var cups = new LinkedList<Cup>();
			foreach (var label in labels)
			{
				var c = new Cup { Label = label };
				cupByLabel[label] = cups.AddLast(c);
			}

			var cup = cups.First;
			for (var i = 0; i < rounds; i++)
			{
				var pick1 = cup.Next ?? cups.First;
				var pick2 = pick1.Next ?? cups.First;
				var pick3 = pick2.Next ?? cups.First;
				cups.Remove(pick1);
				cups.Remove(pick2);
				cups.Remove(pick3);

				var destLabel = cup.Value.Label - 1;
				if (destLabel < 1)
					destLabel = N;
				while (destLabel == pick1.Value.Label || destLabel == pick2.Value.Label || destLabel == pick3.Value.Label)
				{
					destLabel--;
					if (destLabel < 1)
						destLabel = N;
				}
				var dest = cupByLabel[destLabel];

				cups.AddAfter(dest, pick1);
				cups.AddAfter(pick1, pick2);
				cups.AddAfter(pick2, pick3);

				cup = cup.Next ?? cups.First;
			}

			var cup1 = cupByLabel[1];
			var cupN1 = cup1.Next ?? cups.First;
			var cupN2 = cupN1.Next ?? cups.First;
			var label1 = cupN1.Value.Label;
			var label2 = cupN2.Value.Label;
			var result2 = (long)label1 * label2;
			//Console.Write($"cupN1={label1} cupN2={label2} prod={(long)label1 * label2}");

			return result2;
		}
		#endregion
	}
}
