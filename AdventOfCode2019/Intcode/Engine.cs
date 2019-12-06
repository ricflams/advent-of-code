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
				3,
				new Instruction { Name = "get", Execute = (mem, data, _, pc) =>
					{
						var op = mem[pc++];
						mem[op] = data.Input[0];
						data.Input.RemoveAt(0);
						return pc;
					}
				}
			},
			{
				4,
				new Instruction { Name = "put", Execute = (mem, data, mode, pc) =>
					{
						var op = mem[pc++];
						data.Output.Add(mode.Param1IsImmediate ? op : mem[op]);
						return pc;
					}
				}
			},
			{
				5,
				new Instruction { Name = "jump-if-true", Execute = (mem, data, mode, pc) =>
					{
						var op1 = mem[pc++];
						var op2 = mem[pc++];
						if ((mode.Param1IsImmediate ? op1 : mem[op1]) != 0)
						{
							return mode.Param2IsImmediate ? op2 : mem[op2];
						}
						return pc;
					}
				}
			},
			{
				6,
				new Instruction { Name = "jump-if-false", Execute = (mem, data, mode, pc) =>
					{
						var op1 = mem[pc++];
						var op2 = mem[pc++];
						if ((mode.Param1IsImmediate ? op1 : mem[op1]) == 0)
						{
							return mode.Param2IsImmediate ? op2 : mem[op2];
						}
						return pc;
					}
				}
			},
			{
				7,
				new Instruction { Name = "less-than", Execute = (mem, data, mode, pc) =>
					{
						var op1 = mem[pc++];
						var op2 = mem[pc++];
						var dst = mem[pc++];
						mem[dst] = (mode.Param1IsImmediate ? op1 : mem[op1]) < (mode.Param2IsImmediate ? op2 : mem[op2]) ? 1 : 0;
						return pc;
					}
				}
			},
			{
				8,
				new Instruction { Name = "equals", Execute = (mem, data, mode, pc) =>
					{
						var op1 = mem[pc++];
						var op2 = mem[pc++];
						var dst = mem[pc++];
						mem[dst] = (mode.Param1IsImmediate ? op1 : mem[op1]) == (mode.Param2IsImmediate ? op2 : mem[op2]) ? 1 : 0;
						return pc;
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

		public int[] Memory = new int[] { 99 };

		public Engine WithMemory(int[] memory)
		{
			Memory = (int[])memory.Clone();
			return this;
		}

		{
		}


		public bool Halt;
		public int Pc;

		public void Execute()
		{
			// Start from scratch
			Output.Clear();
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
