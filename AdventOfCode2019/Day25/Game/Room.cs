using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Day25.Game
{
	internal class Room
	{
		public static Room Unexplored = new Room { Name = "?" };

		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public List<Direction> Directions { get; internal set; } = new List<Direction>();
		public List<string> Items { get; internal set; } = new List<string>();

		public void TakeItem(string item)
		{
			Items.Remove(item);
		}

		public void DropItem(string item)
		{
			Items.Add(item);
		}
	}
}
