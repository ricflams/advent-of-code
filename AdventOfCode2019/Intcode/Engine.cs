using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Intcode
{
	internal class Engine
	{
		private static readonly IDictionary<int, Instruction> Instructions = new Dictionary<int, Instruction>
		{
			{
				1, new Instruction.WithOpOpPos { Name = "add", Execute = (engine, op1, op2, dest) =>
					{
						engine.WriteMemory(dest, op1 + op2);
					}
				}
			},
			{
				2, new Instruction.WithOpOpPos { Name = "multiply", Execute = (engine, op1, op2, dest) =>
					{
						engine.WriteMemory(dest, op1 * op2);
					}
				}
			},
			{
				3, new Instruction.WithPos { Name = "get", Execute = (engine, dest) =>
					{
						engine.WriteMemory(dest, engine.Input.Take());
					}
				}
			},
			{
				4, new Instruction.WithOp { Name = "put", Execute = (engine, op) =>
					{
						engine.Output.Add(op);
						engine._outputHandler?.Invoke(engine);
					}
				}
			},
			{
				5, new Instruction.WithOpOp { Name = "jump-if-true", Execute = (engine, op1, op2) =>
					{
						if (op1 != 0)
						{
							engine.Pc = op2;
						}
					}
				}
			},
			{
				6, new Instruction.WithOpOp { Name = "jump-if-false", Execute = (engine, op1, op2) =>
					{
						if (op1 == 0)
						{
							engine.Pc = op2;
						}
					}
				}
			},
			{
				7, new Instruction.WithOpOpPos { Name = "less-than", Execute = (engine, op1, op2, dest) =>
					{
						engine.WriteMemory(dest, op1 < op2 ? 1 : 0);
					}
				}
			},
			{
				8, new Instruction.WithOpOpPos { Name = "equals", Execute = (engine, op1, op2, dest) =>
					{
						engine.WriteMemory(dest, op1 == op2 ? 1 : 0);
					}
				}
			},
			{
				9, new Instruction.WithOp { Name = "set-relbase", Execute = (engine, op1) =>
					{
						engine.RelativeBase += op1;
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

		public BlockingCollection<long> Input { get; set; } = new BlockingCollection<long>();
		public BlockingCollection<long> Output { get; set; } = new BlockingCollection<long>();
		public Dictionary<long, long> Memory = new Dictionary<long, long> { { 0, 99 } };

		public Engine WithMemory(int[] memory)
		{
			return WithMemory(memory.Select(x => (long)x).ToArray());
		}

		public Engine WithMemory(long[] memory)
		{
			Memory.Clear();
			for (var i = 0; i < memory.Length; i++)
			{
				Memory[i] = memory[i];
			}
			return this;
		}

		public Engine WithInput(params long[] input)
		{
			foreach (var value in input)
			{
				Input.Add(value);
			}
			return this;
		}

		private Action<Engine> _outputHandler;
		public Engine OnOutput(Action<Engine> outputHandler)
		{
			_outputHandler = outputHandler;
			return this;
		}

		internal bool Halt;
		internal long RelativeBase;
		internal long Pc;

		public Engine Execute()
		{
			Halt = false;
			RelativeBase = 0;
			Pc = 0;

			int mode;
			while (!Halt)
			{
				var opcode = (int)ReadMemory(Pc++);
				var instruction = Instructions[opcode % 100];
				mode = opcode / 10;
				switch (instruction)
				{
					case Instruction.WithNoOp op:
						op.Execute(this);
						break;
					case Instruction.WithOp op:
						op.Execute(this, GetOperand());
						break;
					case Instruction.WithOpOp op:
						op.Execute(this, GetOperand(), GetOperand());
						break;
					case Instruction.WithPos op:
						op.Execute(this, GetPosition());
						break;
					case Instruction.WithOpOpPos op:
						op.Execute(this, GetOperand(), GetOperand(), GetPosition());
						break;
				}
			}
			return this;

			int GetOpMode() => (mode /= 10) % 10;

			long GetOperand()
			{
				switch (GetOpMode())
				{
					case 1: return ReadMemory(Pc++);
					case 2: return ReadMemory(RelativeBase + ReadMemory(Pc++));
					default: return ReadMemory(ReadMemory(Pc++));
				}
			}

			long GetPosition()
			{
				switch (GetOpMode())
				{
					case 2: return RelativeBase + ReadMemory(Pc++);
					default: return ReadMemory(Pc++);
				}
			}
		}

		private long ReadMemory(long address)
		{
			if (!Memory.ContainsKey(address))
			{
				Memory[address] = 0;
			}
			return Memory[address];
		}

		private void WriteMemory(long address, long value)
		{
			Memory[address] = value;
		}

	}
}
