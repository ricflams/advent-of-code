using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Intcode
{
    internal class Instruction
    {
		public string Name { get; set; }
		public Func<Mem, Data, Mode, int, int> Execute;
	}
}
