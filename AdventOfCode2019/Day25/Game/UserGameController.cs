using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019.Day25.Game
{
	internal class UserGameController : IGameController
	{
		private readonly List<string> _pendingCommands = new List<string>();
		private Room _waitingRoom = null;

		public string Command(Maze maze)
		{
			var room = maze.Room;

			if (_pendingCommands.Any())
			{
				if (_waitingRoom == room)
				{
					var command = _pendingCommands.First();
					_pendingCommands.RemoveAt(0);
					return command;
				}
			}
			_pendingCommands.Clear();

			Console.Clear();

			//var (min, max) = maze.Map.Area();
			//var lines = Enumerable.Range(min.Y, max.Y - min.Y + 1)
			//	.Select(y => Enumerable.Range(min.X, max.X - min.X + 1)
			//		.Select(x =>
			//		{
			//			var r = maze.Map[x][y];
			//			return
			//				r == null ? ' ' :
			//				r == room ? '@' :
			//				r == Room.Unexplored ? '?' :
			//				r.Items.Any() ? (maze.DangerousItems.Contains(r.Items.First()) ? 'X' : '%') :
			//				'#';
			//		})
			//		.ToArray()
			//	)
			//	.Select(ch => new string(ch))
			//	.ToArray();
			//foreach (var line in lines)
			//{
			//	Console.WriteLine($"{line}");
			//}

			//Console.WriteLine($"At {maze.Here}: {room.Name}");
			Console.WriteLine($"At {room.Name}");
			Console.WriteLine($"{room.Description}");
			Console.WriteLine($"Directions  : {string.Join(" ", room.Directions.Select(d => Enum.GetName(typeof(Direction), d)))}");
			Console.WriteLine($"You see here: {(room.Items.Any() ? string.Join(", ", room.Items) : "(nothing)")}");
			Console.WriteLine($"Inventory   : {(maze.Inventory.Any() ? string.Join(", ", maze.Inventory.Select(x => $"({x.Key}) {x.Value}")) : "(empty)")}");
			Console.WriteLine();
			foreach (var message in maze.Messages)
			{
				Console.WriteLine($"{message}");
			}
			maze.Messages.Clear();
			Console.Write("Arrows=move ,=take d=drop D=dropall>");

			Console.WriteLine($"\n\n-----------------raw---------------------:\n{maze.RawGameEngineOutput}\n-----------------raw---------------------");

			while (true)
			{
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.UpArrow && room.Directions.Contains(Helpers.Direction.Up))
				{
					//maze.Here = maze.Here.Up; // not pretty
					return "north";
				}
				if (key.Key == ConsoleKey.RightArrow && room.Directions.Contains(Helpers.Direction.Right))
				{
					//maze.Here = maze.Here.Right; // not pretty
					return "east";
				}
				if (key.Key == ConsoleKey.DownArrow && room.Directions.Contains(Helpers.Direction.Down))
				{
					//maze.Here = maze.Here.Down; // not pretty
					return "south";
				}
				if (key.Key == ConsoleKey.LeftArrow && room.Directions.Contains(Helpers.Direction.Left))
				{
					//maze.Here = maze.Here.Left; // not pretty
					return "west";
				}

				if (key.KeyChar == 'q')
				{
					Environment.Exit(0);
				}

				if (key.KeyChar == ',' && room.Items.Any())
				{
					return $"take {room.Items.First()}";
				}

				if (key.KeyChar == 'i')
				{
					return $"inv";
				}

				if (key.KeyChar == 'A')
				{
					Console.WriteLine("###################\n" + maze.AllOutput);
				}

				if (key.KeyChar == 'd' && maze.Inventory.Any())
				{
					var inv = maze.Inventory;
					Console.WriteLine();
					Console.WriteLine($"Drop [{string.Concat(maze.Inventory.Keys)}] or q to cancel: ");
					while (true)
					{
						var letter = Console.ReadKey(true).KeyChar;
						if (inv.ContainsKey(letter))
						{
							return $"drop {inv[letter]}";
						}
						if (letter == 'q')
							break;
						Console.WriteLine($"No item named {letter}");
					}
				}

				if (key.KeyChar == 'D' && maze.Inventory.Any())
				{
					var inv = maze.Inventory;
					Console.WriteLine();
					Console.Write("Press arrow for direction > ");
					key = Console.ReadKey(true);

					var direction = "";
					if (key.Key == ConsoleKey.UpArrow && room.Directions.Contains(Helpers.Direction.Up))
					{
						direction = "north";
					}
					if (key.Key == ConsoleKey.RightArrow && room.Directions.Contains(Helpers.Direction.Right))
					{
						direction = "east";
					}
					if (key.Key == ConsoleKey.DownArrow && room.Directions.Contains(Helpers.Direction.Down))
					{
						direction = "south";
					}
					if (key.Key == ConsoleKey.LeftArrow && room.Directions.Contains(Helpers.Direction.Left))
					{
						direction = "west";
					}

					_pendingCommands.Clear();
					var n = inv.Count();
					var all = inv.Values.ToArray(); // a copy
					foreach (var item in inv.Values)
					{
						_pendingCommands.Add($"drop {item}");
					}
					for (var i = 0; i < 1<<n; i++)
					{
						var items = all.Select((item, idx) => (i & (1 << idx)) != 0 ? item : null).Where(x => x != null).ToList();
						foreach (var item in items)
						{
							_pendingCommands.Add($"take {item}");
						}
						_pendingCommands.Add("inv");
						_pendingCommands.Add(direction);
						foreach (var item in items)
						{
							_pendingCommands.Add($"drop {item}");
						}
					}

					var command = _pendingCommands.First();
					_pendingCommands.RemoveAt(0);
					_waitingRoom = room;
					return command;
				}

			}
		}
	}
}
