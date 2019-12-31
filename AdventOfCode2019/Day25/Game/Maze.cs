using AdventOfCode2019.Day25.Game.Output;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019.Day25.Game
{
	internal class Maze
	{
		private static string ValidInventoryLetters = "abcdefghijklmnoprstuvwxyz";
		private readonly Engine _engine;
		private IGameController _controller;

		public SparseMap<List<Room>> Map { get; } = new SparseMap<List<Room>>();
		public Dictionary<char, string> Inventory { get; } = new Dictionary<char, string>();
		//public Point Here { get; set; } = Point.From(0, 0);
		public List<string> Messages { get; } = new List<string>();

		public string[] DangerousItems { get; } = // Could be learned, but hardcode for now
		{
			"escape pod", "infinite loop", "photons", "molten lava", "giant electromagnet"
		};

		//public Room Room => Map[Here];
		public string RawGameEngineOutput = "";
		private Point _lastSecurityCheckpoint;
		public string AllOutput = "";

		public Maze()
		{
			//Map[Here] = Room.Unexplored;

			var input = new Queue<char>();
			var output = "";
			_engine = new Engine()
				.WithMemoryFromFile("Day25/input.txt")
				.OnOutput(engine =>
				{
					var result = engine.Output.TakeAll().ToList();
					foreach (var ch in result)
					{
						output += (char)ch;
						AllOutput += (char)ch;
						if (output.EndsWith("Command?"))
						{
							RawGameEngineOutput = output;
							foreach (var response in Response.ParseFrom(output))
							{
								HandleResponse(response);
							}
							output = "";
						}
					}
				})
				.OnInput(engine =>
				{
					if (input.Count() == 0)
					{
						var instruction = _controller.Command(this).Trim() + '\x0a';
						foreach (var ch in instruction.ToArray())
						{
							input.Enqueue(ch);
						}
					}
					engine.WithInput(input.Dequeue());
				});
		}

		public Dictionary<string, Room> Rooms { get; } = new Dictionary<string, Room>();
		public Room Room { get; private set; }

		private void HandleResponse(Response response)
		{
			switch (response)
			{
				case RoomResponse resp:
					if (!Rooms.ContainsKey(resp.Name))
					{
						Rooms.Add(resp.Name, new Room
						{
							Name = resp.Name,
							Description = resp.Description,
							Directions = resp.Directions,
							Items = resp.Items
						});
					}
					Room = Rooms[resp.Name];

					//foreach (var dir in Room.Directions)
					//	{
					//		if (Map[Here.Move(dir)] == null)
					//		{
					//			Map[Here.Move(dir)] = Room.Unexplored;
					//		}
					//	}
					//}
					//if (Room.Name == "Security Checkpoint")
					//{
					//	_lastSecurityCheckpoint = Here;
					//}
					break;
				case TakeItemResponse resp:
					Room.Items.Remove(resp.Item);
					var letter = ValidInventoryLetters.First(c => !Inventory.ContainsKey(c));
					Inventory[letter] = resp.Item;
					break;
				case DropItemResponse resp:
					Room.Items.Add(resp.Item);
					Inventory.Remove(Inventory.First(x => x.Value == resp.Item).Key);
					break;
				case MessageResponse resp:
					Messages.Add(resp.Message);
					break;
				case EjectedBackToCheckpointResponse _:
					Room = Rooms["Security Checkpoint"]; // todo
					break;
			}
		}


		public Maze WithController(IGameController controller)
		{
			_controller = controller;
			return this;
		}

		public Maze Run()
		{
			var result = _engine.Execute().Output.TakeAll();

			foreach (var ch in result)
			{
				Console.Write((char)ch);
			}


			Console.WriteLine("You die...");
			return this;
		}
	}

}
