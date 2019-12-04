using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019
{
    class Program
    {
        static void Main(string[] args)
        {
			Day1();
			Day2();
			Day3();
			Day4();
			Console.Write("Done - press any key");
			Console.ReadKey();
        }

		private static void Day1()
		{
			var masses = new[] { 116115, 58728, 102094, 104856, 86377, 97920, 101639, 95328, 103730, 57027, 83080, 57748, 101606, 54629, 90901, 59983, 109795, 123270, 141948, 92969, 149805, 143555, 141387, 136357, 90236, 63577, 127108, 130012, 88223, 51426, 117663, 63924, 56251, 108505, 89625, 126994, 120237, 99351, 136948, 123702, 129849, 93541, 110900, 63759, 58537, 132943, 118213, 104274, 84606, 125256, 76355, 116711, 79344, 66355, 117654, 116026, 80244, 129786, 73054, 119806, 90941, 53877, 96707, 58226, 101666, 53819, 54558, 77342, 149653, 87843, 54388, 128862, 55752, 89962, 147224, 118486, 56910, 124854, 57052, 55495, 62530, 128104, 68788, 60915, 62155, 123614, 115522, 116920, 101263, 92339, 92234, 81542, 78062, 137207, 92082, 120032, 136537, 109035, 115819, 75955 };

			var mass1 = masses.Sum(FuelForMass);
			Console.WriteLine($"Day1: Puzzle1: {mass1}");

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
			Console.WriteLine($"Day1: Puzzle2: {mass2}");

			int FuelForMass(int mass) => mass / 3 - 2;
		}

		private static void Day2()
		{
			var mem = new[] { 1, 0, 0, 3, 1, 1, 2, 3, 1, 3, 4, 3, 1, 5, 0, 3, 2, 13, 1, 19, 1, 19, 10, 23, 1, 23, 6, 27, 1, 6, 27, 31, 1, 13, 31, 35, 1, 13, 35, 39, 1, 39, 13, 43, 2, 43, 9, 47, 2, 6, 47, 51, 1, 51, 9, 55, 1, 55, 9, 59, 1, 59, 6, 63, 1, 9, 63, 67, 2, 67, 10, 71, 2, 71, 13, 75, 1, 10, 75, 79, 2, 10, 79, 83, 1, 83, 6, 87, 2, 87, 10, 91, 1, 91, 6, 95, 1, 95, 13, 99, 1, 99, 13, 103, 2, 103, 9, 107, 2, 107, 10, 111, 1, 5, 111, 115, 2, 115, 9, 119, 1, 5, 119, 123, 1, 123, 9, 127, 1, 127, 2, 131, 1, 5, 131, 0, 99, 2, 0, 14, 0 };
			mem[1] = 12;
			mem[2] = 2;

			var engine = new Intcode.Engine();
			engine.Initialize(mem);
			engine.Execute();
			Console.WriteLine($"Day2: Puzzle1: mem[0] = {engine.Memory[0]}");

			for (var op1 = 0; op1 < 100; op1++)
			{
				for (var op2 = 0; op2 < 100; op2++)
				{
					mem[1] = op1;
					mem[2] = op2;
					engine.Initialize(mem);
					engine.Execute();
					if (engine.Memory[0] == 19690720)
					{
						Console.WriteLine($"Day2: Puzzle2: {op1 * 100 + op2}");
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
			Console.WriteLine($"Day3: Puzzle1: {nearest}");

			var fewestSteps = crossings.Min(x => x.Steps);
			Console.WriteLine($"Day3: Puzzle2: {fewestSteps}");

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

			const int XyFactor = 100000;
			int MakeXy(int x, int y) => x * XyFactor + y;
			int MakeX(int xy) => xy / XyFactor;
			int MakeY(int xy) => xy % XyFactor;
		}

		private static void Day4()
		{
			var matches1 = CalcMatches(382345, 843167).Count(v => SequenceLengths(v).Any(seq => seq >= 2));
			Console.WriteLine($"Day4: Puzzle1: {matches1}");

			var matches2 = CalcMatches(382345, 843167).Count(v => SequenceLengths(v).Any(seq => seq == 2));
			Console.WriteLine($"Day4: Puzzle2: {matches2}");

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
				var digits = $"0{begin.ToString()}".ToCharArray().Select(x => int.Parse($"{x}")).ToArray();
				while (true)
				{
					// Ensure that number consist only of increasing digits
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

	}
}
