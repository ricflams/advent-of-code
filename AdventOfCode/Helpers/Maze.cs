namespace AdventOfCode.Helpers
{
	public class Maze(CharMap map)
	{
		public CharMap Map { get; init; } = map;
		public Point Entry { get; set; }
		public Point Exit { get; set; }

		public Maze WithEntry(Point p)
		{
			Entry = p;
			return this;
		}

		public Maze WithExit(Point p)
		{
			Exit = p;
			return this;
		}

		public virtual Point Teleport(Point p) => p;
		public bool IsWalkable(Point p) => Map[p] != '#';
		public bool IsFork(Point p) => false;
	}
}
