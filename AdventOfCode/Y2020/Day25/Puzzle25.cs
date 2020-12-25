using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day25
{
	internal class Puzzle : SoloParts<uint>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 25;

		public void Run()
		{
			RunPart1For("test1", 14897079U);
			RunPart1For("input", 11288669U);
		}

		protected override uint Part1(string[] input)
		{
			var cardPub = uint.Parse(input[0]);
			var doorPub = uint.Parse(input[1]);

			//To transform a subject number, start with the value 1. Then, a number of times called the loop size, perform the following steps:
			//Set the value to itself multiplied by the subject number.
			//Set the value to the remainder after dividing the value by 20201227.

			var doorSecret = FindSecret(7, doorPub);
			var encryption = EncodeSubject(cardPub, doorSecret);

			return encryption;
		}

		private static uint EncodeSubject(uint subject, uint secret)
		{
			var num = 1UL;
			for (var i = 0; i < secret; i++)
			{
				num = (num * subject) % 20201227;
			}
			return (uint)num;
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

		protected override uint Part2(string[] input) => 0U;
	}
}