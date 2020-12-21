using System;
using System.Collections.Generic;

namespace AdventOfCode.Helpers
{
	internal class Space
	{
		public const int MaxDim = 4;
		private readonly int _dimensions;

		public Space(int dimensions)
		{
			_dimensions = dimensions;
			if (_dimensions > MaxDim)
			{
				throw new Exception($"Max {MaxDim} dimensions supported for now");
			}
		}

		public HashSet<uint> Active = new HashSet<uint>();

		private static uint IdFrom(sbyte[] p) =>
			(uint)p[0] << 24 & 0xff000000 |
			(uint)p[1] << 16 & 0xff0000 |
			(uint)p[2] << 8 & 0xff00 |
			(uint)p[3] & 0xff;
		private static sbyte[] FromId(uint p) => new sbyte[]
		{
				(sbyte)(p >> 24 & 0xff),
				(sbyte)(p >> 16 & 0xff),
				(sbyte)(p >> 8 & 0xff),
				(sbyte)(p & 0xff)
		};

		public void MergeWith(Space other) => Active.UnionWith(other.Active);
		public bool IsSet(uint p) => Active.Contains(p);
		public bool Set(uint p) => Active.Add(p);
		public bool Set(sbyte[] p) => Active.Add(IdFrom(p));

		public Space NeighboursOf(uint p)
		{
			var space = new Space(_dimensions);
			foreach (var delta in MathHelper.PlusZeroMinusSequence(_dimensions))
			{
				var neighbor = FromId(p);
				for (var i = 0; i < delta.Length; i++)
				{
					neighbor[i] += (sbyte)delta[i];
				}
				space.Set(neighbor);
			}
			return space;
		}
	}
}
