using System.Collections.Generic;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day21
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Chronal Conversion";
		public override int Year => 2018;
		public override int Day => 21;

		public override void Run()
		{
			Run("input").Part1(13270004).Part2(12879142);
		}

		protected override long Part1(string[] input)
		{
			var magic1 = int.Parse(input[8].Split()[1]);
			var magic2 = int.Parse(input[12].Split()[2]);

			var d = 0;
			
			var f = d | 0x10000;
			d = magic1;
			while (true)
			{
				d = ((d + (f&0xff) & 0xffffff) * magic2) & 0xffffff;
				if (f < 256)
					break;
				f = f / 256;
			}

			return d;
		}

		protected override long Part2(string[] input)
		{
			var magic1 = int.Parse(input[8].Split()[1]);
			var magic2 = int.Parse(input[12].Split()[2]);

			var seen = new HashSet<int>();
			var last = 0;

			var d = 0;
			while (true)
			{
				var f = d | 0x10000;

				// If a loop is detected then the highest value for A has been found
				if (seen.Contains(d))
					break;
				seen.Add(d);
				last = d;
				
				d = magic1;
				while (true)
				{
					d = ((d + (f&0xff) & 0xffffff) * magic2) & 0xffffff;
					if (f < 256)
						break;
					f = f / 256;
				}
			}

			return last;
		}
	}
}

// # Disassembly
//     IpReg = B
//     00   D = 123/1111011
//     01   D = D & 456/111001000
//     02   D = D == 72/1001000 ? 1 : 0
//     03   B = D + B
//     04   B = 0/0
//     05   D = 0/0
//     06   F = D | 65536/10000000000000000
//     07   D = 15028787/111001010101001000110011
//     08   C = F & 255/11111111
//     09   D = D + C
//     10   D = D & 16777215/111111111111111111111111
//     11   D = D * 65899/10000000101101011
//     12   D = D & 16777215/111111111111111111111111
//     13   C = 256/100000000 > F ? 1 : 0
//     14   B = C + B
//     15   B = B + 1/1
//     16   B = 27/11011
//     17   C = 0/0
//     18   E = C + 1/1
//     19   E = E * 256/100000000
//     20   E = E > F ? 1 : 0
//     21   B = E + B
//     22   B = B + 1/1
//     23   B = 25/11001
//     24   C = C + 1/1
//     25   B = 17/10001
//     26   F = C
//     27   B = 7/111
//     28   C = D == A ? 1 : 0
//     29   B = C + B
//     30   B = 5/101
//
// # Pseudo
//     D = 0
//     do {
//         var F = D | 65536;
//         D = 15028787;// # prime
//         while (true) {
//             var C = F & 255;
//             D += C;
//             D &= 16777215;
//             D *= 65899; //# prime
//             D &= 16777215;
//             if (F < 256)
//                 break;
//             C = 0;
//             var E = 0;
//             do {
//                 E = C + 1;
//                 E *= 256;
//                 if (E <= F)
//                     C++;
//             } while (E <= F);
//             F = C;
//         }
//     } while (D != A);
//
// # Compact
//     var d = 0;
//     do
//     {
//         var f = d | 0x10000;
//         d = 15028787;
//         while (true)
//         {
//             d = ((d + (f & 0xff) & 0xffffff) * 65899) & 0xffffff;
//             if (f < 256)
//                 break;
//             f = f / 256;
//         }
//     } while (d != a);
