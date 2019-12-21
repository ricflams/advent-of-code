using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;

namespace AdventOfCode2019
{
	internal class Program
	{
		private static void Main()
		{
			Exercise(() =>
			{
				//Day1();
				//Day2();
				//Day3();
				//Day4();
				//Day5();
				//Day6();
				//Day7();
				//Day8();
				//Day9();
				//Day10();
				//Day11.Puzzle.Run();
				//Day12.Puzzle.Run();
				//Day13.Puzzle.Run();
				//Day14.Puzzle.Run();
				// Day15.Puzzle.Run();
				//Day16.Puzzle.Run();
				//Day17.Puzzle.Run();
				//Day18.Puzzle.Run();
				//Day19.Puzzle.Run();
				Day20.Puzzle.Run();
			});
			Console.Write("Done - press any key");
			Console.ReadKey();
		}

		private static void Exercise(Action action)
		{
			action();
#if false
			var iterations = 20;
			var sw = System.Diagnostics.Stopwatch.StartNew();
			for (var i = 0; i < iterations; i++)
			{
				action();
			}
			Console.WriteLine($"Elapsed: {(int)(sw.ElapsedMilliseconds/ iterations)} ms");
#endif
		}

		private static void Day1()
		{
			var masses = new[] { 116115, 58728, 102094, 104856, 86377, 97920, 101639, 95328, 103730, 57027, 83080, 57748, 101606, 54629, 90901, 59983, 109795, 123270, 141948, 92969, 149805, 143555, 141387, 136357, 90236, 63577, 127108, 130012, 88223, 51426, 117663, 63924, 56251, 108505, 89625, 126994, 120237, 99351, 136948, 123702, 129849, 93541, 110900, 63759, 58537, 132943, 118213, 104274, 84606, 125256, 76355, 116711, 79344, 66355, 117654, 116026, 80244, 129786, 73054, 119806, 90941, 53877, 96707, 58226, 101666, 53819, 54558, 77342, 149653, 87843, 54388, 128862, 55752, 89962, 147224, 118486, 56910, 124854, 57052, 55495, 62530, 128104, 68788, 60915, 62155, 123614, 115522, 116920, 101263, 92339, 92234, 81542, 78062, 137207, 92082, 120032, 136537, 109035, 115819, 75955 };

			var mass1 = masses.Sum(FuelForMass);
			Console.WriteLine($"Day  1 Puzzle 1: {mass1}");
			Debug.Assert(mass1 == 3228475);

			var mass2 = masses.Sum(x =>
			{
				var totalFuel = FuelForMass(x);
				var fuelMass = totalFuel;
				while (true)
				{
					var extraFuel = FuelForMass(fuelMass);
					if (extraFuel <= 0)
						break;
					totalFuel += extraFuel;
					fuelMass = extraFuel;
				}
				return totalFuel;
			});
			Console.WriteLine($"Day  1 Puzzle 2: {mass2}");
			Debug.Assert(mass2 == 4839845);

			int FuelForMass(int mass) => mass / 3 - 2;
		}

		private static void Day2()
		{
			var mem = new[] { 1, 0, 0, 3, 1, 1, 2, 3, 1, 3, 4, 3, 1, 5, 0, 3, 2, 13, 1, 19, 1, 19, 10, 23, 1, 23, 6, 27, 1, 6, 27, 31, 1, 13, 31, 35, 1, 13, 35, 39, 1, 39, 13, 43, 2, 43, 9, 47, 2, 6, 47, 51, 1, 51, 9, 55, 1, 55, 9, 59, 1, 59, 6, 63, 1, 9, 63, 67, 2, 67, 10, 71, 2, 71, 13, 75, 1, 10, 75, 79, 2, 10, 79, 83, 1, 83, 6, 87, 2, 87, 10, 91, 1, 91, 6, 95, 1, 95, 13, 99, 1, 99, 13, 103, 2, 103, 9, 107, 2, 107, 10, 111, 1, 5, 111, 115, 2, 115, 9, 119, 1, 5, 119, 123, 1, 123, 9, 127, 1, 127, 2, 131, 1, 5, 131, 0, 99, 2, 0, 14, 0 };
			mem[1] = 12;
			mem[2] = 2;

			var engine = new Intcode.Engine();
			var result1 = engine
				.WithMemory(mem)
				.Execute()
				.Memory[0];
			Console.WriteLine($"Day  2 Puzzle 1: mem[0] = {result1}");
			Debug.Assert(result1 == 5866714);

			for (var op1 = 0; op1 < 100; op1++)
			{
				for (var op2 = 0; op2 < 100; op2++)
				{
					mem[1] = op1;
					mem[2] = op2;
					var output = engine
						.WithMemory(mem)
						.Execute()
						.Memory[0];
					if (output == 19690720)
					{
						var result2 = op1 * 100 + op2;
						Console.WriteLine($"Day  2 Puzzle 2: {result2}");
						Debug.Assert(result2 == 5208);
						break;
					}
				}
			}
		}

