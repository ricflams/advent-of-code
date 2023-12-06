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

		public override void Run()
		{
			Run("test1").Part1(13).Part2(140);
			Run("input").Part1(6623).Part2(23049);
			Run("extra").Part1(5506).Part2(21756);
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

		internal record Number(int Val) : Packet
		{
			public override string ToString() => Val.ToString();
		}
		internal record List(Packet[] Items) : Packet
		{
			public override string ToString() => $"[{string.Join(',', Items.Select(x => x.ToString()))}]";
		}

		internal record Packet
		{
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
						return new Number(v);
					}
					if (ch == '[')
					{
						var list = new List<Packet>();
						while (true)
						{
							var p = ReadPacket();
							if (p == null) // nothing to read - that's okay
								break;
							list.Add(p);
							var next = s[pos++];
							if (next == ',') // move onto next item
								continue;
							if (next == ']') // done with this list
								break;
							throw new Exception("Unexpected state");
						}
						return new List(list.ToArray());
					}
					return null;
				}
			}

			public static int Compare(Packet p1, Packet p2)
			{
				return (p1, p2) switch
				{
					(Number a, Number b) => Math.Sign(a.Val - b.Val),
					(Number a, List b) => Compare(new List(new [] { a }), b),
					(List a, Number b) => Compare(a, new List(new [] { b })),
					(List a, List b) => CompareLists(a, b),
					(_, _) => throw new Exception()
				};

				int CompareLists(List a, List b)
				{
					for (var pos = 0;; pos++)
					{
						var (eol1, eol2) = (pos == a.Items.Length, pos == b.Items.Length);
						if (eol1 || eol2)
							return !eol2 ? -1 : !eol1 ? 1 : 0;
						var compared = Compare(a.Items[pos], b.Items[pos]);
						if (compared == 0)
							continue;
						return compared;
					}
				}
			}
		}
	}
}
