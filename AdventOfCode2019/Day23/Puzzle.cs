using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day23
{
	internal static class Puzzle
	{
		public static void Run()
		{
			//Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			const int N = 50;
			var engines = Enumerable.Range(0, N)
				.Select(i => new
				{
					Address = i,
					Engine = new Engine()
						.WithMemoryFromFile("Day23/input.txt")
						.WithInput(i)
				})
				.ToDictionary(x => x.Address, x => x.Engine);

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
								System.Threading.Thread.Sleep(100);
							}
							else
							{
								var val = engine.Input.ToList().Last();
								Console.WriteLine($"Engine {e.Key}: input {val}");
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
								Console.WriteLine($"Engine {e.Key}: send packet {x},{y} to {addr}");
								if (addr == 255)
								{
									Console.WriteLine($"Engine {e.Key}: send packet {x},{y} to {addr}");
									Console.ReadLine();
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
		}

		private static void Puzzle2()
		{
			const int N = 50;
			var engines = Enumerable.Range(0, N)
				.Select(i => new
				{
					Address = i,
					Engine = new Engine()
						.WithMemoryFromFile("Day23/input.txt")
						.WithInput(i)
				})
				.ToDictionary(x => x.Address, x => x.Engine);

			var idle = new ConcurrentDictionary<int, bool>();
			//var idlesent = new ConcurrentBag<long>();
			engines[255] = null;

			Point lastPacket = null;
			//var nat = new BlockingCollection();

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
									Console.WriteLine($"######################## puazle 2: {last.Y}");
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
		}

	}

}

