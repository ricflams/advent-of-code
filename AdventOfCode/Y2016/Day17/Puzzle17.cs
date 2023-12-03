using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace AdventOfCode.Y2016.Day17
{
	internal class Puzzle : Puzzle<string, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Two Steps Forward";
		public override int Year => 2016;
		public override int Day => 17;

		public override void Run()
		{
			Run("test1").Part1("DDRRRD").Part2(370);
			Run("test2").Part1("DDUDRLRRUDRD").Part2(492);
			Run("test3").Part1("DRURDRUDDLLDLUURRDULRLDUUDDDRR").Part2(830);
			Run("input").Part1("DUDRDLRRRD").Part2(502);
		}

		protected override string Part1(string[] input)
		{
			var passcode = input[0];
			var shortest = FindPath(passcode, true);
			return shortest;
		}

		protected override int Part2(string[] input)
		{
			var passcode = input[0];
			var longest = FindPath(passcode, false);
			return longest.Length;
		}

		private string FindPath(string passcode, bool findShortest)
		{
			var hashMemo = new Dictionary<string, byte[]>();
			var md5 = MD5.Create();
			var dest = Point.From(3, 3);

			return ShortestPath(Point.Origin, "");

			string ShortestPath(Point p, string path)
			{
				if (p == dest)
					return path;
				var hash = HashOf(passcode + path);
				var moves = Moves(p, hash)
					.Select(m => ShortestPath(m.Item1, path + m.Item2))
					.Where(x => x != null);
				return findShortest
					? moves.OrderBy(x => x.Length).FirstOrDefault()
					: moves.OrderByDescending(x => x.Length).FirstOrDefault();

				byte[] HashOf(string s)
				{
					if (!hashMemo.TryGetValue(s, out var hash))
					{
						hash = md5.ComputeHash(Encoding.ASCII.GetBytes(s));
					}
					return hash;
				}

				static IEnumerable<(Point,char)> Moves(Point p, byte[] hash)
				{
					if (IsOpen(hash[0] >>4) && p.Y > 0) yield return (p.Up, 'U');
					if (IsOpen(hash[0]&0xf) && p.Y < 3) yield return (p.Down, 'D');
					if (IsOpen(hash[1] >>4) && p.X > 0) yield return (p.Left, 'L');
					if (IsOpen(hash[1]&0xf) && p.X < 3) yield return (p.Right, 'R');
				
					static bool IsOpen(int ch) => ch >= 0xb;
				}
			}
		}
	}
}
