using System;
using System.Linq;

namespace AdventOfCode.Helpers.Puzzles
{
	internal class PuzzleRunner
	{
		public PuzzleOptions Options { get; } = new PuzzleOptions();

		public void RunAll()
		{
				// Get all loaded assemblies
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			// Find and instantiate all RunnablePuzzle
			var puzzles = assemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(RunnablePuzzle)))
				.Where(t => !t.Namespace.EndsWith(".Raw"))
				.Select(t =>
				{
					var puzzle = Activator.CreateInstance(t) as RunnablePuzzle;
					puzzle.Options = Options;
					return puzzle;
				})
				.OrderBy(x => x.Year)
				.ThenBy(x => x.Day)
				.ToArray();

			// Run all puzzles; if there's a filter then the puzzle with exclude itself
			foreach (var puzzle in puzzles)
			{
				puzzle.Run();
			}
		}
	}
}