		private static void Day3()
		{
			var map = new Dictionary<int, int[]>();
			var wiredefs = new string[]
			{
				"R1000,D722,L887,D371,R430,D952,R168,D541,L972,D594,R377,U890,R544,U505,L629,U839,L680,D863,L315,D10,L482,U874,L291,U100,R770,D717,L749,U776,L869,D155,R250,U672,L195,D991,L556,D925,R358,U296,R647,D652,L790,D780,L865,U405,L400,D160,L460,U50,R515,D666,R306,U746,R754,U854,L332,U254,R673,U795,R560,U69,L507,U332,L328,U547,L717,U291,R626,U868,L583,D256,R371,U462,R793,U559,L571,U270,R738,U425,L231,U549,L465,U21,L647,U43,R847,U104,L699,U378,L549,D975,R13,D306,R532,D730,L566,U846,L903,D224,R448,D424,L727,D199,L626,D872,L541,D786,L304,U462,R347,U379,R29,D556,L775,D768,L284,D480,R654,D659,R818,D57,L77,U140,R619,D148,R686,D461,L910,U244,R115,D769,R968,U802,L737,U868,R399,D150,L791,U579,L856,D11,R115,U522,L443,D575,L133,U750,R437,U718,L79,D119,L97,U471,R817,U438,R157,U105,L219,U777,L965,U687,L906,D744,L983,D350,R664,D917,R431,D721,L153,U757,L665,U526,L49,U166,L59,D293,R962,D764,R538,U519,L24,U91,R11,U574,L647,U891,R44,D897,L715,U498,L624,D573,R287,U762,L613,D79,R122,U148,L849,D385,R792,D20,L512,D431,R818,U428,L10,D800,R773,D936,L594,D38,R824,D216,L220,U358,L463,U550,R968,D346,L658,U113,R813,U411,L730,D84,R479,U877,L730,D961,L839,D792,R424,U321,L105,D862,L815,D243,L521,D913,L1,D513,L269,U495,L27,U16,R904,D926,R640,U948,R346,D240,L273,U131,L296,U556,R347,D640,L261,D43,R136,U824,R126,U583,R736,U530,L734,U717,L256,U362,L86,U48,R851,U519,L610,D134,L554,D766,L179,U637,R71,D895,L21,D908,R486,D863,R31,U85,R420,D718,R466,D861,R655,D304,L701,D403,L860,D208,L595,U64,R999",
				"L992,D463,R10,D791,R312,D146,R865,D244,L364,D189,R35,U328,R857,D683,L660,D707,L908,D277,R356,U369,R197,D35,R625,D862,L769,U705,L728,U999,R938,U233,L595,U266,L697,U966,L536,D543,L669,D829,R910,U693,R753,D389,L647,U603,L660,D787,L138,D119,L131,D266,R268,D917,R776,U290,R920,U904,L46,D139,L341,D19,R982,U790,L981,U791,L147,U30,L246,U677,R343,D492,R398,D234,R76,D423,L709,D392,R741,U408,R878,U29,R446,U36,R806,U78,L76,D813,R584,U682,L187,U666,L340,D301,L694,U15,R800,U276,L755,U558,R366,D309,R571,U976,L286,D833,R318,U365,L864,U408,L352,D61,R284,D272,R240,D845,L206,U721,R367,D541,R628,U581,L750,D680,R695,D30,R849,U743,L214,U605,R533,U493,R803,D783,R168,U877,L61,D726,L794,D116,R717,U44,R964,U674,L291,D372,L381,D523,L644,U438,R983,D390,R520,D471,R556,D693,L919,D863,R84,D629,L264,D429,R82,U64,R835,D801,R93,U770,R441,D152,L718,D788,L797,U699,L82,U206,L40,U952,R902,U570,L759,D655,L131,D901,L470,D804,L407,U458,L922,D21,L171,U841,L237,D301,R192,D293,R984,U243,R846,U139,L413,U162,R925,D235,L115,U443,L884,D910,R335,U274,L338,U160,R125,D775,R824,D821,R531,D761,L189,U822,L602,D732,R473,U149,L128,U30,R77,D957,R811,D154,L988,D237,R425,D855,R482,D571,R134,D731,L905,U869,R916,D689,L17,U24,R353,D996,R832,U855,L76,U659,R581,D483,R821,D145,R199,D344,R487,D436,L92,U491,R365,D909,L17,D148,R307,U57,R666,U660,R195,D767,R612,D902,L594,D299,R670,D881,L583,D793,R58,U89,L99,D355,R394,D350,R920,U544,R887,U564,L238,U979,L565,D914,L95,U150,R292,U495,R506,U475,R813,D308,L797,D484,R9"
			};
			for (var i = 0; i < wiredefs.Length; i++)
			{
				MapWire(i, wiredefs.Length, wiredefs[i]);
			}

			var crossings = map
				.Where(x => x.Value.All(s => s != 0))
				.Select(x => new
				{
					X = MakeX(x.Key),
					Y = MakeY(x.Key),
					Steps = x.Value.Sum()
				})
				.ToList();

			var nearest = crossings.Min(x => Math.Abs(x.X) + Math.Abs(x.Y));
			Console.WriteLine($"Day  3 Puzzle 1: {nearest}");
			Debug.Assert(nearest == 860);

			var fewestSteps = crossings.Min(x => x.Steps);
			Console.WriteLine($"Day  3 Puzzle 2: {fewestSteps}");
			Debug.Assert(fewestSteps == 9238);

			void MapWire(int wireIndex, int wireCount, string wiredef)
			{
				int x = 0, y = 0, step = 0;
				foreach (var wire in wiredef.Split(','))
				{
					int dx = 0, dy = 0;
					var len = int.Parse(wire.Substring(1));
					switch (wire[0])
					{
						case 'R': dx = 1; dy = 0; break;
						case 'D': dx = 0; dy = -1; break;
						case 'L': dx = -1; dy = 0; break;
						case 'U': dx = 0; dy = 1; break;
					}
					for (var i = 0; i < len; i++)
					{
						step++;
						x += dx;
						y += dy;
						var xy = MakeXy(x, y);
						if (!map.ContainsKey(xy))
						{
							map[xy] = new int[wireCount];
						}
						if (map[xy][wireIndex] == 0)
						{
							map[xy][wireIndex] = step;
						}
					}
				}
			}

			const int xyFactor = 100000;
			int MakeXy(int x, int y) => x * xyFactor + y;
			int MakeX(int xy) => xy / xyFactor;
			int MakeY(int xy) => xy % xyFactor;
		}

