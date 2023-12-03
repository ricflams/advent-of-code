using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace AdventOfCode.Y2017.Day18
{
	internal class Puzzle : Puzzle<long, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Duet";
		public override int Year => 2017;
		public override int Day => 18;

		public override void Run()
		{
			Run("test1").Part1(4);
			Run("test2").Part2(3);
			Run("input").Part1(2951).Part2(7366);
		}

		protected override long Part1(string[] input)
		{
			var t = new Tablet(input);

			// Registering the last sound and halt when trying to recover
			// the first non-0 sound-value
			var lastSound = 0L;
			t.OnSnd = v => lastSound = v;
			t.OnRcv = () => { t.Halt = lastSound != 0; return 0; };
			t.Run();

			return lastSound;
		}

		protected override int Part2(string[] input)
		{
			var t0 = new TabletWithIo(input, 0);
			var t1 = new TabletWithIo(input, 1);

			t0.CommunicatesWith(t1);
			t1.CommunicatesWith(t0);

			Task.WhenAll(
				Task.Run(() => t0.Tablet.Run()),
				Task.Run(() => t1.Tablet.Run())
			).Wait();

			return t1.NumberOfSentValues;
		}

		internal class TabletWithIo
		{
			public Tablet Tablet { get; }
			internal readonly BlockingCollection<long> Queue = new BlockingCollection<long>();
			internal readonly CancellationTokenSource Cts = new CancellationTokenSource();

			public TabletWithIo(string[] code, int id)
			{
				Tablet = new Tablet(code);
				Tablet.Regs['p'] = id;
			}

			public int NumberOfSentValues { get; private set; }
			internal bool IsWaitingForReceive { get; private set; }
			internal bool IsBlocked => IsWaitingForReceive && !Queue.Any() && NumberOfSentValues > 0;

			public void CommunicatesWith(TabletWithIo other)
			{
				Tablet.OnSnd = v =>
				{
					NumberOfSentValues++;
					other.Queue.Add(v);
				};
				Tablet.OnRcv = () =>
				{
					IsWaitingForReceive = true;
					if (IsBlocked && other.IsBlocked)
					{
						// Deadlocked; both tablets are blocked receiving
						Tablet.Halt = true;
						Cts.Cancel();
						other.Tablet.Halt = true;
						other.Cts.Cancel();
						return 0;
					}
					try
					{
						return Queue.Take(Cts.Token);
					}
					catch (OperationCanceledException)
					{
						return 0;
					}
					finally
					{
						IsWaitingForReceive = false;
					}
				};
			}
		}

		public class Tablet
		{
			private readonly Ins[] _code;
			private int _ip;

			public SafeDictionary<char, long> Regs = new SafeDictionary<char, long>();

			public Func<long> OnRcv { get; set; }
			public Action<long> OnSnd { get; set; }

			public bool Halt { get; set; }

			public Tablet(string[] code)
			{
				_code = code
					.Select(line =>
					{
						var p = line.Split(' ');
						return p[0] switch
						{
							// snd X plays a sound with a frequency equal to the value of X.
							// set X Y sets register X to the value of Y.
							// add X Y increases register X by the value of Y.
							// mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
							// mod X Y sets register X to the remainder of dividing the value contained in register X by the value of Y (that is, it sets X to the result of X modulo Y).
							// rcv X recovers the frequency of the last sound played, but only when the value of X is not zero. (If it is zero, the command does nothing.)
							// jgz X Y jumps with an offset of the value of Y, but only if the value of X is greater than zero. (An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
							//
							// Part 2:
							// snd X sends the value of X to the other program.
							// rcv X receives the next value and stores it in register X.
							"snd" => new Ins(OpCode.Snd, GetOp(p[1])),
							"set" => new Ins(OpCode.Set, GetOp(p[1]), GetOp(p[2])),
							"add" => new Ins(OpCode.Add, GetOp(p[1]), GetOp(p[2])),
							"mul" => new Ins(OpCode.Mul, GetOp(p[1]), GetOp(p[2])),
							"mod" => new Ins(OpCode.Mod, GetOp(p[1]), GetOp(p[2])),
							"rcv" => new Ins(OpCode.Rcv, GetOp(p[1])),
							"jgz" => new Ins(OpCode.Jgz, GetOp(p[1]), GetOp(p[2])),
							_ => throw new Exception($"Unknown instruction {p[0]}")
						};
					})
					.ToArray();
				_ip = 0;

				(int, bool) GetOp(string op) => char.IsLetter(op.First()) ? (op.First(), true) : (int.Parse(op), false);
			}

			private enum OpCode { Snd, Set, Add, Mul, Mod, Rcv, Jgz };
			private class Operand
			{
				public long Value { get; set; }
				public bool IsRegister { get; set; }
			}
			private class Ins
			{
				public Ins(OpCode opc, params (long, bool)[] ops)
				{
					OpCode = opc;
					Ops = ops.Select(x => new Operand { Value = x.Item1, IsRegister = x.Item2 }).ToArray();
				}
				public OpCode OpCode { get; set; }
				public Operand[] Ops { get; set; }
			}

			private long ValueOf(Operand op) => op.IsRegister ? Regs[(char)op.Value] : op.Value;
			private char Reg(Operand op) => (char)op.Value;

			public void Run()
			{
				_ip = 0;
				while (_ip < _code.Length && !Halt)
				{
					var ins = _code[_ip];
					var ops = ins.Ops;
					switch (ins.OpCode)
					{
						case OpCode.Snd:
							OnSnd(ValueOf(ops[0]));
							break;
						case OpCode.Set:
							Regs[Reg(ops[0])] = ValueOf(ops[1]);
							break;
						case OpCode.Add:
							Regs[Reg(ops[0])] += ValueOf(ops[1]);
							break;
						case OpCode.Mul:
							Regs[Reg(ops[0])] *= ValueOf(ops[1]);
							break;
						case OpCode.Mod:
							Regs[Reg(ops[0])] %= ValueOf(ops[1]);
							break;
						case OpCode.Rcv:
							Regs[Reg(ops[0])] = OnRcv();
							break;
						case OpCode.Jgz:
							if (ValueOf(ops[0]) > 0)
							{
								var offset = (int)ValueOf(ops[1]);
								_ip = (_ip + offset + _code.Length) % _code.Length;
								_ip--;
							}
							break;

						default:
							throw new Exception($"Unhandled opcode {ins.OpCode}");
					}
					_ip++;
				}
			}
		}
	}
}
