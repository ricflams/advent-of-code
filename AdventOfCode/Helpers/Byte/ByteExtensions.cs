using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers.Byte
{
    public static class ByteExtensions
    {
        public static int NumberOfSetBits(this uint value)
		{
			var i = value;
			i -= (i >> 1) & 0x55555555;
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			return (int)((((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24);
		}

		private static readonly int[] NumberOfBitsInByteLookup = new []
		{
			0,1,1,2,1,2,2,3,1,2,2,3,2,3,3,4,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,
			1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,
			1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,
			2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,
			1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,
			2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,
			2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,
			3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,4,5,5,6,5,6,6,7,5,6,6,7,6,7,7,8,
		};

		public static int NumberOfSetBits(this byte b) => NumberOfBitsInByteLookup[b];

		public static IEnumerable<uint> Bits(this uint value)
		{
			for (uint bit = 1; value > 0; bit <<= 1, value >>= 1)
			{
				if ((value & 1) == 1)
				{
					yield return bit;
				}
			}
		}

		public static ulong ReverseBits(this ulong value, int len)
		{
			uint reversed = 0;
			for (uint bit = 1U << (len-1); value > 0 && bit > 0; bit >>= 1, value >>= 1)
			{
				if ((value & 1) == 1)
				{
					reversed |= bit;
				}
			}
			return reversed;
		}

		public static bool HasAnyHexSequence(this byte[] ba, int length, out byte val)
		{
			var digits = new byte[ba.Length*2];
			var di = 0;
			for (var i = 0; i < ba.Length; i++)
			{
				digits[di++] = (byte)((ba[i] & 0xf0) >> 4);
				digits[di++] = (byte)(ba[i] & 0x0f);
			}

			for (var i = 0; i < digits.Length - length + 1; i++)
			{
				var match = true;
				for (var j = 1; match && j < length; j++)
				{
					match = digits[i] == digits[i+j];
				}
				if (match)
				{
					val = digits[i];
					return true;
				}
			}
			val = 0;
			return false;
		}

		public static bool HasHexSequence(this byte[] ba, int length, byte val)
		{
			var digits = new byte[ba.Length*2];
			var di = 0;
			for (var i = 0; i < ba.Length; i++)
			{
				digits[di++] = (byte)((ba[i] & 0xf0) >> 4);
				digits[di++] = (byte)(ba[i] & 0x0f);
			}

			for (var i = 0; i < digits.Length - length + 1; i++)
			{
				if (digits[i] == val)
				{
					var match = true;
					for (var j = 1; match && j < length; j++)
					{
						match = digits[i+j] == val;
					}
					if (match)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static string[] HexDigitTable = null;
		public static string FormatAsHex(this byte[] ba)
		{
			if (HexDigitTable == null)
			{
				HexDigitTable = Enumerable.Range(0, 256)
					.Select(x => x.ToString("x2"))
					.ToArray();
			}

			return string.Create(ba.Length * 2, ba, (chars, ba) =>
			{
				for (var i = 0; i < ba.Length; i++)
				{
					HexDigitTable[ba[i]].AsSpan().CopyTo(chars.Slice(i * 2));
				}
			});
		}
    }
}