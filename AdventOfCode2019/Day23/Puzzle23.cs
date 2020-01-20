using System;
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
			// Create network and run all engines until a NAT-packet is received
			var network = new Network(50).Start();
			while (network.LastNatPacket == null)
			{
				foreach (var e in network.Engines)
				{
					e.Resume();
				}
			}

			var result = network.LastNatPacket.Y;
			Console.WriteLine($"Day 23 Puzzle 1: {result}");
			Debug.Assert(result == 22151);
		}

		private static void Puzzle2()
		{
			// Create network and run all engines until they are all idle and the last
			// NAT-packet has the same value as the last time the network was idle.
			var network = new Network(50).Start();
			Point lastNatPacket = null;

			while (true)
			{
				foreach (var e in network.Engines)
				{
					e.Resume();
				}
				if (network.IsAllIdle && network.LastNatPacket != null)
				{
					if (lastNatPacket?.Y == network.LastNatPacket?.Y)
					{
						break;
					}
					lastNatPacket = network.LastNatPacket;
					network.LastNatPacket = null;
					network.Engines[0].WithInput(lastNatPacket.X, lastNatPacket.Y);
				}
			}

			var result = lastNatPacket.Y;
			Console.WriteLine($"Day 23 Puzzle 2: {result}");
			Debug.Assert(result == 17001);
		}

		private class Network
		{
			private readonly bool[] _idle;

			public Network(int size)
			{
				var memory = Engine.ReadMemoryFromFile("Day23/input.txt");

				_idle = new bool[size];
				Engines = Enumerable.Range(0, size).Select(i => new Engine()).ToArray();
				for (var i = 0; i < size; i++)
				{
					var engineNumber = i;
					Engines[i]
						.WithMemory(memory)
						.WithInput(i)
						.OnInput(engine =>
						{
							if (!engine.Input.Any())
							{
								engine.Input.Add(-1);
								_idle[engineNumber] = true;
								//Console.WriteLine($"Engine {e.Key}: no awaiting input");
							}
							else
							{
								_idle[engineNumber] = false;
								//Console.WriteLine($"Engine {e.Key}: input {val}");
							}
							engine.Suspend();
						})
					.OnOutput(engine =>
					{
						if (engine.Output.Count() == 3)
						{
							var values = engine.Output.TakeAll().ToArray();
							var addr = values[0];
							var x = values[1];
							var y = values[2];
							//Console.WriteLine($"Engine {e.Key}: send packet {x},{y} to {addr}");
							if (addr == 255)
							{
								//Console.WriteLine($"Found result {y}");
								LastNatPacket = Point.From((int)x, (int)y);
								return;
							}
							Engines[(int)addr].WithInput(x, y);
						}
					});
				}
			}

			public Engine[] Engines { get; private set; }
			public Point LastNatPacket { get; set; }
			public bool IsAllIdle => _idle.All(x => x);

			public Network Start()
			{
				// Kick off all engines
				foreach (var e in Engines)
				{
					e.Execute();
				}
				return this;
			}
		}
	}
}

