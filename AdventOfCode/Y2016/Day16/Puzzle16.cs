using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2016.Day16
{
	internal class Puzzle : PuzzleWithParameter<(int, int), string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Dragon Checksum";
		public override int Year => 2016;
		public override int Day => 16;

		public void Run()
		{
			Run("test1")
				.WithParameter((20, 0))
				.Part1("01100");
			Run("input")
				.WithParameter((272, 35651584))
				.Part1("00100111000101111")
				.Part2("11101110011100110");
		}

		protected override string Part1(string[] input)
		{
			var state = input[0];
			var (length, _) = PuzzleParameter;
			return Checksum(state, length);
		}

		protected override string Part2(string[] input)
		{
			var state = input[0];
			var (_, length) = PuzzleParameter;
			return Checksum(state, length);
		}

		private static string Checksum(string state, int disksize)
		{
			// Build up everything in one single buffer.
			// It's going to look like so:
			// Step 0: A
			// Step 1: A 0 A' (A' is reverse+inverse A)
			// Step 2: A 0 A' 0 A 1 A' (A'' == A, because double reverse+inverse cancels out)
			// Step 3: A 0 A' 0 A 1 A' 0 A 0 A' 1 A 1 A'
			// .....
			// Step N: total length is 2^N*len(A) + 2^N-1, because there are 2^N A with one less digit inbetween
			// 
			// Thus:      disksize = 2^N*len(A) + 2^N-1
			//       <=>  disksize+1 = 2^N*(len(A) + 1)
			//       <=>  2^N = (disksize+1) / (len(A)+1)
			//       <=>  N = log2((disksize+1) / (len(A)+1))
			// Easiest to run a full number of iterations so N must be a full number.
			// Therefore we round up and calculate the required buffer-size from that N.
			var N = (int)Math.Ceiling(Math.Log2((disksize+1.0) / (state.Length + 1)));
			var bufsize = (1U<<N) * (state.Length + 1) - 1;

			// Fill in the initial state
			var a = new byte[bufsize];
			for (var i = 0; i < state.Length; i++)
			{
				a[i] = state[i] == '1' ? (byte)1 : (byte)0;
			}

			// Now do the N steps, reverse+invert the entire buffer at every step
			// Let length represent how much we've filled out so far. It will
			// grow as nextlen = len + 1 + len for every step.
			var len = state.Length;
			for (var loop = 0; loop < N; loop++)
			{
				for (int i = 0, i2 = len * 2; i < len; i++, i2--)
				{
					a[i2] = (byte)(1 - a[i]);
				}
				a[len] = 0; // Middle diigit is always 0
				len = len * 2 + 1;
			}

			// For reduction we don't need to allocate a new buffer but
			// can simply shift into the lower part of the existing one
			while (disksize % 2 == 0)
			{
				disksize /= 2;
				for (var i = 0; i < disksize; i++)
				{
					a[i] = a[i*2] == a[i*2+1] ? (byte)1 : (byte)0;
				}
			}

			var digits = a.Take(disksize).Select(x => x == 1 ? '1' : '0').ToArray();
			var checksum = new string(digits);

			return checksum;			
		}
	}
}
