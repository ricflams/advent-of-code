using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day16
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Packet Decoder";
		public override int Year => 2021;
		public override int Day => 16;

		public override void Run()
		{
			Run("test1-1").Part1(16);
			Run("test1-2").Part1(12);
			Run("test1-3").Part1(23);
			Run("test1-4").Part1(31);
			Run("test2-1").Part2(3);
			Run("test2-2").Part2(54);
			Run("test2-3").Part2(7);
			Run("test2-4").Part2(9);
			Run("test2-5").Part2(1);
			Run("test2-6").Part2(0);
			Run("test2-7").Part2(0);
			Run("test2-8").Part2(1);
			Run("input").Part1(989).Part2(7936430475134);
		}

		protected override long Part1(string[] input)
		{
			return Packet.ParsePacket(input[0]).VersionSum;
		}

		protected override long Part2(string[] input)
		{
			return Packet.ParsePacket(input[0]).Value;
		}

		internal class Packet
		{
			public int Version { get; init; }
			public int TypeId { get; init; }
			public long? Literal { get; init; }
			public List<Packet> SubPackets { get; init; }

			public int VersionSum => Version + SubPackets.Sum(p => p.VersionSum);

			public long Value =>
				TypeId switch
				{
					0 => SubPackets.Sum(x => x.Value),
					1 => SubPackets.Aggregate(1L, (prod, x) => prod * x.Value),
					2 => SubPackets.Min(x => x.Value),
					3 => SubPackets.Max(x => x.Value),
					4 => Literal.Value,
					5 => SubPackets[0].Value > SubPackets[1].Value ? 1 : 0,
					6 => SubPackets[0].Value < SubPackets[1].Value ? 1 : 0,
					7 => SubPackets[0].Value == SubPackets[1].Value ? 1 : 0,
					_ => throw new Exception($"Unhandled type {TypeId}")
				};

			public static Packet ParsePacket(string s)
			{
				var bits = s
					.Select(x => Convert.ToString(Convert.ToInt32(x.ToString(), 16), 2).PadLeft(4, '0'))
					.SelectMany(x => x.Select(ch => ch == '1'))
					.ToArray();

				var offset = 0;
				return ParsePacket(bits, ref offset);
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
						SubPackets = new List<Packet>()
					};
				}
				if (bits[offset++])
				{
					// Read n sub-packets
					var n = ParseBits(bits, ref offset, 11);
					var subPackets = new List<Packet>();
					for (var i = 0; i < n; i++)
					{
						subPackets.Add(ParsePacket(bits, ref offset));
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
					// Read x bytes worth of sub-packets 
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
						SubPackets = subPackets
					};
				}

				static int ParseBits(bool[] bits, ref int offset, int len)
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
	}
}