		private static void Day4()
		{
			var matches1 = CalcMatches(382345, 843167).Count(v => SequenceLengths(v).Any(seq => seq >= 2));
			Console.WriteLine($"Day  4 Puzzle 1: {matches1}");
			Debug.Assert(matches1 == 460);

			var matches2 = CalcMatches(382345, 843167).Count(v => SequenceLengths(v).Any(seq => seq == 2));
			Console.WriteLine($"Day  4 Puzzle 2: {matches2}");
			Debug.Assert(matches2 == 290);

			IEnumerable<int> SequenceLengths(IReadOnlyList<int> value)
			{
				var digit = value[0]; // Assume at least 1-digit values
				var seqlen = 1;
				for (var pos = 1; pos < value.Count; pos++)
				{
					if (value[pos] == digit)
					{
						seqlen++;
					}
					else
					{
						yield return seqlen;
						digit = value[pos];
						seqlen = 1;
					}
				}
				yield return seqlen;
			}

			IEnumerable<int[]> CalcMatches(int begin, int end)
			{
				// Pick out digits from begin-value; add leading 0 to avoid special case for overflow
				var digits = $"0{begin.ToString()}".ToCharArray().Select(x => int.Parse($"{x}")).ToArray();
				while (true)
				{
					// Increment number until it consist only of increasing digits
					for (var pos = 1; pos < digits.Length; pos++)
					{
						if (digits[pos] < digits[pos - 1])
						{
							digits[pos] = digits[pos - 1];
						}
					}

					// Stop if we've moved beyond the end
					var digitValue = digits.Aggregate(0, (sum, digit) => sum * 10 + digit);
					if (digitValue > end)
					{
						break;
					}

					// This is a candidate
					yield return digits;

					// Increment the number one digit at a time, starting from the least significant digit
					// Example: 456789 -> 456790
					// Example: 678999 -> 679000
					for (var pos = digits.Length - 1; pos >= 0 && ++digits[pos] > 9; pos--)
					{
						digits[pos] = 0;
					}
				}
			}
		}

