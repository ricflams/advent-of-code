using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Y2019.Intcode;

namespace AdventOfCode.Y2019.Day15
{
    internal static class Puzzle15
    {
		private const int MoveNone = 0;
		private const int MoveNorth = 1;
		private const int MoveSouth = 2;
		private const int MoveEast = 3;
		private const int MoveWest = 4;
		private const int StatusHitTheWall = 0;
		private const int StatusMoved = 1;
		private const int StatusFoundOxygen = 2;
		const char MapSpace = '.';
		const char MapWall = '#';
		const char MapDroid = 'D';
		const char MapOxygen = 'O';

		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var map = new CharMap();

			var movements = new MoveGenerator();
			map[movements.Current.Position] = MapSpace;

			var stepsToOxygen = 0;
			var debug = false;

			var engine2 = new Engine()
				.WithMemoryFromFile("Y2019/Day15/input.txt")
				.OnInput(engine =>
				{
					var movement = movements.NextProposal(map);
					if (movement == MoveNone)
					{
						engine.Halt = true;
					}
					if (debug)
					{
						Console.Clear();
						Console.WriteLine($"Moves: {movements}");
						foreach (var line in map.Render(MapOverlay))
						{
							Console.WriteLine(line);
						}
						Console.ReadKey();
					}
					engine.Input.Add(movement);
				})
				.OnOutput(engine =>
				{
					var status = engine.Output.Take();
					switch (status)
					{
						case StatusHitTheWall:
							map[movements.ProposedPosition] = MapWall;
							break;
						case StatusMoved:
							movements.ApproveMove();
							map[movements.Current.Position] = MapSpace;
							break;
						case StatusFoundOxygen:
							movements.ApproveMove();
							map[movements.Current.Position] = MapOxygen;
							stepsToOxygen = movements.Moves;
							break;
					}
				})
				.Execute();
			Console.WriteLine($"Day 15 Puzzle 1: {stepsToOxygen}");
			Debug.Assert(stepsToOxygen == 300);

			var minutes = 0;
			while (map.AllPoints(val => val == MapSpace).Count() > 0)
			{
				foreach (var p in map.AllPoints(val => val == MapOxygen).ToList())
				{
					foreach (var d in AllDirections())
					{
						var neighboor = MoveGenerator.MoveFrom(p, d);
						if (map[neighboor] == MapSpace)
						{
							map[neighboor] = MapOxygen;
						}
					}
				}
				minutes++;
			}
			Console.WriteLine($"Day 15 Puzzle 2: {minutes}");
			Debug.Assert(minutes == 312);

			// Draw Droid on top of map
			char MapOverlay(Point p, char val)
			{
				var droid = movements.Current?.Position;
				return p == droid ? 'D' : val;
			}
		}

		private static IEnumerable<int> AllDirections()
		{
			yield return MoveNorth;
			yield return MoveEast;
			yield return MoveSouth;
			yield return MoveWest;
		}

		private class MoveGenerator
		{
			private static readonly Point StartPosition = Point.From(0, 0);

			public class Move
			{
				public Point Position { get; set; }
				public int Direction { get; set; }
			}

			private Stack<Move> _moves = new Stack<Move>();
			public int Moves => _moves.Count() - 1;

			public Move Current => _moves.TryPeek(out var move) ? move : null;
			public Point ProposedPosition => MoveFrom(Current.Position, Current.Direction);
			private Move _pendingMove;

			public MoveGenerator()
			{
				_moves.Push(new Move
				{
					Position = StartPosition,
					Direction = MoveNone
				});
			}

			public static Point MoveFrom(Point p, int direction)
			{
				switch (direction)
				{
					case MoveNorth: return p.Up;
					case MoveEast: return p.Right;
					case MoveSouth: return p.Down;
					case MoveWest: return p.Left;
				}
				throw new Exception($"{nameof(MoveFrom)} fail for {direction}");
			}

			private int DirectionTo(Point p1, Point p2)
			{
				return AllDirections().First(d => MoveFrom(p1, d) == p2);
			}

			public int NextProposal(CharMap map)
			{
				while (true)
				{
					if (Current.Direction == MoveWest)
					{
						// Dead end, so backtrack
						_pendingMove = null;
						var pos = Current.Position;
						if (pos == StartPosition)
						{
							return MoveNone; // We're done
						}
						_moves.Pop();
						return DirectionTo(pos, Current.Position);
					}

					while (Current.Direction < MoveWest)
					{
						Current.Direction++;
						if (map[ProposedPosition] == 0)
						{
							_pendingMove = new Move
							{
								Position = ProposedPosition,
								Direction = MoveNone
							};
							return Current.Direction;
						}
					}
				}
			}

			public void ApproveMove()
			{
				if (_pendingMove != null)
				{
					_moves.Push(_pendingMove);
				}
			}
			public override string ToString() => $"{string.Join(" ", _moves.Select(x => x.Direction))} [{Current.Direction}]";
		}
	}
}

