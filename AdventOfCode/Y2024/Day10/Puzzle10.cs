using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day10
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Hoof It";
		public override int Year => 2024;
		public override int Day => 10;

		public override void Run()
		{
			Run("test1").Part1(1);
			Run("test2").Part1(2);
			Run("test3").Part1(4);
			Run("test4").Part1(3);
			Run("test5").Part1(36).Part2(81);
			Run("test6").Part2(3);
			Run("test7").Part2(13);
			Run("test8").Part2(227);
			Run("input").Part1(744).Part2(1651);
			Run("extra").Part1(825).Part2(1805);
		}

		protected override long Part1(string[] input)
		{
			var map = new CharMap(input);

			var score = 0;
			foreach (var start in map.AllPointsWhere(c => c == '0'))
			{
				var seen9 = new HashSet<Point>();
				TrailheadScore(seen9, start, '0');
				score += seen9.Count;
			}

			return score;

			void TrailheadScore(HashSet<Point> seen9, Point p0, char ch)
			{
				if (ch == '9')
				{
					seen9.Add(p0);
					return;
				}
				var next = (char)(ch + 1);
				foreach (var p in p0.LookAround().Where(p => map[p] == next))
				{
					TrailheadScore(seen9, p, next);
				}
			}			
		}

		protected override long Part2(string[] input)
		{
			var map = new CharMap(input);

			var score = map.AllPointsWhere(c => c == '0').Sum(p => TrailheadScore(p, '0'));

			return score;

			int TrailheadScore(Point p0, char ch)
			{
				if (ch == '9')
					return 1;
				var next = (char)(ch + 1);
				return p0.LookAround().Where(p => map[p] == next).Sum(p => TrailheadScore(p, next));
			}				
		}
	}
}
