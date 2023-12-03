using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day13.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 13";
		public override int Year => 2022;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(13).Part2(140);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(6623).Part2(23049);
		}

		class Packet
		{
			public int? Value;
			public Packet[] List;

			public static Packet Read(string s)
			{
				var pos = 0;
				return ReadPacket(s, ref pos);
			}

			public override string ToString()
			{
				return Value.HasValue
					? Value.ToString()
					: $"[{string.Join(',', List.Select(x => x.ToString()))}]";
			}

			private static Packet ReadPacket(string s, ref int pos)
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
						var p = ReadPacket(s, ref pos);
						if (p == null)
							break;
						list.Add(p);
						var ch2 = s[pos++];
						if (ch2 == ',')
							continue; // all ok
						if (ch2 == ']')
							break;
						throw new Exception();
					}
					return new Packet { List = list.ToArray() };
				}
				return null;
			}
		}

		private static bool AreEqual(Packet[] list1, Packet[] list2)
		{
			if (list1.Length != list2.Length)
				return false;
			for (var i = 0; i < list1.Length; i++)
			{
				var p1 = list1[i];
				var p2 = list2[i];
				if (p1.Value.HasValue && p2.Value.HasValue)
				{
					if (p1.Value != p2.Value)
						return false;
					continue;
				}
				if (!p1.Value.HasValue && !p2.Value.HasValue)
				{
					return AreEqual(p1.List, p2.List);
				}
				else if (p1.Value.HasValue)
				{
					return AreEqual(new[] { p1 }, p2.List);
				}
				else if (p2.Value.HasValue)
				{
					return AreEqual(p1.List, new[] { p2 });
				}
				throw new Exception();
			}
			return true;
		}

		private static bool? IsInRightOrder(Packet p1, Packet p2)
		{
			if (p1.Value.HasValue && p2.Value.HasValue)
			{
				if (p1.Value < p2.Value)
					return true;
				if (p2.Value < p1.Value)
					return false;
				return null;
			}
			if (p1.Value.HasValue && !p2.Value.HasValue)
			{
				return IsInRightOrder(new Packet { List = new [] { p1 }}, p2);
			}
			if (!p1.Value.HasValue && p2.Value.HasValue)
			{
				return IsInRightOrder(p1, new Packet { List = new [] { p2 }});
			}

			var list1 = p1.List;
			var list2 = p2.List;
			var pos = 0;
			while (true)
			{
				if (pos == list1.Length && pos == list2.Length)
					return null;
				if (pos == list1.Length && pos < list2.Length) // left ran out => ok
					return true;
				if (pos == list2.Length && pos < list1.Length) // right ran out => not ok
					return false;
				
				var px1 = list1[pos];
				var px2 = list2[pos];
				pos++;
				var check = IsInRightOrder(px1, px2);
				if (check == null)
					continue;
				return check;
			}
			//return true;
		}

		protected override long Part1(string[] input)
		{
			var sum = 0;
			var x = input
				.GroupByEmptyLine()
				.Select((s, idx) =>
				{
					var p1 = Packet.Read(s[0]);
					var p2 = Packet.Read(s[1]);
					var ok = IsInRightOrder(p1, p2).Value;
					//Console.WriteLine(ok);
					if (ok)
						sum += idx+1;
					return ok;
				})
				.ToArray();
			return sum;
		}

		private class PacketComparer : IComparer<Packet>
		{
			public int Compare(Packet p1, Packet p2)
			{
				// if (p1.ToString() == p2.ToString())
				// 	return 0;
				return IsInRightOrder(p1, p2).Value ? -1 : 1;
			}
		}

		protected override long Part2(string[] input)
		{


			var dec1 = Packet.Read("[[2]]");
			var dec2 = Packet.Read("[[6]]");

// var l = new List<Packet>();
// l.Add(dec1);
// //l.Add(dec2);
// // l.Add(Packet.Read("[]"));
// // l.Add(Packet.Read("[[]]]"));
// //l.Add(Packet.Read("[[[]]]"));
// l.Add(Packet.Read("[1,1,3,1,1]"));
// //l.Add(Packet.Read("[1,1,5,1,1]"));
// // l.Add(Packet.Read("[[1],4]"));
// l.Sort(new PacketComparer());
// foreach (var x in l)
// 	Console.WriteLine(x);
// for (var i = 0; i < l.Count; i++)
// {
// 	for (var j = i + 1; j < l.Count; j++)
// 	{
// 		var t1 = IsInRightOrder(l[i],l[j]);
// 		var t2 = IsInRightOrder(l[j],l[i]);
// 		Console.WriteLine($"{l[i]} vs {l[j]}: {t1} {t2} {(t1==!t2?"ok":"#######")}");
// 	}
// }


			var packets = input
				.GroupByEmptyLine()
				.SelectMany((s, idx) =>
				{
					var p1 = Packet.Read(s[0]);
					var p2 = Packet.Read(s[1]);
					return new[] { p1, p2 };
				})
				.Append(dec1)
				.Append(dec2)
				.ToList();


// for (var i = 0; i < packets.Count; i++)
// {
// 	for (var j = i + 1; j < packets.Count; j++)
// 	{
// 		var t1 = IsInRightOrder(packets[i],packets[j]);
// 		var t2 = IsInRightOrder(packets[j],packets[i]);
// 		Console.WriteLine($"{packets[i]} vs {packets[j]}: {t1} {t2} {(t1==!t2?"ok":"#######")}");
// 	}
// }				

				// var dup = packets
				// 	.GroupBy(x => x.ToString())
				// 	.Where(x => x.Count() > 1)
				// 	.ToArray();

			packets.Sort(new PacketComparer());
			var i1 = packets.IndexOf(dec1) + 1;
			var i2 = packets.IndexOf(dec2) + 1;

// 23283 not right
// 23600 to high
			return i1 * i2;
		}


	}
}
