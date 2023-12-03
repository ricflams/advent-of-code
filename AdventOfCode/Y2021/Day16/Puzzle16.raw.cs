using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day16.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 16";
		public override int Year => 2021;
		public override int Day => 16;

		public override void Run()
		{
			//	Run("test0").Part1(1).Part2(54);//.Part2(0);

			Run("test1").Part1(16);//.Part2(0);

			Run("test2").Part1(12);//.Part2(0);
			Run("test3").Part1(23);//.Part2(0);
			Run("test4").Part1(31);//.Part2(0);

			//Debug.Assert(Part2(new string[] { "C200B40A82" } ) == 3);
			//Debug.Assert(Part2(new string[] { "04005AC33890" }) == 54);
			//Debug.Assert(Part2(new string[] { "880086C3E88112" }) == 7);
			//Debug.Assert(Part2(new string[] { "CE00C43D881120" }) == 9);
			//Debug.Assert(Part2(new string[] { "D8005AC2A8F0" }) == 1);
			//Debug.Assert(Part2(new string[] { "F600BC2D8F" }) == 0);
			//Debug.Assert(Part2(new string[] { "9C005AC2F8F0" }) == 0);
			//Debug.Assert(Part2(new string[] { "9C0141080250320F1802104A08" }) == 1);

			Run("input").Part1(989).Part2(7936430475134);
			//// 7917324454 too low
		}

		internal class Packet
		{
			public int Version { get; init; }
			public int TypeId { get; init; }
			public long? Literal { get; init; }
			public Packet[] SubPackets { get; init; }
		}

		protected override long Part1(string[] input)
		{
			var bits = input[0]
				.Select(x => Convert.ToString(Convert.ToInt32(x.ToString(), 16), 2).PadLeft(4, '0'))
				.SelectMany(x => x.Select(ch => ch == '1'))
				.ToArray();

			var offset = 0;
			var packet = ParsePacket(bits, ref offset);

			var sum = VersionSum(packet);
			return sum;
		}

		protected override long Part2(string[] input)
		{
			var bits = input[0]
				.Select(x => Convert.ToString(Convert.ToInt32(x.ToString(), 16), 2).PadLeft(4, '0'))
				.SelectMany(x => x.Select(ch => ch == '1'))
				.ToArray();

			var offset = 0;
			var packet = ParsePacket(bits, ref offset);

			var v = Calculate(packet);
			return v;
		}

		private int VersionSum(Packet p)
		{
			return p.Version + p.SubPackets.Sum(VersionSum);
		}

		private static long Calculate(Packet p)
		{
			switch (p.TypeId)
			{
				case 0: return p.SubPackets.Sum(Calculate);
				case 1: return p.SubPackets.Aggregate(1L, (prod, x) => prod * Calculate(x));
				case 2: return p.SubPackets.Min(Calculate);
				case 3: return p.SubPackets.Max(Calculate);
				case 4: return p.Literal.Value;
				case 5: return Calculate(p.SubPackets[0]) > Calculate(p.SubPackets[1]) ? 1 : 0;
				case 6: return Calculate(p.SubPackets[0]) < Calculate(p.SubPackets[1]) ? 1 : 0;
				case 7: return Calculate(p.SubPackets[0]) == Calculate(p.SubPackets[1]) ? 1 : 0;
			}
			throw new Exception();
		}

		private static Packet ParsePacket(bool[] bits, ref int offset)
		{
			var version = ParseBits(bits, ref offset, 3);
			var typeId = ParseBits(bits, ref offset, 3);
			if (typeId == 4)
			{
				var value = 0L;
				while (true)
				{
					var more = bits[offset++];
					for (var i = 0; i < 4; i++)
					{
						value = value << 1 | (bits[offset++] ? 1L : 0);
					}
					if (!more)
						break;
				}
				return new Packet
				{
					Version = version,
					TypeId = typeId,
					Literal = value,
					SubPackets = Array.Empty<Packet>()
				};
			}
			var packetModeCount = bits[offset++];
			if (packetModeCount)
			{
				var n = ParseBits(bits, ref offset, 11);
				var subPackets = new Packet[n];
				for (var i = 0; i < n; i++)
				{
					subPackets[i] = ParsePacket(bits, ref offset);
				}
				return new Packet
				{
					Version = version,
					TypeId = typeId,
					SubPackets = subPackets
				};
			}
			else
			{
				var len = ParseBits(bits, ref offset, 15);
				var end = offset + len;
				var subPackets = new List<Packet>();
				while (offset < end)
				{
					subPackets.Add(ParsePacket(bits, ref offset));
				}
				return new Packet
				{
					Version = version,
					TypeId = typeId,
					SubPackets = subPackets.ToArray()
				};
			}
		}

		private static int ParseBits(bool[] bits, ref int offset, int len)
		{
			var v = 0;
			while (len-- > 0)
			{
				v = v << 1 | (bits[offset++] ? 1 : 0);
			}
			return v;
		}
	}
}
