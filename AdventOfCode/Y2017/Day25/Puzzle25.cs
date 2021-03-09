using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2017.Day25
{
	internal class Puzzle : Puzzle<int, bool>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Halting Problem";
		public override int Year => 2017;
		public override int Day => 25;

		public void Run()
		{
			RunPart1For("test1", 3);
			RunPart1For("input", 4385);
		}

		protected override int Part1(string[] input)
		{
			var turingMachine = new TuringMachine(input);
			var checksum = turingMachine.CalculateDiagnosticsChecksum();
			return checksum;
		}

		protected override bool Part2(string[] input) => true;


		internal class TuringMachine
		{
			private readonly State[] _states;
			private readonly int _steps;
			private int _state;

			public TuringMachine(string[] input)
			{
				// Begin in state A.
				// Perform a diagnostic checksum after 12386363 steps.
				_state = input[0].RxMatch("Begin in state %c").Get<char>() - 'A';
				_steps = input[1].RxMatch("Perform a diagnostic checksum after %d steps.").Get<int>();
				_states = input.Skip(3)
					.GroupByEmptyLine()
					.Select(g => new State(g))
					.ToArray();
			}

			public int CalculateDiagnosticsChecksum()
			{
				// var tape = new SafeDictionary<int, int>();
				// Nah, just use a plain array that is big enough - it's 3x faster
				var N = 20000;
				var cursor = N / 2;
				var tape = new int[N];

				var state = _state;
				for (var i = 0; i < _steps; i++)
				{
					var x = _states[state].Transitions[tape[cursor]];
					tape[cursor] = x.ValueToWrite;
					cursor += x.Movement;
					state = x.NextState;
				}
				var checksum = tape.Count(x => x == 1);
				return checksum;
			}

			internal class State
			{
				public State(string[] def)
				{
					// 0:  In state A:
					// 1:    If the current value is 0:
					// 2:      - Write the value 1.
					// 3:      - Move one slot to the right.
					// 4:      - Continue with state B.
					// 5:    If the current value is 1:
					// 6:      - Write the value 0.
					// 7:      - Move one slot to the left.
					// 8:      - Continue with state E.
					//
					// The first transition is always 0, second is 1.
					Transitions = new Transition[]
					{
						new Transition(def[2..5]),
						new Transition(def[6..9])
					};
				}
				public Transition[] Transitions { get; }

				internal class Transition
				{
					public Transition(string[] def)
					{
						ValueToWrite = def[0].RxMatch("value %d").Get<int>();
						Movement = def[1].Contains("right") ? 1 : -1;
						NextState = def[2].RxMatch("state %c").Get<char>() - 'A';
					}
					public int ValueToWrite { get; set; }
					public int Movement { get; set; }
					public int NextState { get; set; }
				}
			}
		}
	}
}
