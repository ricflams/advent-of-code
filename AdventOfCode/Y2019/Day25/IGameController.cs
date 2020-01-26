namespace AdventOfCode.Y2019.Day25
{
	internal interface IGameController
	{
		string WhatNext(Game game);
		void OnGameOver(Game game);
	}
}
