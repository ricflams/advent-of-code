using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Day25.Game
{
	internal interface IGameController
	{
		string Command(Maze maze);
	}
}
