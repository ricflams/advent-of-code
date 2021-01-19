using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day17
{
	internal class Puzzle : Puzzle<int, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Set and Forget";
		public override int Year => 2019;
		public override int Day => 17;

		public void Run()
		{
			RunFor("input", 5056, 942367);
		}

		protected override int Part1(string[] input)
		{
			var map = CreateMap(input[0]);
			//map.ConsoleWrite();

			var intersections = map.AllPoints(ch => ch == '#')
				.ToList()
				.Where(IsIntersection)
				.Sum(x => x.X * x.Y);
			return intersections;

			bool IsIntersection(Point p) =>
				'#' == map[p.Up] &&
				'.' == map[p.Up.Right] &&
				'#' == map[p.Right] &&
				'.' == map[p.Down.Right] &&
				'#' == map[p.Down] &&
				'.' == map[p.Down.Left] &&
				'#' == map[p.Left] &&
				'.' == map[p.Up.Left];
		}

		protected override long Part2(string[] input)
		{
			var intcode = input[0];
			var map = CreateMap(intcode);
			var path = CalculatePath(map);
			var programs = GenerateAsciiProgram(path.AsMovement(), path.ToList(), new List<string>());
			programs.Add("n");

			//Console.WriteLine(command);
			//foreach (var p in programs)
			//{
			//	Console.WriteLine(p);
			//}

			var engineinput = string.Concat(programs.Select(x => x + "\n")).Select(ch => (long)ch);
			var dust = new Engine()
				.WithMemory(intcode)
				.WithMemoryValueAt(0, 2)
				.WithInput(engineinput.ToArray())
				.Execute()
				.Output
				.TakeAll()
				.Last();

			return dust;
		}


		private static List<string> GenerateAsciiProgram(string fullpath, List<string> path, List<string> moves)
		{
			const int MaxCommandLength = 20;
			var moveNames = new string[] { "A", "B", "C" };

			if (moves.Count == moveNames.Length)
			{
				var reduce = fullpath;
				for (var i = 0; i < moveNames.Length; i++)
				{
					reduce = reduce.Replace(moves[i], moveNames[i]);
				}
				return reduce.Length > MaxCommandLength
					? null
					: moves.Prepend(reduce).ToList();
			}

			for (var offset = 0; path.GetRange(0, offset).AsMovement().Length < MaxCommandLength; offset++)
			{
				for (var len = 1; ; len++)
				{
					var head = path.GetRange(offset, len).AsMovement();
					if (head.Length > MaxCommandLength) // too wide
						break;
					var tail = path.GetRange(offset + len, path.Count() - (offset + len));
					moves.Add(head);
					var asciiProgram = GenerateAsciiProgram(fullpath, tail, moves);
					if (asciiProgram != null)
					{
						return asciiProgram;
					}
					moves.RemoveAt(moves.Count - 1);
				}
			}

			return null;
		}

		private static CharMap CreateMap(string intcode)
		{
			var map = new CharMap();
			var pos = Point.From(0, 0);
			new Engine()
				.WithMemory(intcode)
				.OnOutput(engine =>
				{
					var ch = (char)engine.Output.Take();
					if (ch == '\n')
					{
						pos = Point.From(0, pos.Y + 1);
					}
					else
					{
						map[pos] = ch;
						pos = pos.Right;
					}
				})
				.Execute();
			return map;
		}

		private static string[] CalculatePath(CharMap map)
		{
			var pos = map.AllPoints(ch => "^v<>".Contains(ch)).First();
			var vc = map[pos];
			var direction =
				vc == '^' ? Direction.Up :
				vc == '>' ? Direction.Right :
				vc == 'v' ? Direction.Down :
				vc == '<' ? Direction.Left : 0; // 0 can't happen

			var path = new List<string>();
			var stretch = 0;
			while (true)
			{
				var originalDirection = direction;
				if (map[pos.Move(direction)] == '#')
				{
					pos = pos.Move(direction);
					stretch++;
					continue;
				}
				if (stretch > 0)
				{
					path.Add(stretch.ToString());
					stretch = 0;
				}
				if (map[pos.Move(direction.TurnLeft())] == '#')
				{
					direction = direction.TurnLeft();
					path.Add("L");
				}
				else if (map[pos.Move(direction.TurnRight())] == '#')
				{
					direction = direction.TurnRight();
					path.Add("R");
				}
				else
				{
					return path.ToArray();
				}
				//Console.WriteLine(path);
			}
		}
	}

	static class Extensions
	{
		internal static string AsMovement(this IEnumerable<string> list) => string.Join(",", list);
	}
}

