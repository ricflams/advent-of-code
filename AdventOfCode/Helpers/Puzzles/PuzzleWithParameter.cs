namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class PuzzleWithParameter<TP, T1, T2> : Puzzle<T1, T2>
	{
		protected TP PuzzleParameter { get; set; }

		public new PuzzleWithParameterRunner Run(string testname)
		{
			return new PuzzleWithParameterRunner(this, testname, testname);
		}

		internal class PuzzleWithParameterRunner : Runner
		{
			private readonly PuzzleWithParameter<TP, T1, T2> _puzzleWithParameter;

			public PuzzleWithParameterRunner(PuzzleWithParameter<TP, T1, T2> puzzle, string testname, string filename)
			 : base(puzzle, testname, filename)
				 => _puzzleWithParameter = puzzle;

			public Runner WithParameter(TP param)
			{
				_puzzleWithParameter.PuzzleParameter = param;
				return this;
			}
		}
	}
}
