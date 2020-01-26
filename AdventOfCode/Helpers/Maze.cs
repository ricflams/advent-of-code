using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Helpers
{
    public class Maze
    {
		public CharMap Map { get; protected set; }

		public Point Entry { get; protected set; }
		public IEnumerable<Point> ExternalMapPoints = Enumerable.Empty<Point>();

		public virtual Point Transform(Point p) => p;
		public virtual bool IsWalkable(Point p) => Map[p] != '#';
		public virtual bool IsFork(Point p) => false;

		public static CharMap ReadMapFromFile(string filename)
		{
			var map = new CharMap();
			var lines = File.ReadAllLines(filename);
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

		public void WriteMap()
		{
			foreach (var line in Map.Render())
			{
				Console.WriteLine(line);
			}
		}
	}
}
