using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day25
{
	internal static class Puzzle25
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var game = new Game.Maze()
					.WithController(new Game.UserGameController())
					.Run();
			//while (true)
			//{
			//	var game = new Game()
			//		.WithController(UserPaddleControl)
			//		.Run();
			//}


			////var shortestPath = 0;// ShortestPath(maze);
			////Console.WriteLine($"Day 20 Puzzle 1: {shortestPath}");
			////			Debug.Assert(damage = 19354083);

			//string UserPaddleControl(Game g)
			//{
			//	return Console.ReadLine();
			//}

			//Console.WriteLine($"Day 24 Puzzle 1: {}");
			//Debug.Assert(beampoints == 141);
		}

		private static void Puzzle2()
		{
			//Console.WriteLine($"Day 24 Puzzle 2: {}");
			//Debug.Assert(beampoints == 141);
		}



	}

}

