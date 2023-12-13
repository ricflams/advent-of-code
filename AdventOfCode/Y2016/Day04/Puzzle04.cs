using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2016.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Security Through Obscurity";
		public override int Year => 2016;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(1514);
			Run("input").Part1(158835).Part2(993);
			Run("extra").Part1(361724).Part2(482);
		}

		protected override int Part1(string[] input)
		{
			// Each room consists of an encrypted name (lowercase letters separated by dashes)
			// followed by a dash, a sector ID, and a checksum in square brackets.
			var checksum = input
				.Select(line =>
				{
					// Eg aaaaa-bbb-z-y-x-123[abxyz]
					var (info, id, checksum) = line.RxMatch("%*-%d[%s]").Get<string, int, string>();
					return IsRealRoom(info, checksum) ? id : 0;
				})
				.Sum();
			return checksum;

			static bool IsRealRoom(string info, string checksum)
			{
				var letters = info.Replace("-", "").ToCharArray().GroupBy(x => x).OrderByDescending(x => x.Count()).ThenBy(x => x.Key);
				var mostcommon = new string(letters.Take(5).Select(x => x.Key).ToArray());
				return mostcommon == checksum;
			}
		}

		protected override int Part2(string[] input)
		{
			var roomname = "northpole object storage";
			var sectorId = input
				.Select(line =>
				{
					// Eg aaaaa-bbb-z-y-x-123[abxyz]
					var (info, id) = line.RxMatch("%*-%d").Get<string, int>();
					return DecryptName(info, id) == roomname ? id : 0;
				})
				.First(x => x != 0);

			return sectorId;

			static string DecryptName(string s, int n)
			{
				var shift = n % 26;
				return new string(s.Select(DecryptChar).ToArray());
				char DecryptChar(char c) =>
					c == '-' ? ' ' :
					c + shift <= 'z' ? (char)(c + shift) :
					(char)(c + shift - 26);
			}
		}
	}
}
