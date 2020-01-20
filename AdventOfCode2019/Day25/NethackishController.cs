using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Day25
{
	internal class NethackishController : IGameController
	{
		private readonly Dictionary<ConsoleKey, string> _movement = new Dictionary<ConsoleKey, string>()
		{
			{ ConsoleKey.UpArrow, "north" },
			{ ConsoleKey.RightArrow, "east" },
			{ ConsoleKey.DownArrow, "south" },
			{ ConsoleKey.LeftArrow, "west" }
		};

		public string WhatNext(Game game)
		{
			Console.WriteLine();
			if (game.Message.Any())
			{
				Console.WriteLine();
				foreach (var message in game.Message)
				{
					Console.WriteLine($"Message: {message}");
				}
			}
			Console.WriteLine($"Room: {game.CurrentRoom.Name}");
			Console.WriteLine($"      {game.CurrentRoom.Description}");
			Console.WriteLine($"Items here: {(game.CurrentRoom.Items.Any() ? String.Join(",", game.CurrentRoom.Items) : "(none)")}");
			Console.WriteLine($"Doors here: {String.Join(",", game.CurrentRoom.Doors)}");
			Console.WriteLine();
			Console.WriteLine($"Your Inventory: {(game.Inventory.Any() ? String.Join(",", game.Inventory) : "(empty)")}");
			Console.WriteLine();
			Console.WriteLine("Commands:");
			Console.WriteLine("  press arrows to move");
			Console.WriteLine("  ,: take item");
			Console.WriteLine("  d: drop item");
			Console.WriteLine("  i: inventory");
			Console.WriteLine();
			Console.Write("What next? ");
			var command = NextCommand();
			Console.WriteLine(command);
			return command;

			string NextCommand()
			{
				while (true)
				{
					var key = Console.ReadKey(true);
					if (_movement.TryGetValue(key.Key, out var direction) && game.CurrentRoom.Doors.Contains(direction))
					{
						return direction;
					}

					switch (key.KeyChar)
					{
						case ',':
							if (SelectItem(game.CurrentRoom.Items, out var takeItem))
							{
								return $"take {takeItem}";
							}
							break;
						case 'd':
							if (SelectItem(game.Inventory, out var dropItem))
							{
								return $"drop {dropItem}";
							}
							break;
						case 'i':
							return "inv";
					}
				}
			}

			bool SelectItem(List<string> items, out string item)
			{
				if (!items.Any())
				{
					item = null;
					return false;
				}

				if (items.Count() == 1)
				{
					item = items.First();
					return true;
				}

				Console.WriteLine();
				var options = NamedItemList(items);
				foreach (var i in options)
				{
					Console.WriteLine($"Press {i.Key} for {i.Value}");
				}
				Console.WriteLine("Press q to cancel");
				while (true)
				{
					var letter = Console.ReadKey(true).KeyChar;
					if (options.TryGetValue(letter, out var choice))
					{
						item = choice;
						return true;
					}
					if (letter == 'q')
					{
						item = null;
						return false;
					}
					Console.WriteLine($"No item named {letter}");
				}
			}
		}

		private Dictionary<char, string> NamedItemList(IEnumerable<string> items)
		{
			var options = items
				.Select((name, i) => new { Letter = Convert.ToChar(i + 'a'), Name = name })
				.ToDictionary(x => x.Letter, x => x.Name);
			return options;
		}

		public void OnGameOver(Game game)
		{
			Console.Write("You die...--More--");
			Console.ReadLine();
			Console.Write("Do you want your possessions identified? [ynq] (n)");
			var key = Console.ReadKey().KeyChar;
			Console.WriteLine();
			if (key == 'y')
			{
				if (game.Inventory.Any())
				{
					foreach (var possession in NamedItemList(game.Inventory))
					{
						Console.WriteLine($"{possession.Key} - {possession.Value}");
					}
				}
				else
				{
					Console.WriteLine($"You're not carrying anything");
				}
			}
		}
	}
}
