using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2019.Intcode
{
	internal class Engine
	{
		private long[] _staticMemory = new long[] { 99 };
		private readonly Dictionary<long, long> _dynamicMemory = new Dictionary<long, long>();
		private long _relativeBase;
		private long _pc;

		public BlockingCollection<long> Input { get; set; } = new BlockingCollection<long>();
		public BlockingCollection<long> Output { get; set; } = new BlockingCollection<long>();
		public bool Halt { get; set; }

		public Engine WithMemoryFromFile(string filename)
		{
			var memory = ReadMemoryFromFile(filename);
			return WithMemory(memory);
		}

		public static long[] ReadMemoryFromFile(string filename)
		{
			var memory = File.ReadAllText(filename)
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(long.Parse)
				.ToArray();
			return memory;
		}

		public Engine WithMemory(int[] memory)
		{
			return WithMemory(memory.Select(x => (long)x).ToArray());
		}

		public Engine WithMemory(long[] memory)
		{
			_staticMemory = memory.ToArray();
			_dynamicMemory.Clear();
			return this;
		}

		public Engine WithMemoryValueAt(long address, long value)
		{
			WriteMemory(address, value);
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

		private Action<Engine> onOutput;
		public Engine OnOutput(Action<Engine> outputHandler)
		{
			onOutput = outputHandler;
			return this;
		}

		private Action<Engine> onInput;
		public Engine OnInput(Action<Engine> inputHandler)
		{
			onInput = inputHandler;
			return this;
		}

		public long ReadMemory(long address)
		{
			if (address < _staticMemory.Length)
			{
				return _staticMemory[address];
			}
			return _dynamicMemory.TryGetValue(address, out var value) ? value : 0;
		}

		private void WriteMemory(long address, long value)
		{
			if (address < _staticMemory.Length)
			{
				_staticMemory[address] = value;
				return;
			}
			_dynamicMemory[address] = value;
		}

		public Engine Execute()
		{
			Halt = false;
			_relativeBase = 0;
			_pc = 0;

			while (!Halt)
			{
				var opcode = (int)ReadMemory(_pc++);
				var mode1 = (opcode / 100) % 10;
				var mode2 = (opcode / 1000) % 10;
				var mode3 = (opcode / 10000) % 10;
				switch (opcode % 100)
				{
					case 1:
						{
							var (op1, op2, dest) = (Operand(mode1), Operand(mode2), Position(mode3));
							WriteMemory(dest, op1 + op2);
						}
						break;
					case 2:
						{
							var (op1, op2, dest) = (Operand(mode1), Operand(mode2), Position(mode3));
							WriteMemory(dest, op1 * op2);
						}
						break;
					case 3:
						{
							var dest = Position(mode1);
							onInput?.Invoke(this);
							WriteMemory(dest, Input.Take());
						}
						break;
					case 4:
						{
							var op = Operand(mode1);
							Output.Add(op);
							onOutput?.Invoke(this);
						}
						break;
					case 5:
						{
							var (op1, op2) = (Operand(mode1), Operand(mode2));
							if (op1 != 0)
							{
								_pc = op2;
							}
						}
						break;
					case 6:
						{
							var (op1, op2) = (Operand(mode1), Operand(mode2));
							if (op1 == 0)
							{
								_pc = op2;
							}
						}
						break;
					case 7:
						{
							var (op1, op2, dest) = (Operand(mode1), Operand(mode2), Position(mode3));
							WriteMemory(dest, op1 < op2 ? 1 : 0);
						}
						break;
					case 8:
						{
							var (op1, op2, dest) = (Operand(mode1), Operand(mode2), Position(mode3));
							WriteMemory(dest, op1 == op2 ? 1 : 0);
						}
						break;
					case 9:
						{
							var op1 = Operand(mode1);
							_relativeBase += op1;
						}
						break;
					case 99:
						{
							Halt = true;
						}
						break;
				}
			}
			return this;

			long Operand(int mode)
			{
				switch (mode)
				{
					case 1: return ReadMemory(_pc++);
					case 2: return ReadMemory(_relativeBase + ReadMemory(_pc++));
					default: return ReadMemory(ReadMemory(_pc++));
				}
			}

			long Position(int mode)
			{
				switch (mode)
				{
					case 2: return _relativeBase + ReadMemory(_pc++);
					default: return ReadMemory(_pc++);
				}
			}
		}
	}
}
