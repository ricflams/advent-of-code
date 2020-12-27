using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode.Y2015.Day04
{

	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2015;
		protected override int Day => 4;

		public void Run()
		{
			RunFor("input", 254575, 1038736);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var secret = input[0];
			var answer1 = FirstZerosAt(secret, 0, hash => hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xf0) == 0);
			// Puzzle 2 can resume searching where Puzzle 1 ended
			var answer2 = FirstZerosAt(secret, answer1, hash => hash[0] == 0 && hash[1] == 0 && hash[2] == 0);
			return (answer1, answer2);
		}

		private static int FirstZerosAt(string input, int start, Func<byte[], bool> condition)
		{
			var md5 = MD5.Create();
			var secret = input.ToCharArray().Select(x => (byte)x).ToArray();
			var buffer = new byte[100]; // more than big enough
			Array.Copy(secret, 0, buffer, 0, secret.Length);
			for (var i = start; ; i++)
			{
				var guess = Encoding.ASCII.GetBytes(i.ToString());
				Array.Copy(guess, 0, buffer, secret.Length, guess.Length);
				var hash = md5.ComputeHash(buffer, 0, secret.Length + guess.Length);
				if (condition(hash))
				{
					return i;
				}
			}
		}
	}
}