		private static void Day5()
		{
			var mem = new[] { 3, 225, 1, 225, 6, 6, 1100, 1, 238, 225, 104, 0, 1002, 148, 28, 224, 1001, 224, -672, 224, 4, 224, 1002, 223, 8, 223, 101, 3, 224, 224, 1, 224, 223, 223, 1102, 8, 21, 225, 1102, 13, 10, 225, 1102, 21, 10, 225, 1102, 6, 14, 225, 1102, 94, 17, 225, 1, 40, 173, 224, 1001, 224, -90, 224, 4, 224, 102, 8, 223, 223, 1001, 224, 4, 224, 1, 224, 223, 223, 2, 35, 44, 224, 101, -80, 224, 224, 4, 224, 102, 8, 223, 223, 101, 6, 224, 224, 1, 223, 224, 223, 1101, 26, 94, 224, 101, -120, 224, 224, 4, 224, 102, 8, 223, 223, 1001, 224, 7, 224, 1, 224, 223, 223, 1001, 52, 70, 224, 101, -87, 224, 224, 4, 224, 1002, 223, 8, 223, 1001, 224, 2, 224, 1, 223, 224, 223, 1101, 16, 92, 225, 1101, 59, 24, 225, 102, 83, 48, 224, 101, -1162, 224, 224, 4, 224, 102, 8, 223, 223, 101, 4, 224, 224, 1, 223, 224, 223, 1101, 80, 10, 225, 101, 5, 143, 224, 1001, 224, -21, 224, 4, 224, 1002, 223, 8, 223, 1001, 224, 6, 224, 1, 223, 224, 223, 1102, 94, 67, 224, 101, -6298, 224, 224, 4, 224, 102, 8, 223, 223, 1001, 224, 3, 224, 1, 224, 223, 223, 4, 223, 99, 0, 0, 0, 677, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1105, 0, 99999, 1105, 227, 247, 1105, 1, 99999, 1005, 227, 99999, 1005, 0, 256, 1105, 1, 99999, 1106, 227, 99999, 1106, 0, 265, 1105, 1, 99999, 1006, 0, 99999, 1006, 227, 274, 1105, 1, 99999, 1105, 1, 280, 1105, 1, 99999, 1, 225, 225, 225, 1101, 294, 0, 0, 105, 1, 0, 1105, 1, 99999, 1106, 0, 300, 1105, 1, 99999, 1, 225, 225, 225, 1101, 314, 0, 0, 106, 0, 0, 1105, 1, 99999, 108, 677, 677, 224, 102, 2, 223, 223, 1005, 224, 329, 101, 1, 223, 223, 1107, 677, 226, 224, 102, 2, 223, 223, 1006, 224, 344, 101, 1, 223, 223, 1107, 226, 226, 224, 102, 2, 223, 223, 1006, 224, 359, 101, 1, 223, 223, 1108, 677, 677, 224, 102, 2, 223, 223, 1005, 224, 374, 101, 1, 223, 223, 8, 677, 226, 224, 1002, 223, 2, 223, 1005, 224, 389, 101, 1, 223, 223, 108, 226, 677, 224, 1002, 223, 2, 223, 1006, 224, 404, 1001, 223, 1, 223, 107, 677, 677, 224, 102, 2, 223, 223, 1006, 224, 419, 101, 1, 223, 223, 1007, 226, 226, 224, 102, 2, 223, 223, 1005, 224, 434, 101, 1, 223, 223, 1007, 677, 677, 224, 102, 2, 223, 223, 1005, 224, 449, 1001, 223, 1, 223, 8, 677, 677, 224, 1002, 223, 2, 223, 1006, 224, 464, 101, 1, 223, 223, 1108, 677, 226, 224, 1002, 223, 2, 223, 1005, 224, 479, 101, 1, 223, 223, 7, 677, 226, 224, 1002, 223, 2, 223, 1005, 224, 494, 101, 1, 223, 223, 1008, 677, 677, 224, 1002, 223, 2, 223, 1006, 224, 509, 1001, 223, 1, 223, 1007, 226, 677, 224, 1002, 223, 2, 223, 1006, 224, 524, 1001, 223, 1, 223, 107, 226, 226, 224, 1002, 223, 2, 223, 1006, 224, 539, 1001, 223, 1, 223, 1107, 226, 677, 224, 102, 2, 223, 223, 1005, 224, 554, 101, 1, 223, 223, 1108, 226, 677, 224, 102, 2, 223, 223, 1006, 224, 569, 101, 1, 223, 223, 108, 226, 226, 224, 1002, 223, 2, 223, 1006, 224, 584, 1001, 223, 1, 223, 7, 226, 226, 224, 1002, 223, 2, 223, 1006, 224, 599, 101, 1, 223, 223, 8, 226, 677, 224, 102, 2, 223, 223, 1005, 224, 614, 101, 1, 223, 223, 7, 226, 677, 224, 1002, 223, 2, 223, 1005, 224, 629, 101, 1, 223, 223, 1008, 226, 677, 224, 1002, 223, 2, 223, 1006, 224, 644, 101, 1, 223, 223, 107, 226, 677, 224, 1002, 223, 2, 223, 1005, 224, 659, 1001, 223, 1, 223, 1008, 226, 226, 224, 1002, 223, 2, 223, 1006, 224, 674, 1001, 223, 1, 223, 4, 223, 99, 226 };
			var engine = new Intcode.Engine();

			var result1 = engine
				.WithMemory(mem)
				.WithInput(1)
				.Execute()
				.Output.TakeAll()
				.SkipWhile(x => x == 0)
				.First();
			Console.WriteLine($"Day  5 Puzzle 1: {result1}");
			Debug.Assert(result1 == 7566643);

			var result2 = engine
				.WithMemory(mem)
				.WithInput(5)
				.Execute()
				.Output.Take();
			Console.WriteLine($"Day  5 Puzzle 2: {result2}");
			Debug.Assert(result2 == 9265694);
		}

