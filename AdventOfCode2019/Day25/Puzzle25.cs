using System;
using System.Diagnostics;

namespace AdventOfCode2019.Day25
{
	internal static class Puzzle25
	{
		public static void Run()
		{
			Puzzle1();
		}

		private static void Puzzle1()
		{
			//while (true)
			//{
			//	new Game()
			//		.WithController(new NethackishController())
			//		//.WithController(new RawController())
			//		.Run();
			//}

			var password = new Game()
					.WithController(new AutoplayController())
					.Run()
					.Password;
			Console.WriteLine($"Day 25 Puzzle 1: {password}");
			Debug.Assert(password == 33624080);
		}
	}
}

