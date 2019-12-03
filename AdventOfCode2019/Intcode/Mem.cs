using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Intcode
{
    internal class Mem
    {
		private int[] _raw;

		public void Initialize(int[] raw)
		{
			_raw = (int[])raw.Clone();
		}

		public int this[int address]
		{
			get => _raw[address];
			set => _raw[address] = value;
		}
	}
}
