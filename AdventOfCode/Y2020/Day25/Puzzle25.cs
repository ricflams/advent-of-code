using AdventOfCode.Helpers.Puzzles;
using System.Numerics;

namespace AdventOfCode.Y2020.Day25
{
	internal class Puzzle : PuzzleWithParam<(uint, uint), uint, bool>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Combo Breaker";
		public override int Year => 2020;
		public override int Day => 25;

		public void Run()
		{
			Run("test1").WithParam((5764801, 17807724)).WithNoInput().Part1(14897079U);
			Run("input").WithParam((5099500,  7648211)).WithNoInput().Part1(11288669U);
		}

		protected override uint Part1(string[] _)
		{
			var (cardPub, doorPub) = Param;

			//To transform a subject number, start with the value 1. Then, a number of times called the loop size, perform the following steps:
			//Set the value to itself multiplied by the subject number.
			//Set the value to the remainder after dividing the value by 20201227.

			var doorSecret = FindSecret(7, doorPub);
			var encryption = EncodeSubject(cardPub, doorSecret);

			return encryption;
		}

		private static uint EncodeSubject(uint subject, uint secret)
		{
			// x = a^b % n can be solved for x using ModPow
			return (uint)BigInteger.ModPow(subject, secret, 20201227);
		}

		private static uint FindSecret(uint subject, uint pub)
		{
			var num = 1UL;
			for (var i = 1U;; i++)
			{
				num = (num * subject) % 20201227;
				if (num == pub)
				{
					return i;
				}
			}
		}

		protected override bool Part2(string[] input) => true;
	}
}
