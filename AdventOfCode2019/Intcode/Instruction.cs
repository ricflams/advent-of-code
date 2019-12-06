using System;

namespace AdventOfCode2019.Intcode
{
    internal class Instruction
    {
		public string Name { get; set; }

		public class WithNoOp : Instruction
		{
			public Action<Engine> Execute { get; set; }
		}

		public class WithOp1 : Instruction
		{
			public Action<Engine, int> Execute { get; set; }
		}

		public class WithOp1Op2 : Instruction
		{
			public Action<Engine, int, int> Execute { get; set; }
		}

		public class WithDest : Instruction
		{
			public Action<Engine, int> Execute { get; set; }
		}

		public class WithOp1Op2Dest : Instruction
		{
			public Action<Engine, int, int, int> Execute { get; set; }
		}
	}
}