		private static void Day6()
		{
			var orbitdefs = File.ReadLines("inputs/day6.txt")
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Split(')'))
				.ToList();
			var nodes = new Dictionary<string, List<string>>();
			foreach (var o in orbitdefs)
			{
				if (!nodes.ContainsKey(o[0]))
				{
					nodes[o[0]] = new List<string> { o[1] };
				}
				else
				{
					nodes[o[0]].Add(o[1]);
				}
			}
			var root = nodes.Keys.Except(nodes.SelectMany(x => x.Value)).First();
			var orbitCount = CountOrbits(0, root);
			Console.WriteLine($"Day  6 Puzzle 1: {orbitCount}");
			Debug.Assert(orbitCount == 387356);

			int CountOrbits(int orbitlevel, string name) => 
				nodes.TryGetValue(name, out var o)
					? orbitlevel + o.Select(x => CountOrbits(orbitlevel + 1, x)).Sum()
					: orbitlevel;

			var you = FindPathTo("YOU").ToList();
			var san = FindPathTo("SAN").ToList();
			var dist = you.Count + san.Count - 2;
			for (var i = 0; you[i] == san[i]; i++)
			{
				dist -= 2;
			}
			Console.WriteLine($"Day  6 Puzzle 2: {dist}");
			Debug.Assert(dist == 532);

			IEnumerable<string> FindPathTo(string name)
			{
				if (name == root)
				{
					yield return name;
				}
				else
				{
					var obj = orbitdefs.First(o => o[1] == name);
					foreach (var o in FindPathTo(obj[0]))
					{
						yield return o;
					}
					yield return obj[1];
				}
			}
		}

		private static void Day7()
		{
			var mem = new int[] { 3, 8, 1001, 8, 10, 8, 105, 1, 0, 0, 21, 46, 59, 84, 93, 110, 191, 272, 353, 434, 99999, 3, 9, 101, 2, 9, 9, 102, 3, 9, 9, 1001, 9, 5, 9, 102, 4, 9, 9, 1001, 9, 4, 9, 4, 9, 99, 3, 9, 101, 3, 9, 9, 102, 5, 9, 9, 4, 9, 99, 3, 9, 1001, 9, 4, 9, 1002, 9, 2, 9, 101, 2, 9, 9, 102, 2, 9, 9, 1001, 9, 3, 9, 4, 9, 99, 3, 9, 1002, 9, 2, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 1001, 9, 5, 9, 1002, 9, 3, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 99, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 99, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 99 };

			IEnumerable<IEnumerable<int>> Permute(IEnumerable<int> x)
			{
				if (x.Count() == 1)
				{
					yield return x;
				}
				else
				{
					foreach (var head in x)
					{
						foreach (var perm in Permute(x.Where(y => y != head)))
						{
							yield return new[] { head }.Concat(perm);
						}
					}
				}
			}

			var maxSignal1 = Permute(Enumerable.Range(0, 5))
				.Max(phases =>
				{
					var signal = 0L;
					foreach (var phase in phases)
					{
						var engine = new Intcode.Engine();
						engine
							.WithMemory(mem)
							.WithInput(phase, signal)
							.Execute();
						signal = engine.Output.Take();
					}
					return signal;
				});
			Console.WriteLine($"Day  7 Puzzle 1: {maxSignal1}");
			Debug.Assert(maxSignal1 == 19650);

			var maxSignal2 = Permute(Enumerable.Range(5, 5))
				.Max(phases =>
				{
					var engines = phases.Select(phase => new Intcode.Engine().WithMemory(mem).WithInput(phase)).ToList();
					var n = engines.Count;
					for (var i = 0; i < n; i++)
					{
						engines[(i + n - 1) % n].Output = engines[i].Input;
					}
					engines[0].WithInput(0);
					return engines
						.AsParallel()
						.WithDegreeOfParallelism(n)
						.Select(e => e.Execute())
						.AsSequential()
						.First(e => e == engines.Last())
						.Output
						.Take();
				});
			Console.WriteLine($"Day  7 Puzzle 2: {maxSignal2}");
			Debug.Assert(maxSignal2 == 35961106);
		}

