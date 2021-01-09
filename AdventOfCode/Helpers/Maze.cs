using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class Maze
    {
		public CharMap Map { get; set; }
		public Point Entry { get; set; }
		
		public Maze(CharMap map)
		{
			Map = map;
		}

		public IEnumerable<Point> ExternalMapPoints = Enumerable.Empty<Point>();

		public virtual Point Transform(Point p) => p;
		public virtual bool IsWalkable(Point p) => Map[p] != '#';
		public virtual bool IsFork(Point p) => false;
	
		public void ConsoleWrite()
		{
			foreach (var line in Map.Render())
			{
				Console.WriteLine(line);
			}
		}
	}
}
