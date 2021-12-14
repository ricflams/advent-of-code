using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day17
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Reservoir Research";
		public override int Year => 2018;
		public override int Day => 17;

		public void Run()
		{
			Run("test1").Part1(57).Part2(29);
			Run("input").Part1(31883).Part2(24927);
		}

		protected override int Part1(string[] input)
		{
			var reservoir = new Reservoir(input);
			reservoir.Fill();
			return reservoir.RunningWater + reservoir.SteadyWater;
		}

		protected override int Part2(string[] input)
		{
			var reservoir = new Reservoir(input);
			reservoir.Fill();
			return reservoir.SteadyWater;
		}
	}

	internal class Reservoir
	{
		private readonly Point _spring = Point.From(500, 0);
		private readonly CharMap _map = new('.');
		private readonly Point _min, _max;

		public Reservoir(string[] input)
		{
			foreach (var line in input)
			{
				// Example: x=495, y=2..7
				var (name, val1, _, val21, val22) = line.RxMatch("%c=%d, %c=%d..%d").Get<char, int, char, int, int>();
				if (name == 'x')
				{
					var x = val1;
					for (var y = val21; y <= val22; y++)
					{
						_map[x][y] = '#';
					}
				}
				else
				{
					var y = val1;
					for (var x = val21; x <= val22; x++)
					{
						_map[x][y] = '#';
					}
				}
			}
			(_min, _max) = _map.MinMax();

			_map[_spring] = '+';
		}

		public int RunningWater => _map.Count('|');
		public int SteadyWater => _map.Count('~');

		public void Fill()
		{
			Fill(_spring);
		}

		private void Fill(Point p)
		{
			if (p.Y == _max.Y)
				return;

			if (!_map[p.Down].IsSurface())
			{
				// Only fill with running water above the min Y position
				if (p.Down.Y >= _min.Y)
				{
					_map[p.Down] = '|';
				}

				Fill(p.Down);
			}

			if (_map[p.Down].IsSurface())
			{
				if (_map[p.Right] == '.')
				{
					_map[p.Right] = '|';
					Fill(p.Right);
				}

				if (_map[p.Left] == '.')
				{
					_map[p.Left] = '|';
					Fill(p.Left);
				}
			}

			if (_map[p.Left] == '#')
			{
				// If the running water is bounded by clay at each side then re-mark is as steady
				var edge = p.Right;
				while (_map[edge] == '|' && _map[edge.Down].IsSurface())
					edge = edge.Right;
				if (_map[edge] == '#')
				{
					// Yes, bounded, so so re-mark as steady
					for (var steady = p; steady != edge; steady = steady.Right)
					{
						_map[steady] = '~';
					}
				}
			}
		}
	}

	static class Extensions
	{
		public static bool IsSurface(this char ch) => ch is '#' or '~';
	}
}
