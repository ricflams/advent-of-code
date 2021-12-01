using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2021.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "DAY1";
		public override int Year => 2021;
		public override int Day => 1;

		public void Run()
		{
			Run("test1").Part1(0).Part2(0);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
		}

		protected override int Part1(string[] input)
		{
			var vs = input.Select(int.Parse).ToArray();

			var n = 0;
			var last = (int?)null;
			for (var i = 0; i < vs.Length; i++)
			{
				var x = 0;
				if (i == 0)
					x = vs[0];
				else if (i == 1)
					x = vs[0] + vs[1];
				else if (i == vs.Length - 1)
					x = vs[vs.Length - 1];
				else if (i == vs.Length-2)
					x = vs[vs.Length - 1] + vs[vs.Length-2];
				else
					x = vs[i] + vs[i + 1] + vs[i + 2];

				//if (last != null)
				{
					if (x > last)
						n++;
				}
				last = x;
			}


			return n;
		}

		protected override int Part2(string[] input)
		{





			return 0;
		}
	}
}
