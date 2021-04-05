using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2018;
		public override int Day => 15;

		public void Run()
		{
			Run("test1").Part1(36334);
			Run("test1").Part1(39514);
			Run("test1").Part1(27755);
			Run("test1").Part1(28944);
			Run("test1").Part1(18740);
			Run("input").Part1(371284).Part2(0);
		}

		protected override int Part1(string[] input)
		{





			return 0;
		}

		protected override int Part2(string[] input)
		{





			return 0;
		}

		internal class Combat
		{
			private readonly CharMap _map;
			private readonly List<Unit> _elfs;
			private readonly List<Unit> _gobs;

			public Combat(string[] input)
			{
				_map = CharMap.FromArray(input);

				_elfs = _map.AllPoints(c => c == 'E').Select(p => new Unit(p)).ToList();
				_gobs = _map.AllPoints(c => c == 'G').Select(p => new Unit(p)).ToList();

				Completed = false;
			}

			internal class Unit
			{
				public Unit(Point p) => (Position, Hitpoints) = (p, 200);
				public Point Position { get; set; }
				public int Hitpoints { get; set; }
			}

			public void BattleRound()
			{
				
			}

			public bool Completed { get; private set; }
		}
	}
}
