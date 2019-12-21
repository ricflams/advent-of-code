using System;
using System.Linq;

namespace AdventOfCode2019.Helpers
{
	internal class CharMap : SparseMap<char>
	{
		public CharMap(char defaultValue = default(char))
			: base(defaultValue)
		{
		}

		public void ConsoleWrite(params string[] headers)
		{
			Console.Clear();
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
			var points = AllPoints().ToList();
			var xMin = points.Min(z => z.X);
			var xMax = points.Max(z => z.X);
			var yMin = points.Min(z => z.Y);
			var yMax = points.Max(z => z.Y);
			return Enumerable.Range(yMin, yMax - yMin + 1)
				.Select(y => Enumerable.Range(xMin, xMax - xMin + 1)
					.Select(x => rendering != null ? rendering(Point.From(x, y), this[x][y]) : this[x][y])
					.ToArray()
				)
				.Select(ch => new string(ch))
				.ToArray();
		}
	}
}
