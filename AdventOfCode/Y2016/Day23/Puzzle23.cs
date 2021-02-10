using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2016.Assembunny;

namespace AdventOfCode.Y2016.Day23
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Safe Cracking";
		public override int Year => 2016;
		public override int Day => 23;

		public void Run()
		{
			RunPart1For("test1", 3);
			RunFor("input", 12762, 479009322);
		}

		protected override int Part1(string[] input)
		{
			var comp = new Computer(input);
			comp.Regs[0] = 7;
			comp.Run();
			return comp.Regs[0];
		}

		protected override int Part2(string[] input)
		{
			Computer.OptimizeFragment(input,
				new[]
				{
					"cpy 0 a",
					"cpy b c",
					"inc a",
					"dec c",
					"jnz c -2",
					"dec d",
					"jnz d -5"
				},
				new[]
				{
					"mul b d a",
					"cpy 0 c",
					"cpy 0 d"
				});
			Computer.OptimizeFragment(input,
				new[]
				{
					"cpy b c",
					"cpy c d",
					"dec d",
					"inc c",
					"jnz d -2"
				},
				new[]
				{
					"mul b 2 c",
					"cpy 0 d"
				});
			var comp = new Computer(input);
			comp.Regs[0] = 12;
			comp.Run();
			return comp.Regs[0];
		}



	}

	// Only lines marked by * are toggled at runtime, rest remain
	// fixed and are therefore safe to modify:
	//     cpy a b
	//     dec b
	//     cpy a d
	//     cpy 0 a
	//     cpy b c
	//     inc a
	//     dec c
	//     jnz c -2
	//     dec d
	//     jnz d -5
	//     dec b
	//     cpy b c
	//     cpy c d
	//     dec d
	//     inc c
	//     jnz d -2
	//     tgl c
	//     cpy -16 c
	//   * jnz 1 c
	//     cpy 78 c
	//   * jnz 99 d
	//     inc a
	//   * inc d
	//     jnz d -2
	//   * inc c
	//     jnz c -5		

	// Two multiplication optimizations:
	//     a = 0             =>  a = b * d       
	//     do                    c = 0
	//       c = b               d = 0
	//       do
	//         a++
	//         c--
	//       while c != 0
	//       d--
	//     while d != 0
	//     ....
	//     c = b              =>  c = b * 2
	//     d = c                  d = 0
	//     do
	//       d--
	//       c++
	//     while d != 0
}
