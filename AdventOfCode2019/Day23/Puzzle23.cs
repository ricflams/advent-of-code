using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day23
{
	internal static class Puzzle23
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var memory = Engine.ReadMemoryFromFile("Day23/input.txt");
			const int N = 50;
			var engines = Enumerable.Range(0, N)
				.Select(i => new
				{
					Address = i,
					Engine = new Engine()
						.WithMemory(memory)
						.WithInput(i)
				})
				.ToDictionary(x => x.Address, x => x.Engine);

			var result = 0L;
			engines
				.AsParallel()
				.WithDegreeOfParallelism(N)
				.Select(e =>
				{
					var receiveBuffer = new List<long>();
					e.Value
						.OnInput(engine =>
						{
							if (engine.Input.Count == 0)
							{
								engine.Input.Add(-1);
								//Console.WriteLine($"Engine {e.Key}: no awaiting input");
								System.Threading.Thread.Sleep(10);
							}
							else
							{
								var val = engine.Input.ToList().Last();
								//Console.WriteLine($"Engine {e.Key}: input {val}");
							}
						})
						.OnOutput(engine =>
						{
							receiveBuffer.Add(engine.Output.Take());
							if (receiveBuffer.Count() == 3)
							{
								var addr = receiveBuffer[0];
								var x = receiveBuffer[1];
								var y = receiveBuffer[2];
								//Console.WriteLine($"Engine {e.Key}: send packet {x},{y} to {addr}");
								if (addr == 255)
								{
									//Console.WriteLine($"Engine {e.Key}: send packet {x},{y} to {addr}");
									result = y;
									foreach (var ee in engines.Values)
									{
										ee.Halt = true;
									}
									return;
								}
								receiveBuffer.Clear();
								engines[(int)addr].WithInput(x, y);
							}
						})
						.Execute();
					return false;
				})
				.AsSequential()
				.ToList();

			Console.WriteLine($"Day 23 Puzzle 1: {result}");
			Debug.Assert(result == 22151);
		}

		private static void Puzzle2()
		{
			var memory = Engine.ReadMemoryFromFile("Day23/input.txt");
			const int N = 50;
			var engines = Enumerable.Range(0, N)
				.Select(i => new
				{
					Address = i,
					Engine = new Engine()
						.WithMemory(memory)
						.WithInput(i)
				})
				.ToDictionary(x => x.Address, x => x.Engine);

			var idle = new ConcurrentDictionary<int, bool>();
			//var idlesent = new ConcurrentBag<long>();
			engines[255] = null;

			Point lastPacket = null;
			//var nat = new BlockingCollection();

			var result = 0;
			engines
				.AsParallel()
				.WithDegreeOfParallelism(engines.Count)
				.Select(kvp =>
				{
					var address = kvp.Key;
					var engine = kvp.Value;
					if (engine == null)
					{
						Point last = null;
						while (true)
						{
							System.Threading.Thread.Sleep(10);
							if (idle.All(z => z.Value) && lastPacket != null)
							{
//								Console.WriteLine($"All is idle: send {lastPacket} to 0");
								if (last?.Y == lastPacket.Y)
								{
									result = last.Y;
									foreach (var ee in engines.Values.Where(e => e != null))
									{
										ee.Halt = true;
									}
									break;
								}
								var x = lastPacket.X;
								var y = lastPacket.Y;
								last = lastPacket;
								lastPacket = null;
								engines[0].WithInput(x, y);
							}
						}
						// the NAT
					}
					else
					{
						var receiveBuffer = new List<long>();
						engine
							.OnInput(_ =>
							{
								if (engine.Input.Count == 0)
								{
									engine.Input.Add(-1);
									//Console.WriteLine($"Engine {e.Key}: no awaiting input");
									System.Threading.Thread.Sleep(100);
									idle[address] = true;
								}
								else
								{
									idle[address] = false;
									var val = engine.Input.ToList().Last();
//									Console.WriteLine($"Engine {address}: input {val}");
								}
							})
							.OnOutput(_ =>
							{
								receiveBuffer.Add(engine.Output.Take());
								if (receiveBuffer.Count() == 3)
								{
									var destinationAddr = receiveBuffer[0];
									var x = receiveBuffer[1];
									var y = receiveBuffer[2];
									receiveBuffer.Clear();
									if (destinationAddr == 255)
									{
										//Console.WriteLine($"###### {y} to NAT");
										lastPacket = Point.From((int)x, (int)y);
										return;
									}
//									Console.WriteLine($"Engine {address}: send packet {x},{y} to {destinationAddr}");
									engines[(int)destinationAddr].WithInput(x, y);
								}
							})
							.Execute();
					}
					return false;
				})
				.AsSequential()
				.ToList();

			Console.WriteLine($"Day 23 Puzzle 2: {result}");
			Debug.Assert(result == 17001);
		}
	}
}

