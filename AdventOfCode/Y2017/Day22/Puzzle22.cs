using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2017.Day22
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Sporifica Virus";
		public override int Year => 2017;
		public override int Day => 22;

		public override void Run()
		{
			Run("test1").Part1(5587).Part2(2511944);
			Run("input").Part1(5460).Part2(2511702);
			Run("extra").Part1(5322).Part2(2512079);
		}

		protected override int Part1(string[] input)
		{
			var map = CharMap.FromArray(input, '.');

			var (_, p2) = map.MinMax();
			var p = Pose.From(p2.X/2, p2.Y/2, Direction.Up);

			var infections = 0;
			for (var i = 0; i < 10000; i++)
			{
				// If the current node is infected, it turns to its right. Otherwise, it turns to its left. (Turning is done in-place; the current node does not change.)
				// If the current node is clean, it becomes infected. Otherwise, it becomes cleaned. (This is done after the node is considered for the purposes of changing direction.)
				// The virus carrier moves forward one node in the direction it is facing.
				if (map[p.Point] == '#')
					p.TurnRight();
				else
					p.TurnLeft();
				if (map[p.Point] == '.')
				{
					map[p.Point] = '#';	
					infections++;
				}
				else
				{
					map[p.Point] = '.';	
				}
				p.Move(1);
			}

			return infections;
		}

		protected override int Part2(string[] input)
		{
			var inputMap = CharMap.FromArray(input);

			// Use a 2D array for fastest storage (faster than CharMap/sparse array
			// or Dictionary). Trials show that x,y stray <300 steps from origin so
			// that'll do with a bit of margin. Shift all x,y coordinates into positive
			// numbers by xyOffset so x,y can index the char[,]-map directly.
			var xyOffset = 500;
			var map = new char[xyOffset*2, xyOffset*2];

			// Populate the map with all known infections
			foreach (var (p, ch) in inputMap.AllWhere(c => c == '#'))
			{
				map[p.X + xyOffset, p.Y + xyOffset] = '#';
			}

			// Initial x,y is at the centre of the map
			var (_, size) = inputMap.MinMax();
			var x = (uint)size.X/2 + xyOffset;
			var y = (uint)size.Y/2 + xyOffset;

			// Setup fast lookups for determining dx/dy-movements and turns,
			// using the current direction as index.
			// This is faster than a switch or PointWithDirection.
			var (up, right, down, left) = (0, 1, 2, 3);
			var dx = new [] { 0, 1, 0, -1 };
			var dy = new [] { -1, 0, 1, 0 };
			var turnleft = new [] { left, up, right, down };
			var turnright = new [] { right, down, left, up };
			var turnaround = new [] { down, left, up, right };

			int dir = up;
			var infections = 0;
			for (var i = 0; i < 10_000_000; i++)
			{
				// Clean nodes become weakened.
				// Weakened nodes become infected.
				// Infected nodes become flagged.
				// Flagged nodes become clean.				
				// Decide which way to turn based on the current node:
				//   If it is clean, it turns left.
				//   If it is weakened, it does not turn, and will continue moving in the same direction.
				//   If it is infected, it turns right.
				//   If it is flagged, it reverses direction, and will go back the way it came.
				// Modify the state of the current node, as described above.
				// The virus carrier moves forward one node in the direction it is facing.
				switch (map[x, y])
				{
					case '\0': map[x,y] = 'w'; dir = turnleft[dir]; break;
					case 'w': map[x,y] = '#'; infections++; break;
					case '#': map[x,y] = 'f'; dir = turnright[dir]; break;
					case 'f': map[x,y] = '\0'; dir = turnaround[dir]; break;
				}
				x += dx[dir];
				y += dy[dir];
			}

			return infections;
		}
	}
}