		private static void Day8()
		{
			var imagedata = File.ReadAllText("inputs/day8.txt");

			const int width = 25;
			const int height = 6;
			const int size = width * height;

			// Divide raw imagedata into the individual layers
			var layers = Enumerable.Range(0, imagedata.Length / size)
				.Select(i => imagedata.Substring(i * size, size))
				.ToList();

			// FInd the layer with most 0's and multiply its 1's and 2's
			var layerWithMostZeros = layers
				.Select(l => new
				{
					Count0 = l.Count(x => x == '0'),
					Layer = l
				})
				.OrderBy(x => x.Count0)
				.Select(x => x.Layer)
				.First();
			var sum = layerWithMostZeros.Count(x => x == '1') * layerWithMostZeros.Count(x => x == '2');
			Console.WriteLine($"Day  8 Puzzle 1: {sum}");
			Debug.Assert(sum == 2356);

			// Render all "pixels" by looping through each layer's similar positions and
			// pick the first non-transparent value, turning '1' into black and '2' into blank.
			var rendering = Enumerable.Range(0, size)
				.Select(pos => layers.Select(x => x[pos]))
				.Select(x =>
				{
					var value = x.First(pixel => pixel != '2');
					return value == '1' ? Graphics.FullBlock : ' ';
				})
				.ToArray();
			var image = new string(rendering);

			// Split rendering into <height> individual lines and print them
			var lines = Enumerable.Range(0, height)
				.Select(x => image.Substring(x * width, width));
			foreach (var line in lines)
			{
				Console.WriteLine($"Day  8 Puzzle 2: {line}");
			}
		}

