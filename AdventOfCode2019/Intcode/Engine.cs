using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;

namespace AdventOfCode2019.Intcode
{
	internal class Engine
	{
		private static readonly IDictionary<int, Instruction> Instructions = new Dictionary<int, Instruction>
		{
			{
				1, new Instruction.WithOp1Op2Dest { Name = "add", Execute = (engine, op1, op2, dest) =>
					{
						engine.WriteMemory(dest, op1 + op2);
					}
				}
			},
			{
				2, new Instruction.WithOp1Op2Dest { Name = "multiply", Execute = (engine, op1, op2, dest) =>
					{
						engine.WriteMemory(dest, op1 * op2);
					}
				}
			},
			{
				3, new Instruction.WithDest { Name = "get", Execute = (engine, dest) =>
					{
						engine.WriteMemory(dest, engine.Input.Take());
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
						engine.WriteMemory(dest, op1 < op2 ? 1 : 0);
					}
				}
			},
			{
				8, new Instruction.WithOp1Op2Dest { Name = "equals", Execute = (engine, op1, op2, dest) =>
					{
						engine.WriteMemory(dest, op1 == op2 ? 1 : 0);
					}
				}
			},
			{
				9, new Instruction.WithOp1 { Name = "set rel base", Execute = (engine, op1) =>
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

		public BlockingCollection<BigInteger> Input { get; set; } = new BlockingCollection<BigInteger>();
		public BlockingCollection<BigInteger> Output { get; set; } = new BlockingCollection<BigInteger>();

		public Dictionary<BigInteger, BigInteger> Memory = new Dictionary<BigInteger,BigInteger> { { 0, 99 } };

		public Engine WithMemory(int[] memory)
		{
			Memory.Clear();
			for (var i = 0; i < memory.Length; i++)
			{
				Memory[i] = memory[i];
			}
			return this;
		}

		public Engine WithMemory(BigInteger[] memory)
		{
			Memory.Clear();
			for (var i = 0; i < memory.Length; i++)
			{
				Memory[i] = memory[i];
			}
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
			var output = new List<BigInteger>();
			while (Output.TryTake(out var value))
			{
				output.Add(value);
			}
			return string.Join(" ", output);
		}

		public bool Halt;
		public BigInteger Pc;
		public BigInteger RelativeBase;

		public Engine Execute()
		{
			Halt = false;
			RelativeBase = 0;
			Pc = 0;

			while (!Halt)
			{
				var opcode = (int)Memory[Pc++];
				var instruction = Instructions[(int)(opcode % 100)];
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
						op.Execute(this, GetPosOperand(opcode / 100 % 10 == 2));
						break;
					case Instruction.WithOp1Op2Dest op:
						op.Execute(this, GetOperand1(opcode), GetOperand2(opcode), GetPosOperand(opcode / 10000 % 10 == 2));
						break;
				}
			}
			return this;
		}

		private BigInteger GetOperand1(int opcode)
		{
			switch (opcode / 100 % 10)
			{
				case 1: return ReadMemory(Pc++);
				case 2: return ReadMemory(RelativeBase + Pc++);
				default: return ReadMemory(ReadMemory(Pc++));
			}
		}

		private BigInteger GetOperand2(int opcode)
		{
			switch (opcode / 1000 % 10)
			{
				case 1: return ReadMemory(Pc++);
				case 2: return ReadMemory(RelativeBase + Pc++);
				default: return ReadMemory(ReadMemory(Pc++));
			}
		}

		private BigInteger ReadMemory(BigInteger address)
		{
			if (!Memory.ContainsKey(address))
			{
				Memory[address] = 0;
			}
			return Memory[address];
		}

		private void WriteMemory(BigInteger address, BigInteger value)
		{
			if (!Memory.ContainsKey(address))
			{
				Memory[address] = 0;
			}
			Memory[address] = value;
		}

		private BigInteger GetPosOperand(bool isRelative)
		{
			var pos = isRelative ? ReadMemory(RelativeBase + Pc++) : ReadMemory(Pc++);
			if (!Memory.ContainsKey(pos))
			{
				Memory[pos] = 0;
			}
			return pos;
		}
	}
}
