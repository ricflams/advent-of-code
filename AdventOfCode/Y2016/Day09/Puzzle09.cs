using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2016.Day09
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Explosives in Cyberspace";
		public override int Year => 2016;
		public override int Day => 9;

		public void Run()
		{
			//RunFor("test1", 0, 0);
			//RunFor("test2", 0, 0);
			RunFor("input", 0, 0);
		}

		protected override long Part1(string[] input)
		{

			// ADVENT contains no markers and decompresses to itself with no changes, resulting in a decompressed length of 6.
			// A(1x5)BC repeats only the B a total of 5 times, becoming ABBBBBC for a decompressed length of 7.
			// (3x3)XYZ becomes XYZXYZXYZ for a decompressed length of 9.
			// A(2x2)BCD(2x2)EFG doubles the BC and EF, becoming ABCBCDEFEFG for a decompressed length of 11.
			// (6x1)(1x3)A simply becomes (1x3)A - the (1x3) looks like a marker, but because it's within a data section of another marker, it is not treated any differently from the A that comes after it. It has a decompressed length of 6.
			// X(8x2)(3x3)ABCY becomes X(3x3)ABC(3x3)ABCY (for a decompressed length of 18), because the decompressed data from the (8x2) marker (the (3x3)ABC) is skipped and not processed further.
			var seq = input[0].TrimAll();

			var s = new StringBuilder();
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
					while (repeat-- > 0)
					{
						s.Append(letters);
					}
				}
				else
				{
					s.Append(seq[i]);
					i++;
				}
				//var xx = s.ToString();
			}

			static int ParseNumber(string s, ref int pos)
			{
				var val = 0;
				while (char.IsDigit(s[pos]))
				{
					val = val * 10 + s[pos++] - '0';
				}
				return val;
			}

			var length = s.ToString().Length;


			return length;
		}


		private static long DecompressedLength(string seq)
		{
			var length = 0L;
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
					length += repeat * DecompressedLength(letters);
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

		protected override long Part2(string[] input)
		{
			var seq = input[0].TrimAll();
			var length = DecompressedLength(seq);


			return length;
		}
	}




	//  internal class Puzzle : ComboParts<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//		public override string Name => "";
	//  	public override int Year => 2016;
	//  	public override int Day => 9;
	//  
	//  	public void Run()
	//  	{
	//  		//RunFor("test1", 0, 0);
	//  		//RunFor("test2", 0, 0);
	//  		RunFor("input", 0, 0);
	//  	}
	//  
	//  	protected override (int, int) Part1And2(string[] input)
	//  	{
	//  
	//  
	//  
	//  
	//  
	//  		return (0, 0);
	//  	}
	//  }

}