		private static void Day9()
		{
			var mem = new [] { 1102, 34463338, 34463338, 63, 1007, 63, 34463338, 63, 1005, 63, 53, 1102, 3, 1, 1000, 109, 988, 209, 12, 9, 1000, 209, 6, 209, 3, 203, 0, 1008, 1000, 1, 63, 1005, 63, 65, 1008, 1000, 2, 63, 1005, 63, 904, 1008, 1000, 0, 63, 1005, 63, 58, 4, 25, 104, 0, 99, 4, 0, 104, 0, 99, 4, 17, 104, 0, 99, 0, 0, 1101, 20, 0, 1007, 1101, 0, 197, 1022, 1102, 475, 1, 1028, 1102, 30, 1, 1008, 1101, 25, 0, 1010, 1102, 1, 23, 1009, 1101, 0, 22, 1013, 1101, 470, 0, 1029, 1102, 24, 1, 1014, 1102, 1, 39, 1005, 1101, 31, 0, 1003, 1101, 807, 0, 1026, 1101, 0, 26, 1018, 1102, 1, 804, 1027, 1101, 0, 0, 1020, 1102, 1, 38, 1017, 1101, 0, 27, 1016, 1102, 443, 1, 1024, 1101, 0, 36, 1006, 1102, 21, 1, 1015, 1101, 28, 0, 1001, 1102, 33, 1, 1019, 1102, 1, 37, 1011, 1102, 1, 190, 1023, 1101, 0, 434, 1025, 1101, 34, 0, 1004, 1102, 1, 1, 1021, 1101, 0, 29, 1012, 1102, 1, 32, 1002, 1101, 35, 0, 1000, 109, 30, 2105, 1, -7, 1001, 64, 1, 64, 1105, 1, 199, 4, 187, 1002, 64, 2, 64, 109, -23, 2101, 0, -5, 63, 1008, 63, 32, 63, 1005, 63, 225, 4, 205, 1001, 64, 1, 64, 1105, 1, 225, 1002, 64, 2, 64, 109, 7, 2102, 1, -5, 63, 1008, 63, 23, 63, 1005, 63, 251, 4, 231, 1001, 64, 1, 64, 1106, 0, 251, 1002, 64, 2, 64, 109, -16, 2101, 0, 2, 63, 1008, 63, 33, 63, 1005, 63, 275, 1001, 64, 1, 64, 1106, 0, 277, 4, 257, 1002, 64, 2, 64, 109, 10, 21102, 40, 1, 4, 1008, 1012, 40, 63, 1005, 63, 299, 4, 283, 1106, 0, 303, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 7, 2102, 1, -9, 63, 1008, 63, 33, 63, 1005, 63, 327, 1001, 64, 1, 64, 1105, 1, 329, 4, 309, 1002, 64, 2, 64, 109, -17, 2107, 34, 2, 63, 1005, 63, 347, 4, 335, 1105, 1, 351, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 1, 1201, 8, 0, 63, 1008, 63, 23, 63, 1005, 63, 375, 1001, 64, 1, 64, 1106, 0, 377, 4, 357, 1002, 64, 2, 64, 109, -4, 2108, 31, 8, 63, 1005, 63, 395, 4, 383, 1105, 1, 399, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 3, 1201, 8, 0, 63, 1008, 63, 36, 63, 1005, 63, 421, 4, 405, 1105, 1, 425, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 25, 2105, 1, 1, 4, 431, 1001, 64, 1, 64, 1105, 1, 443, 1002, 64, 2, 64, 109, -3, 1205, 0, 459, 1001, 64, 1, 64, 1106, 0, 461, 4, 449, 1002, 64, 2, 64, 109, -2, 2106, 0, 10, 4, 467, 1106, 0, 479, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 12, 1206, -9, 495, 1001, 64, 1, 64, 1106, 0, 497, 4, 485, 1002, 64, 2, 64, 109, -39, 1207, 9, 36, 63, 1005, 63, 519, 4, 503, 1001, 64, 1, 64, 1105, 1, 519, 1002, 64, 2, 64, 109, 11, 1202, -1, 1, 63, 1008, 63, 28, 63, 1005, 63, 541, 4, 525, 1105, 1, 545, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 6, 2107, 24, 1, 63, 1005, 63, 565, 1001, 64, 1, 64, 1106, 0, 567, 4, 551, 1002, 64, 2, 64, 109, 1, 1207, -3, 35, 63, 1005, 63, 583, 1106, 0, 589, 4, 573, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 1, 21102, 41, 1, 5, 1008, 1015, 40, 63, 1005, 63, 613, 1001, 64, 1, 64, 1105, 1, 615, 4, 595, 1002, 64, 2, 64, 109, -2, 2108, 22, 1, 63, 1005, 63, 635, 1001, 64, 1, 64, 1105, 1, 637, 4, 621, 1002, 64, 2, 64, 109, -10, 1208, 4, 33, 63, 1005, 63, 653, 1106, 0, 659, 4, 643, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 16, 1206, 6, 673, 4, 665, 1106, 0, 677, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -4, 1202, -8, 1, 63, 1008, 63, 35, 63, 1005, 63, 701, 1001, 64, 1, 64, 1105, 1, 703, 4, 683, 1002, 64, 2, 64, 109, 13, 21108, 42, 42, -8, 1005, 1015, 721, 4, 709, 1105, 1, 725, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -18, 21107, 43, 44, 5, 1005, 1010, 743, 4, 731, 1106, 0, 747, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -11, 1208, 8, 32, 63, 1005, 63, 765, 4, 753, 1106, 0, 769, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 15, 21101, 44, 0, 5, 1008, 1014, 47, 63, 1005, 63, 789, 1105, 1, 795, 4, 775, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 13, 2106, 0, 5, 1106, 0, 813, 4, 801, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -12, 21108, 45, 43, 0, 1005, 1010, 829, 1106, 0, 835, 4, 819, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -4, 21107, 46, 45, 10, 1005, 1016, 855, 1001, 64, 1, 64, 1106, 0, 857, 4, 841, 1002, 64, 2, 64, 109, 3, 21101, 47, 0, 5, 1008, 1014, 47, 63, 1005, 63, 883, 4, 863, 1001, 64, 1, 64, 1106, 0, 883, 1002, 64, 2, 64, 109, 10, 1205, 2, 901, 4, 889, 1001, 64, 1, 64, 1105, 1, 901, 4, 64, 99, 21102, 27, 1, 1, 21102, 915, 1, 0, 1106, 0, 922, 21201, 1, 13433, 1, 204, 1, 99, 109, 3, 1207, -2, 3, 63, 1005, 63, 964, 21201, -2, -1, 1, 21101, 0, 942, 0, 1106, 0, 922, 22102, 1, 1, -1, 21201, -2, -3, 1, 21102, 1, 957, 0, 1105, 1, 922, 22201, 1, -1, -2, 1106, 0, 968, 21202, -2, 1, -2, 109, -3, 2106, 0, 0 };
			var engine = new Intcode.Engine();

			var result1 = engine
				.WithMemory(mem)
				.WithInput(1)
				.Execute()
				.Output.Take();
			Console.WriteLine($"Day  9 Puzzle 1: {result1}");
			Debug.Assert(result1 == 2682107844);

			var result2 = engine
				.WithMemory(mem)
				.WithInput(2)
				.Execute()
				.Output.Take();
			Console.WriteLine($"Day  9 Puzzle 2: {result2}");
			Debug.Assert(result2 == 34738);
		}

