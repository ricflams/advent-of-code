using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Beverage Bandits";
		public override int Year => 2018;
		public override int Day => 15;

		public void Run()
		{
			Run("test1").Part1(27730).Part2(4988);
			Run("test2").Part1(36334);
			Run("test3").Part1(39514).Part2(31284);
			Run("test4").Part1(27755).Part2(3478);
			Run("test5").Part1(28944).Part2(6474);
			Run("test6").Part1(18740).Part2(1140);
			Run("input").Part1(198531).Part2(90420);

			// 218610 too high?!
			// 216528 too high
			// 199070 too high

		}

		protected override int Part1(string[] input)
		{
			var combat = new Combat(input, 3);

			while (!combat.Completed)
			{
				combat.BattleRound();
			}
			return combat.Outcome;

			// var rounds = 0;
			// while (true)
			// {
			// 	combat.BattleRound();
			// 	if (combat.Completed)
			// 		break;
			// 	rounds++;
			// }

			// //rounds--;

			// var hp = combat._units.Where(u => u.IsAlive).Sum(u => u.Hitpoints);
			// var outcome = rounds * hp;
			//return outcome;
		}

		protected override int Part2(string[] input)
		{
			for (var attack = 4; ; attack++)
			{
				var combat = new Combat(input, attack);
				while (!combat.Completed)
				{
					combat.BattleRound();
				}
				if (combat.AllElfsSurvived)
				{
					return combat.Outcome;
				}
			}
		}

		internal class Combat
		{
			private readonly CharMap _map;
			public Unit[] _units;
			private readonly int _initialElfCount;

			public Combat(string[] input, int elfAttack)
			{
				_map = CharMap.FromArray(input);

				// Find all units and remove them from the map
				_units = _map
					.AllPoints(c => c == 'E' || c == 'G')
					.Select(p => new Unit(p, _map[p], _map[p] == 'E' ? elfAttack : 3))
					.ToArray();
				// foreach (var u in _units)
				// {
				// 	_map[u.Position] = '.';
				// }

				_initialElfCount = _units.Count(u => u.Kind == 'E');

				Completed = false;
			}

			internal class Unit
			{
				public Unit(Point p, char kind, int attack) => (Position, Kind, Attack, Hitpoints) = (p, kind, attack, 200);
				public char Kind { get; set; }
				public Point Position { get; set; }
				public int Attack { get; set; }
				public int Hitpoints { get; set; }
				public bool IsAlive => Hitpoints > 0;
				public bool IsDead => !IsAlive;
			}

			public bool Completed { get; private set; }
			public bool AllElfsSurvived { get; private set; }
			public int Outcome { get; private set; }

			private int _rounds = 0;

			public void BattleRound()
			{
				// Units take turns in this order
				_units = _units
					.Where(u => u.IsAlive)
					.OrderBy(u => u.Position.Y)
					.ThenBy(u => u.Position.X)
					.ToArray();

				// Each unit still alive take its turn
				foreach (var unit in _units)
				{
					if (unit.IsDead)
						continue;

					// Find targets; if none are left then combat is over
					var targets = _units.Where(u => u.IsAlive && u.Kind != unit.Kind).ToArray();
					if (!targets.Any())
					{
						Completed = true;
						var hp = _units.Where(u => u.IsAlive).Sum(u => u.Hitpoints);
						Outcome = _rounds * hp;
						AllElfsSurvived = _units.Where(u => u.IsAlive).Count(u => u.Kind == 'E') == _initialElfCount;

						// ConsoleWrite();
						// Console.WriteLine($"Outcome: {_rounds} * {hp} = {Outcome}");
						// Console.WriteLine();
						// Winner = _units.First().Kind;
						break;
					}

					// if (unit.IsDead)
					// 	continue;



					// Look around. If there are units to attack then do that; else move
					var enemy = unit.Kind == 'E' ? 'G' : 'E';
					var attackables = unit.Position
						.LookAround()
						.Where(p => _map[p] == enemy)
						.Select(p => targets.Single(t => t.Position == p))
						.ToArray();
					if (!attackables.Any())
					{
						// Move
						var step = Move(unit, targets);
						if (step != null)
						{
							_map[unit.Position] = '.';
							unit.Position = step;
							_map[unit.Position] = unit.Kind;
							attackables = unit.Position
								.LookAround()
								.Where(p => _map[p] == enemy)
								.Select(p => targets.Single(t => t.Position == p))
								.ToArray();
						}
					}

					if (attackables.Any())
					{
						// Attack
						var hp = attackables.Min(t => t.Hitpoints);
						var weakest = attackables
							.Where(t => t.Hitpoints == hp)
							.OrderBy(u => u.Position.Y)
							.ThenBy(u => u.Position.X)
							.First();
						weakest.Hitpoints -= unit.Attack;
						if (weakest.IsDead)
						{
							_map[weakest.Position] = '.';
						}
					}
				}

				_rounds++;
				//ConsoleWrite();
			}

			private Point Move(Unit unit, Unit[] targets)
			{
				// Find all open squares in range of each target
				var inrange = targets
					.SelectMany(target => target.Position.LookAround().Select(adjacent => (target, adjacent)))
					.Where(p => _map[p.adjacent] == '.')
					.ToArray();

				var reachable = inrange
					.SelectMany(p => ShortestPathFromTo(unit.Position, p.adjacent, p.target.Position))
					.ToArray();
				if (!reachable.Any())
					return null;

				var mindist = reachable.Min(x => x.Distance);
				var nearest = reachable
					.Where(x => x.Distance == mindist)
					.ToArray();

				var chosen = nearest
					.OrderBy(x => x.Adjacent.Y)
					.ThenBy(x => x.Adjacent.X)
					.First();

				var step = nearest
					//.Where(x => x.Adjacent == chosen)
					//.Where(x => x.TargetUnit == chosen.TargetUnit)
					.Where(x => x.Adjacent == chosen.Adjacent)
					.OrderBy(x => x.FirstStep.Y)
					.ThenBy(x => x.FirstStep.X)
					.First().FirstStep;
				
				return step;
			}

			private void ConsoleWrite()
			{
				Console.WriteLine($"After {_rounds} rounds:");
				_map.ConsoleWrite();
				var (min, max) = _map.Area();
				for (var y = min.Y; y <= max.Y; y++)
				{
					for (var x = min.X; x <= max.X; x++)
					{
						var u = _units.Where(u => u.IsAlive && u.Position == Point.From(x, y)).FirstOrDefault();
						if (u != null)
						{
							Console.Write($"{u.Kind}({u.Hitpoints}) ");
						}
					}
					Console.WriteLine();
				}
			}

			struct PathInfo
			{
				public Point FirstStep;
				public Point Adjacent;
				public Point TargetUnit;
				public int Distance;
			}

			private List<PathInfo> ShortestPathFromTo(Point from, Point adjacent, Point targetunit)
			{
				// Firststep, dest, target
				var queue = new Queue<(Point, Point, int, HashSet<Point>)>();

				// var seen = new HashSet<Point>();
				// //var seen = new Dictionary<Point, int>();

				foreach (var p in from.LookAround().Where(p => _map[p] == '.'))
				{
					var seen = new HashSet<Point>();
					queue.Enqueue((p, p, 1, seen));
					seen.Add(p);
					//seen[p] = 1;
				}

				var shorteststeps = new List<PathInfo>();
				var mindist = int.MaxValue;

				// queue.Enqueue((from, 0));
				while (queue.Any())
				{
					var (firststep, pos, dist, seen) = queue.Dequeue();
					if (dist > mindist)
						continue;
					if (pos == adjacent)
					{
						if (mindist > dist && shorteststeps.Any())
							throw new Exception();
						mindist = dist;
						shorteststeps.Add(new PathInfo
						{
							FirstStep = firststep,
							Adjacent = pos,
							TargetUnit = targetunit,
							Distance = dist
						});
						continue;
					}
					var adjacents = pos.LookAround().Where(p => _map[p] == '.' && !seen.Contains(p)).ToArray();
					//var adjacents = pos.LookAround().Where(p => _map[p] == '.' && (!seen.ContainsKey(p) || seen[p] > dist+1)).ToArray();
					foreach (var p in adjacents)
					{
						queue.Enqueue((firststep, p, dist+1, seen));
						seen.Add(p);
						//seen[p] = dist+1;
					}
				}

				if (!shorteststeps.Any())
					return new List<PathInfo>(); // enumerable empty

				return shorteststeps;
			}


		}

	}
}
