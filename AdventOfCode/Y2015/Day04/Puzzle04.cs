using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace AdventOfCode.Y2015.Day04
{
	internal class Puzzle04
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var answer1 = FirstZerosAt("bgvyzdsv", 0, hash => hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xf0) == 0);
			Console.WriteLine($"Day  4 Puzzle 1: {answer1}");
			Debug.Assert(answer1 == 254575);

			// Puzzle 2 can resume searching where Puzzle 1 ended
			var answer2 = FirstZerosAt("bgvyzdsv", answer1, hash => hash[0] == 0 && hash[1] == 0 && hash[2] == 0);
			Console.WriteLine($"Day  4 Puzzle 2: {answer2}");
			Debug.Assert(answer2 == 1038736);
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