		private static void Day10()
		{
			var asteroidmap = File.ReadAllLines("inputs/day10.txt");

			var (stationX, stationY, detectableAsteroids) = MaxDetectable(asteroidmap);
			Console.WriteLine($"Day 10 Puzzle 1: {detectableAsteroids} at {stationX},{stationY}");
			Debug.Assert(detectableAsteroids == 299);

			(int, int, int) MaxDetectable(string[] mapinfo)
			{
				var w = mapinfo[0].Length;
				var h = mapinfo.Length;

				var asteroid = Enumerable.Range(0, w).SelectMany(x =>
					Enumerable.Range(0, h).Select(y => new
					{
						X = x,
						Y = y,
						Detectable = DetectableFrom(x, y)
					})
				)
				.OrderByDescending(x => x.Detectable)
				.First();
				return (asteroid.X, asteroid.Y, asteroid.Detectable);

				int DetectableFrom(int xpos, int ypos)
				{
					// Create a copy of the map to work on
					var map = mapinfo.Select(x => x.ToArray()).ToArray();
					map[ypos][xpos] = '@'; // welcome to nethack

					var detected = 0;
					for (var dx = 0; xpos + dx >= 0; dx--)
					{
						MapX(dx);
					}
					for (var dx = 1; xpos + dx < w; dx++)
					{
						MapX(dx);
					}
					return map.Sum(row => row.Count(ch => ch == '#'));

					void MapX(int dx)
					{
						for (var dy = 0; ypos + dy >= 0; dy--)
						{
							if (dx == 0 && dy == 0)
								continue;
							MapXY(dx, dy);
						}
						for (var dy = 1; ypos + dy < h; dy++)
						{
							MapXY(dx, dy);
						}
					}

					void MapXY(int dx, int dy)
					{
						var x = xpos;
						var y = ypos;
						var visible = true;
						while (0 <= x && x < w && 0 <= y && y < h)
						{
							if (!visible)
							{
								map[y][x] = ' ';
							}
							else if (map[y][x] == '#')
							{
								detected++;
								visible = false;
							}
							x += dx;
							y += dy;
						}
					}
				}
			}

			//var map2 = asteroidmap.Select(x => x.ToArray()).ToArray();
			//map2[stationY][stationX] = 'X';
			//int xx = 65;
			//foreach (var a in VaporizedAsteroids(asteroidmap, xloc, yloc))
			//{
			//	var x = a / 100;
			//	var y = a % 100;
			//	Console.WriteLine($">>> {x},{y}");
			//	map2[y][x] = char.ConvertFromUtf32(xx++).First();
			//	for (var line = 0; line < asteroidmap.Length; line++)
			//	{
			//		Console.WriteLine($"{line % 10}" + new string(map2[line]));
			//	}
			//	Console.ReadLine();
			//}

			var asteroid200 = VaporizedAsteroidsFrom(asteroidmap, stationX, stationY)
				.Skip(199)
				.First();
			Console.WriteLine($"Day 10 Puzzle 2: {asteroid200}");
			Debug.Assert(asteroid200 == 1419);

			IEnumerable<int> VaporizedAsteroidsFrom(string[] mapinfo, int xpos, int ypos)
			{
				var w = mapinfo[0].Length;
				var h = mapinfo.Length;

				var agByDist =
					Enumerable.Range(0, w).SelectMany(x =>
						Enumerable.Range(0, h).Select(y =>
						{
							if (mapinfo[y][x] == '#' && (x != xpos || y != ypos))
							{
								var angle = Math.Atan2(ypos - y, xpos - x) - Math.PI / 2;
								var dx = x - xpos;
								var dy = y - ypos;
								return new
								{
									X = x,
									Y = y,
									Dist = Math.Sqrt(dx * dx + dy * dy),
									Angle = angle >= 0 ? angle : angle + 2 * Math.PI
								};
							}
							return null;
						}))
				.Where(x => x != null)
				.GroupBy(x => x.Angle)
				.OrderBy(x => x.Key)
				.Select(a => a.OrderBy(x => x.Dist));

				for (var dist = 0; agByDist.Any(x => x.Count() > dist); dist++)
				{
					foreach (var ag in agByDist)
					{
						if (ag.Count() > dist)
						{
							var asteroid = ag.ElementAt(dist);
							//Console.WriteLine($"{asteroid.X},{asteroid.Y} at angle {asteroid.Angle}");
							//if (astoroids.Count() > dist + 1)
							//{
							//	Console.WriteLine($"--- skipping {string.Join(" ", astoroids.Skip(dist + 1).Select(x => $"{x.X},{x.Y}"))}");
							//}
							yield return asteroid.X * 100 + asteroid.Y;
						}
					}
				}
			}
		}




		private static void Day()
		{
			var result1 = 0;
			Console.WriteLine($"Day  Puzzle 1: {result1}");
			//Debug.Assert(result1 == ...)

			var result2 = 0;
			Console.WriteLine($"Day  Puzzle 2: {result2}");
			//Debug.Assert(result2 == ...)
		}
	}
}
