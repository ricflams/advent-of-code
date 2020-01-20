using System;

namespace AdventOfCode2019.Day25
{
	internal class RawController : IGameController
	{
		public string WhatNext(Game game)
		{
			Console.WriteLine(game.RawOutput);
			return Console.ReadLine();
		}

		public void OnGameOver(Game game)
		{
			Console.WriteLine(game.RawOutput);
		}
	}
}
