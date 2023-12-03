using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System.Linq;

namespace AdventOfCode.Y2019.Day23
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Category Six";
		public override int Year => 2019;
		public override int Day => 23;

		public override void Run()
		{
			Run("input").Part1(22151).Part2(17001);
		}

		protected override int Part1(string[] input)
		{
			// Create network and run all engines until a NAT-packet is received
			var intcode = input[0];
			var network = new Network(intcode, 50).Start();
			while (network.LastNatPacket == null)
			{
				foreach (var e in network.Engines)
				{
					e.Resume();
				}
			}

			var result = network.LastNatPacket.Y;
			return result;
		}

		protected override int Part2(string[] input)
		{
			// Create network and run all engines until they are all idle and the last
			// NAT-packet has the same value as the last time the network was idle.
			var intcode = input[0];
			var network = new Network(intcode, 50).Start();
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
			return result;
		}

		private class Network
		{
			private readonly bool[] _idle;

			public Network(string intcode, int size)
			{
				var memory = Engine.ReadAsMemory(intcode);

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

