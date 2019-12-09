using System;
using System.Numerics;

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
			public Action<Engine, BigInteger> Execute { get; set; }
		}

		public class WithOp1Op2 : Instruction
		{
			public Action<Engine, BigInteger, BigInteger> Execute { get; set; }
		}

		public class WithDest : Instruction
		{
			public Action<Engine, BigInteger> Execute { get; set; }
		}

		public class WithOp1Op2Dest : Instruction
		{
			public Action<Engine, BigInteger, BigInteger, BigInteger> Execute { get; set; }
		}
	}
}
