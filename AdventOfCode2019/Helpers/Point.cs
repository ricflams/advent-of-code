using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Helpers
{
    internal class Point
    {
		public int X { get; set; }
		public int Y { get; set; }

		static public Point From(int x, int y) => new Point { X = x, Y = y };
	}
}
