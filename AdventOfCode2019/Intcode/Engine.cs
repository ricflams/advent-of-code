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
				new Instruction { Name = "ADD", Execute = (mem, pc) =>
					{
						var loc1 = mem[pc++];
						var loc2 = mem[pc++];
						var dst =  mem[pc++];
						mem[dst] = mem[loc1] + mem[loc2];
						return pc;
					}
				}
			},
			{
				2,
				new Instruction { Name = "MUL", Execute = (mem, pc) =>
					{
						var loc1 = mem[pc++];
						var loc2 = mem[pc++];
						var dst =  mem[pc++];
						mem[dst] = mem[loc1] * mem[loc2];
						return pc;
					}
				}
			},
			{
				99,
				new Instruction { Name = "HALT", Execute = (mem, pc) => -1 }
			}
		};

		public void Initialize(int[] raw)
		{
			Memory.Initialize(raw);
		}

		public readonly Mem Memory = new Mem();

		public void Execute()
		{
			for (var pc = 0; pc >= 0; )
			{
				var instruction = _instructions[Memory[pc++]];
				pc = instruction.Execute(Memory, pc);
			}
		}
    }
}
