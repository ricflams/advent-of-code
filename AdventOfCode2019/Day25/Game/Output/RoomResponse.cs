using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Day25.Game.Output
{
	internal class RoomResponse : Response
	{
		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public List<Direction> Directions { get; internal set; }
		public List<string> Items { get; internal set; }
	}
}
