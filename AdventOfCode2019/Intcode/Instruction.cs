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

		public class WithOp : Instruction
		{
			public Action<Engine, long> Execute { get; set; }
		}

		public class WithOpOp : Instruction
		{
			public Action<Engine, long, long> Execute { get; set; }
		}

		public class WithPos : Instruction
		{
			public Action<Engine, long> Execute { get; set; }
		}

		public class WithOpOpPos : Instruction
		{
			public Action<Engine, long, long, long> Execute { get; set; }
		}
	}
}
