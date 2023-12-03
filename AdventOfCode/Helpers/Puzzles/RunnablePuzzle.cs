namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class RunnablePuzzle : IPuzzle
	{
		public abstract string Name { get; }
		public abstract int Year { get; }
		public abstract int Day { get; }
		public abstract void Run();
		public PuzzleOptions Options { get; set; }
	}
}
