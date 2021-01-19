using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2016.Day09
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Explosives in Cyberspace";
		public override int Year => 2016;
		public override int Day => 9;

		public void Run()
		{
			RunFor("input", 152851, 11797310782);
		}

		protected override long Part1(string[] input)
		{
			var sequence = input[0];
			return DecompressedLength(sequence, true);
		}


		protected override long Part2(string[] input)
		{
			var sequence = input[0];
			return DecompressedLength(sequence, false);
		}

		private static long DecompressedLength(string seq, bool isPart1)
		{
			// ADVENT contains no markers and decompresses to itself with no changes, resulting in a decompressed length of 6.
			// A(1x5)BC repeats only the B a total of 5 times, becoming ABBBBBC for a decompressed length of 7.
			// (3x3)XYZ becomes XYZXYZXYZ for a decompressed length of 9.
			// A(2x2)BCD(2x2)EFG doubles the BC and EF, becoming ABCBCDEFEFG for a decompressed length of 11.
			// (6x1)(1x3)A simply becomes (1x3)A - the (1x3) looks like a marker, but because it's within a data section of another marker, it is not treated any differently from the A that comes after it. It has a decompressed length of 6.
			// X(8x2)(3x3)ABCY becomes X(3x3)ABC(3x3)ABCY (for a decompressed length of 18), because the decompressed data from the (8x2) marker (the (3x3)ABC) is skipped and not processed further.
			var length = 0L;
			seq = seq.TrimAll();
			for (var i = 0; i < seq.Length; )
			{
				if (seq[i] == '(')
				{
					// Parse ( length x repeats ), then read length chars and insert them repeated
					i++; // (
					var len = ParseNumber(seq, ref i);
					i++; // x
					var repeat = ParseNumber(seq, ref i);
					i++; // )
					var letters = seq[i..(i+len)];
					//var yy = letters.Length;
					i += len;
					length += repeat * (isPart1 ? letters.Length : DecompressedLength(letters, isPart1));
				}
				else
				{
					i++;
					length++;
				}
			}
			return length;

			static int ParseNumber(string s, ref int pos)
			{
				var val = 0;
				while (char.IsDigit(s[pos]))
				{
					val = val * 10 + s[pos++] - '0';
				}
				return val;
			}
		}
	}
}
