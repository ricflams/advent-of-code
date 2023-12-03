using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day07
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Some Assembly Required";
		public override int Year => 2015;
		public override int Day => 7;

		public override void Run()
		{
			Run("input").Part1(46065).Part2(14134);
		}

		protected override int Part1(string[] input)
		{
			var gates = new Gates(input);

			while (gates["a"].Output == null)
			{
				gates.EmulateAllGates();
			}
			var signal = gates["a"].Output.Value;
			return signal;
		}

		protected override int Part2(string[] input)
		{
			var gates = new Gates(input);
			var signal1 = Part1(input);

			gates["b"].Input1 = signal1.ToString();
			gates.ResetAllGates();
			while (gates["a"].Output == null)
			{
				gates.EmulateAllGates();
			}
			var signal2 = gates["a"].Output.Value;
			return signal2;
		}

		private class Gates : Dictionary<string, Gates.Gate>
		{
			public Gates(IEnumerable<string> wirings)
			{
				foreach (var line in wirings)
				{
					var gate = ParseGate(line);
					this[gate.Name] = gate;
				}
			}

			public void EmulateAllGates()
			{
				//Console.WriteLine($"Emulating {gates.Values.Count(g => g.Output == null)} gates:");
				foreach (var gate in Values.Where(g => g.Output == null))
				{
					gate.Emulate(this);
					//Console.WriteLine($"  Emulate {gate.Name}: {gate.Output}");
				}
			}

			public void ResetAllGates()
			{
				foreach (var gate in Values)
				{
					gate.Output = null;
				}
			}

			public abstract class Gate
			{
				public string Name { get; set; }
				public string Input1 { get; set; }
				public string Input2 { get; set; }
				public ushort? Output { get; set; }
				public abstract void Emulate(Gates gates);
				protected ushort? Input(Gates gates, string input)
				{
					if (gates.TryGetValue(input, out var gate))
					{
						return gate.Output;
					}
					if (ushort.TryParse(input, out var value))
					{
						return value;
					}
					throw new Exception($"Unexpected input for {Name}: {input}");
				}
			}

			public class ValueGate : Gate
			{
				public override void Emulate(Gates gates)
				{
					var input = Input(gates, Input1);
					Output = input;
				}
			}

			public class NotGate : Gate
			{
				public override void Emulate(Gates gates)
				{
					var input = Input(gates, Input1);
					if (input != null)
					{
						Output = (ushort)~input;
					}
				}
			}

			public class AndGate : Gate
			{
				public override void Emulate(Gates gates)
				{
					var input1 = Input(gates, Input1);
					var input2 = Input(gates, Input2);
					if (input1 != null && input2 != null)
					{
						Output = (ushort)(input1 & input2);
					}
				}
			}

			public class OrGate : Gate
			{
				public override void Emulate(Gates gates)
				{
					var input1 = Input(gates, Input1);
					var input2 = Input(gates, Input2);
					if (input1 != null && input2 != null)
					{
						Output = (ushort)(input1 | input2);
					}
				}
			}

			public class LShiftGate : Gate
			{
				public override void Emulate(Gates gates)
				{
					var input1 = Input(gates, Input1);
					var input2 = Input(gates, Input2);
					if (input1 != null && input2 != null)
					{
						Output = (ushort)(input1 << input2);
					}
				}
			}

			public class RShiftGate : Gate
			{
				public override void Emulate(Gates gates)
				{
					var input1 = Input(gates, Input1);
					var input2 = Input(gates, Input2);
					if (input1 != null && input2 != null)
					{
						Output = (ushort)(input1 >> input2);
					}
				}
			}

			private Gate ParseGate(string operation)
			{
				// Examples:
				// 123 -> x
				// z -> p
				// x AND y -> d
				// x OR y -> e
				// x LSHIFT 2 -> f
				// y RSHIFT 2 -> g
				// NOT y -> i
				if (operation.IsRxMatch("NOT %s -> %s", out var captures))
				{
					var (input1, name) = captures.Get<string, string>();
					return new NotGate { Input1 = input1, Name = name };
				}
				if (operation.IsRxMatch("%s AND %s -> %s", out captures))
				{
					var (input1, input2, name) = captures.Get<string, string, string>();
					return new AndGate { Input1 = input1, Input2 = input2, Name = name };
				}
				if (operation.IsRxMatch("%s OR %s -> %s", out captures))
				{
					var (input1, input2, name) = captures.Get<string, string, string>();
					return new OrGate { Input1 = input1, Input2 = input2, Name = name };
				}
				if (operation.IsRxMatch("%s LSHIFT %d -> %s", out captures))
				{
					var (input1, input2, name) = captures.Get<string, string, string>();
					return new LShiftGate { Input1 = input1, Input2 = input2, Name = name };
				}
				if (operation.IsRxMatch("%s RSHIFT %d -> %s", out captures))
				{
					var (input1, input2, name) = captures.Get<string, string, string>();
					return new RShiftGate { Input1 = input1, Input2 = input2, Name = name };
				}
				if (operation.IsRxMatch("%s -> %s", out captures))
				{
					var (input1, name) = captures.Get<string, string>();
					return new ValueGate { Input1 = input1, Name = name };
				}

				throw new Exception($"Unexpected input in line {operation}");
			}
		}
	}
}
