using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class CharMap : SparseMap<char>
	{
		public CharMap(char defaultValue = default)
			: base(defaultValue)
		{
		}

		public CharMap(string[] lines, char defaultValue = default)
			: base(defaultValue)
		{
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					this[x][y] = line[x];
				}
			}			
		}

		public static CharMap FromArray(string[] lines, char defaultValue = default)
		{
			return new CharMap(lines, defaultValue);
		}

		public void ConsoleWrite(bool clear = false, params string[] headers)
		{
			if (clear)
			{
				Console.Clear();
			}
			foreach (var header in headers)
			{
				Console.WriteLine(header);
			}
			foreach (var line in Render())
			{
				Console.WriteLine(line);
			}
		}

		public new string[] Render(Func<Point, char, char> rendering = null)
		{
			var (min, max) = MinMax();
			return Enumerable.Range(min.Y, max.Y- min.Y + 1)
				.Select(y => Enumerable.Range(min.X, max.X - min.X + 1)
					.Select(x => rendering != null ? rendering(Point.From(x, y), this[x][y]) : this[x][y])
					.ToArray()
				)
				.Select(ch => new string(ch))
				.ToArray();
		}

		public CharMap Copy()
		{
			var map = new CharMap(_defaultValue);
			foreach (var p in AllPoints())
			{
				map[p] = this[p];
			}
			return map;
		}

		public void Map(Func<Point, char, char> transform)
		{
			foreach (var p in AllPoints())
			{
				this[p] = transform(p, this[p]);
			}
		}

		public void Map(Func<char, char> transform)
		{
			foreach (var p in AllPoints())
			{
				this[p] = transform(this[p]);
			}
		}

		public CharMap Transform(Func<Point, char, char> transform)
		{
			var map = new CharMap(_defaultValue);
			foreach (var p in AllPoints())
			{
				map[p] = transform(p, this[p]);
			}
			return map;
		}

		public CharMap TransformAutomata(Func<Point, IEnumerable<Point>> findAdjacents, Func<Point, char, int, char> transform)
		{
			// Combine all points' neighbourhoods for the total set of points to transform
			var neighbourhood = new CharMap();
			var active = AllPointsWhere(ch => ch != _defaultValue);
			foreach (var p in active)
			{
				neighbourhood[p] = '+';
				foreach (var adj in findAdjacents(p))
				{
					neighbourhood[adj] = '+';
				}
			}

			var map = new CharMap(_defaultValue);
			foreach (var p in neighbourhood.AllPoints())
			{
				var adjcount = findAdjacents(p).Count(pos => this[pos] != _defaultValue);
				var ch = transform(p, this[p], adjcount);
				if (ch != _defaultValue)
				{
					map[p] = ch;
				}
			}

			return map;
		}

		public int Count(char ch)
		{
			var sum = 0;
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					if (column.Row[y] == ch)
					{
						sum++;
					}
				}
			}
			return sum;
		}

		public CharMap ResetToOrigin()
		{
			var (min, _) = MinMax();
			var map = new CharMap(_defaultValue);
			foreach (var p in AllPoints())
			{
				map[p - min] = this[p];
			}
			return map;
		}

	}
}
