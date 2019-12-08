using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AdventOfCode2019.Intcode
{
	internal class Engine
	{
		private static readonly IDictionary<int, Instruction> Instructions = new Dictionary<int, Instruction>
		{
			{
				1, new Instruction.WithOp1Op2Dest { Name = "add", Execute = (engine, op1, op2, dest) =>
					{
						engine.Memory[dest] = op1 + op2;
					}
				}
			},
			{
				2, new Instruction.WithOp1Op2Dest { Name = "multiply", Execute = (engine, op1, op2, dest) =>
					{
						engine.Memory[dest] = op1 * op2;
					}
				}
			},
			{
				3, new Instruction.WithDest { Name = "get", Execute = (engine, dest) =>
					{
						engine.Memory[dest] = engine.Input.Take();
					}
				}
			},
			{
				4, new Instruction.WithOp1 { Name = "put", Execute = (engine, op) =>
					{
						engine.Output.Add(op);
					}
				}
			},
			{
				5, new Instruction.WithOp1Op2 { Name = "jump-if-true", Execute = (engine, op1, op2) =>
					{
						if (op1 != 0)
						{
							engine.Pc = op2;
						}
					}
				}
			},
			{
				6, new Instruction.WithOp1Op2 { Name = "jump-if-false", Execute = (engine, op1, op2) =>
					{
						if (op1 == 0)
						{
							engine.Pc = op2;
						}
					}
				}
			},
			{
				7, new Instruction.WithOp1Op2Dest { Name = "less-than", Execute = (engine, op1, op2, dest) =>
					{
						engine.Memory[dest] = op1 < op2 ? 1 : 0;
					}
				}
			},
			{
				8, new Instruction.WithOp1Op2Dest { Name = "equals", Execute = (engine, op1, op2, dest) =>
					{
						engine.Memory[dest] = op1 == op2 ? 1 : 0;
					}
				}
			},
			{
				99,
				new Instruction.WithNoOp { Name = "halt", Execute = engine =>
					{
						engine.Halt = true;
					}
				}
			}
		};

		public BlockingCollection<int> Input { get; set; } = new BlockingCollection<int>();
		public BlockingCollection<int> Output { get; set; } = new BlockingCollection<int>();

		public int[] Memory = new int[] { 99 };

		public Engine WithMemory(int[] memory)
		{
			Memory = (int[])memory.Clone();
			return this;
		}

		public Engine WithInput(params int[] input)
		{
			foreach (var value in input)
			{
				Input.Add(value);
			}
			return this;
		}

		public string TakeOutput()
		{
			var output = new List<int>();
			while (Output.TryTake(out var value))
			{
				output.Add(value);
			}
			return string.Join(" ", output);
		}

		public bool Halt;
		public int Pc;

		public void Execute()
		{
			Halt = false;
			Pc = 0;

			while (!Halt)
			{
				var opcode = Memory[Pc++];
				var instruction = Instructions[opcode % 100];
				switch (instruction)
				{
					case Instruction.WithNoOp op:
						op.Execute(this);
						break;
					case Instruction.WithOp1 op:
						op.Execute(this, GetOperand1(opcode));
						break;
					case Instruction.WithOp1Op2 op:
						op.Execute(this, GetOperand1(opcode), GetOperand2(opcode));
						break;
					case Instruction.WithDest op:
						op.Execute(this, GetPosOperand());
						break;
					case Instruction.WithOp1Op2Dest op:
						op.Execute(this, GetOperand1(opcode), GetOperand2(opcode), GetPosOperand());
						break;
				}
			}
		}

		private int GetOperand1(int opcode) => opcode / 100 % 10 == 1 ? Memory[Pc++] : Memory[Memory[Pc++]];
		private int GetOperand2(int opcode) => opcode / 1000 % 10 == 1 ? Memory[Pc++] : Memory[Memory[Pc++]];
		private int GetPosOperand() => Memory[Pc++];
	}
}
