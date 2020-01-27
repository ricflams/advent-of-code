using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2015.Day07
{
	internal class Puzzle07
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2015/Day07/input.txt");
			var gates = new Gates(input);

			while (gates["a"].Output == null)
			{
				gates.EmulateAllGates();
			}
			var signal1 = gates["a"].Output.Value;
			Console.WriteLine($"Day  7 Puzzle 1: {signal1}");
			Debug.Assert(signal1 == 46065);

			gates["b"].Input1 = signal1.ToString();
			gates.ResetAllGates();
			while (gates["a"].Output == null)
			{
				gates.EmulateAllGates();
			}
			var signal2 = gates["a"].Output.Value;
			Console.WriteLine($"Day  7 Puzzle 2: {signal2}");
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
				bool TryParse(string pattern, out string[] groups)
				{
					var match = Regex.Match(operation, pattern);
					if (!match.Success)
					{
						groups = null;
						return false;
					}
					groups = match.Groups.Skip(1).Select(g => g.Value).ToArray();
					return true;
				}

				// Examples:
				// 123 -> x
				// z -> p
				// x AND y -> d
				// x OR y -> e
				// x LSHIFT 2 -> f
				// y RSHIFT 2 -> g
				// NOT y -> i
				string[] op;
				if (TryParse(@"NOT (\w+) -> (\w+)", out op))
				{
					return new NotGate { Input1 = op[0], Name = op[1] };
				}
				if (TryParse(@"(\w+) AND (\w+) -> (\w+)", out op))
				{
					return new AndGate { Input1 = op[0], Input2 = op[1], Name = op[2] };
				}
				if (TryParse(@"(\w+) OR (\w+) -> (\w+)", out op))
				{
					return new OrGate { Input1 = op[0], Input2 = op[1], Name = op[2] };
				}
				if (TryParse(@"(\w+) LSHIFT (\d+) -> (\w+)", out op))
				{
					return new LShiftGate { Input1 = op[0], Input2 = op[1], Name = op[2] };
				}
				if (TryParse(@"(\w+) RSHIFT (\d+) -> (\w+)", out op))
				{
					return new RShiftGate { Input1 = op[0], Input2 = op[1], Name = op[2] };
				}
				if (TryParse(@"(\w+) -> (\w+)", out op))
				{
					return new ValueGate { Input1 = op[0], Name = op[1] };
				}

				throw new Exception($"Unexpected input in line {operation}");
			}
		}









		//private static Gate ParseGate(Gates gates, string operation)
		//{
		//	// Examples:
		//	// 123 -> x
		//	// x AND y -> d
		//	// x OR y -> e
		//	// x LSHIFT 2 -> f
		//	// y RSHIFT 2 -> g
		//	// NOT y -> i

		//	bool TryParse(string pattern, out string[] groups)
		//	{
		//		var match = Regex.Match(operation, pattern);
		//		if (!match.Success)
		//		{
		//			groups = null;
		//			return false;
		//		}
		//		groups = match.Groups.Skip(1).Select(g => g.Value).ToArray();
		//		return true;
		//	}

		//	T GetOrCreateGate<T>(string name, Func<string, T> factory) where T : Gate
		//	{
		//		if (!gates.TryGetValue(name, out var gate))
		//		{
		//			gate = factory(name);
		//			gates.Add(name, gate);
		//		}
		//		return gate as T;
		//	}

		//	if (TryParse(@"NOT (\w+) -> (\w+)", out var not))
		//	{
		//		var (op1name, destname) = (not[0], not[1]);

		//		if (!gates.TryGetValue(op1name, out var op1))
		//		{
		//			op1 = new UnaryGate { Name = op1name, GateTypez = Gate.GateType.Not };
		//			gates.Add(op1);
		//		}
		//		if (!gates.TryGetValue(op1name, out var op1))
		//		{
		//			op1 = new UnaryGate { Name = op1name, GateTypez = Gate.GateType.Not };
		//			gates.Add(op1);
		//		}
		//		return new Gate
		//		{
		//			Kind = Type.Not,
		//			Op1 = not[0],
		//			Dest = not[1]
		//		};
		//	}

		//	if (TryParse(@"(\w+) (\w+) (\w+) -> (\w+)", out var bin))
		//	{
		//		return new Gate
		//		{
		//			Kind = Gate.BinaryOperands[bin[1]],
		//			Op1 = bin[0],
		//			Op2 = bin[2],
		//			Dest = bin[3]
		//		};
		//	}

		//	if (TryParse(@"(\d+) -> (\w+)", out var literal))
		//	{
		//		return new Gate
		//		{
		//			Kind = Type.Literal,
		//			Op1 = literal[0],
		//			Dest = literal[1]
		//		};
		//	}

		//	if (TryParse(@"(\w+) -> (\w+)", out var copy))
		//	{
		//		return new Gate
		//		{
		//			Kind = Type.Copy,
		//			Op1 = copy[0],
		//			Dest = copy[1]
		//		};
		//	}


		//	throw new Exception($"Unexpected input in line {operation}");

		//}


	}
}
