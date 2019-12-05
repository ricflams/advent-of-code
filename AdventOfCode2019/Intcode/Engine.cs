using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Intcode
{
	internal class Engine
	{
		private readonly Mem _memory = new Mem();

		private IDictionary<int, Instruction> _instructions = new Dictionary<int, Instruction>
		{
			{
				1,
				new Instruction { Name = "ADD", Execute = (mem, _, mode, pc) =>
					{
						var op1 = mem[pc++];
						var op2 = mem[pc++];
						var dst = mem[pc++];
						mem[dst] =
							(mode.Param1IsImmediate ? op1 : mem[op1]) +
							(mode.Param2IsImmediate ? op2 : mem[op2]);
						return pc;
					}
				}
			},
			{
				2,
				new Instruction { Name = "MUL", Execute = (mem, _, mode, pc) =>
					{
						var op1 = mem[pc++];
						var op2 = mem[pc++];
						var dst = mem[pc++];
						mem[dst] =
							(mode.Param1IsImmediate ? op1 : mem[op1]) *
							(mode.Param2IsImmediate ? op2 : mem[op2]);
						return pc;
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
				new Instruction { Name = "HALT", Execute = (_, __, ___, ____) => -1 }
			}
		};

		public void Initialize(int[] raw)
		{
			Memory.Initialize(raw);
		}

		public readonly Mem Memory = new Mem();
		public readonly Data Data = new Data();

		public void Execute()
		{
			for (var pc = 0; pc >= 0; )
			{
				var opcode = Memory[pc++];
				var instruction = _instructions[opcode % 100];
				var mode = new Mode
				{
					Param1IsImmediate = opcode / 100 % 10 == 1,
					Param2IsImmediate = opcode / 1000 % 10 == 1,
					Param3IsImmediate = opcode / 10000 % 10 == 1
				};
				pc = instruction.Execute(Memory, Data, mode, pc);
				Console.WriteLine("pc: " + pc);
			}
		}
    }
}
