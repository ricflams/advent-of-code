using System;
using System.Linq;

namespace AdventOfCode2019.Helpers
{
	public class CharMap : SparseMap<char>
	{
		public CharMap(char defaultValue = default(char))
			: base(defaultValue)
		{
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
