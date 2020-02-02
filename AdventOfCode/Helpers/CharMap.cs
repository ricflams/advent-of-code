using System;
using System.IO;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class CharMap : SparseMap<char>
	{
		public CharMap(char defaultValue = default(char))
			: base(defaultValue)
		{
		}

		public static CharMap FromFile(string filename)
		{
			var input = File.ReadAllLines(filename);
			return FromArray(input);
		}

		public static CharMap FromArray(string[] lines)
		{
			var map = new CharMap();
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					map[x][y] = line[x];
				}
			}
			return map;
		}

		public void ConsoleWrite(bool clear, params string[] headers)
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

		public string[] Render(Func<Point, char, char> rendering = null)
		{
			var (min, max) = Area();
			return Enumerable.Range(min.Y, max.Y- min.Y + 1)
				.Select(y => Enumerable.Range(min.X, max.X - min.X + 1)
					.Select(x => rendering != null ? rendering(Point.From(x, y), this[x][y]) : this[x][y])
					.ToArray()
				)
				.Select(ch => new string(ch))
				.ToArray();
		}
	}
}
