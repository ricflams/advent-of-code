using System.Collections.Generic;

namespace AdventOfCode2019.Day25
{
	internal class Room
	{
		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public bool IsSecurityCheckpoint { get; internal set; }
		public bool IsPressureSensitiveFloor { get; internal set; }
		public List<string> Doors { get; } = new List<string>();
		public List<string> Items { get; } = new List<string>();
	}
}
