using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day13
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Distress Signal";
		public override int Year => 2022;
		public override int Day => 13;

		public void Run()
		{
			Run("test1").Part1(13).Part2(140);
		//	Run("input").Part1(6623).Part2(23049);
		}

		protected override long Part1(string[] input)
		{
			var sum = input
				.GroupByEmptyLine()
				.Select((s, idx) =>
				{
					var p1 = Packet.Read(s[0]);
					var p2 = Packet.Read(s[1]);
					return Packet.Compare(p1, p2) <= 0 ? idx+1 : 0;
				})
				.Sum();
			return sum;
		}

		protected override long Part2(string[] input)
		{
			var div1 = Packet.Read("[[2]]");
			var div2 = Packet.Read("[[6]]");

			var packets = input
				.Where(s => s.Any())
				.Select(Packet.Read)
				.Append(div1)
				.Append(div2)
				.ToList();
			packets.Sort(Packet.Compare);

			var i1 = packets.IndexOf(div1) + 1;
			var i2 = packets.IndexOf(div2) + 1;
			return i1 * i2;
		}

		private class Packet
		{
			public int? Value;
			public Packet[] List;

			public static Packet Read(string s)
			{
				var pos = 0;
				return ReadPacket();

				Packet ReadPacket()
				{
					var ch = s[pos++];
					if (Char.IsDigit(ch))
					{
						var v = ch - '0';
						while (Char.IsDigit(s[pos]))
						{
							v = v*10 + s[pos++] - '0';
						}
						return new Packet { Value = v };
					}
					if (ch == '[')
					{
						var list = new List<Packet>();
						while (true)
						{
							var p = ReadPacket();
							if (p == null)
								break;
							list.Add(p);
							var next = s[pos++];
							if (next == ',') // move onto next item
								continue;
							if (next == ']') // done with this list
								break;
							throw new Exception("Unexpected state");
						}
						return new Packet { List = list.ToArray() };
					}
					return null;
				}
			}

			public override string ToString() =>
				Value.HasValue
					? $"{Value}"
					: $"[{string.Join(',', List.Select(x => x.ToString()))}]";

			public static int Compare(Packet p1, Packet p2)
			{
				var (v1, v2) = (p1.Value, p2.Value);
				if (v1.HasValue && v2.HasValue)
				{
					if (v1 < v2)
						return -1;
					if (v1 > v2)
						return 1;
					return 0;
				}
				if (p1.Value.HasValue && !p2.Value.HasValue)
				{
					return Compare(new Packet { List = new [] { p1 }}, p2);
				}
				if (!p1.Value.HasValue && p2.Value.HasValue)
				{
					return Compare(p1, new Packet { List = new [] { p2 }});
				}

				var list1 = p1.List;
				var list2 = p2.List;
				for (var pos = 0;; pos++)
				{
					if (pos == list1.Length && pos == list2.Length)
						return 0;
					if (pos == list1.Length && pos < list2.Length) // left ran out => ok
						return -1;
					if (pos == list2.Length && pos < list1.Length) // right ran out => not ok
						return 1;
				
					var compared = Compare(list1[pos], list2[pos]);
					if (compared != 0)
						return compared;
				}
			}
		}
	}
}
