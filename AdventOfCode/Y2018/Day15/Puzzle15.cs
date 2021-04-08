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
			//var combat = new Combat(input, 17);
			//while (!combat.Completed)
			//{
			//	combat.BattleRound();
			//}
			//return combat.Outcome;

			var frontiercount = 0;


			// var attack = 4;
			// while (true)
			// {
			// 	var combat = new Combat(input, attack);
			// 	while (!combat.Completed)
			// 	{
			// 		combat.BattleRound();
			// 	}
			// 	frontiercount += combat._frontierCount;
			// 	if (combat.AllElfsSurvived)
			// 		break;
			// 	attack *= 2;
			// }

			// // var min = attack / 2;
			// // var max = attack;
			// // var outcome = 0;
			// // while (max - min > 2)
			// // {
			// // 	attack = (max + min) / 2;
			// // 	var combat = new Combat(input, attack);
			// // 	while (!combat.Completed)
			// // 	{
			// // 		combat.BattleRound();
			// // 	}
			// // 	frontiercount += combat._frontierCount;
			// // 	if (combat.AllElfsSurvived)
			// // 	{
			// // 		max = attack - 1;
			// // 		outcome = combat.Outcome;
			// // 	}
			// // 	else
			// // 	{
			// // 		min = attack
			// // 	}
			// // }

			// // 	{
			// // 		Console.WriteLine($"Frontiers: {frontiercount}");
			// // 		return combat.Outcome;
			// // 	}



			for (var attack = 4;; attack++)
			{
				var sw = Stopwatch.StartNew();
				var combat = new Combat(input, attack);
				//Console.WriteLine($"Attack {attack}");
				while (!combat.Completed && combat.AllElfsSurvived)
				{
					combat.BattleRound();
				}
				frontiercount += combat._frontierCount;
				var win = combat.AllElfsSurvived;
				//Console.WriteLine($"Attack {attack}: win={win} elapsed={sw.ElapsedMilliseconds}");
				if (combat.AllElfsSurvived)
				{
					//Console.WriteLine($"Frontiers: {frontiercount}");
					return combat.Outcome;
				}
			}



// for (var N = 1; N < Environment.ProcessorCount; N++)
// {


// 			var minAttack = 4;
// //			var N = (int)(Environment.ProcessorCount*.8);
// var sw = Stopwatch.StartNew();
// 			while (true)
// 			{
// 				var win = Enumerable.Range(minAttack, N)
// 					.AsParallel()
// 					//.WithDegreeOfParallelism(N)
// 					.Select(attack =>
// 					{
// 						var combat = new Combat(input, attack);
// 						while (!combat.Completed && combat.AllElfsSurvived)
// 						{
// 							combat.BattleRound();
// 						}
// 						return new
// 						{
// 							attack,
// 							combat
// 						};
// 					})
// 					.Where(x => x.combat.AllElfsSurvived)
// 					.OrderBy(x => x.attack)
// 					.FirstOrDefault();
// 				if (win != null)
// 				{
// 					break;
// ///					return win.combat.Outcome;
// 				}
// 				minAttack += N;
// 			}
// 		Console.WriteLine($"N={N} elapsed=={sw.ElapsedMilliseconds}");
// }
// return 0;

			//var outcome = 0;
			//Guess.FindLowest(4, attack =>
			//{
			//	var combat = new Combat(input, attack);
			//	while (!combat.Completed)
			//	{
			//		combat.BattleRound();
			//	}
			//	frontiercount += combat._frontierCount;
			//	outcome = combat.Outcome;
			//	var win = combat.AllElfsSurvived;
			//	Console.WriteLine($"Attack {attack}: win={win}");
			//	return win;
			//});

			//return outcome;

		}

		internal class Combat
		{
			private readonly CharMap _map;
			public Unit[] _units;
			private readonly int _initialElfCount;
			private readonly int _width;
			private readonly int _height;

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

				var (_, max) = _map.Area();
				_width = max.X + 1;
				_height = max.Y + 1;


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
			public bool AllElfsSurvived => _units.Where(u => u.IsAlive).Count(u => u.Kind == 'E') == _initialElfCount;
			public int Outcome { get; private set; }

			private int _rounds = 0;

			public void BattleRound()
			{
				var moves = 0;
				var cantmoves = 0;
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
					var targets = _units
						.Where(u => u.IsAlive && u.Kind != unit.Kind)
						.ToDictionary(x => x.Position, x => x);
					if (!targets.Any())
					{
						Completed = true;
						var hp = _units.Where(u => u.IsAlive).Sum(u => u.Hitpoints);
						Outcome = _rounds * hp;

						// ConsoleWrite();
						// Console.WriteLine($"Outcome: {_rounds} * {hp} = {Outcome}");
						// Console.WriteLine();
						// Winner = _units.First().Kind;
						//Console.WriteLine($"Frontiers: {_frontierCount}");
						break;
					}

					// if (unit.IsDead)
					// 	continue;



					// Look around. If there are units to attack then do that; else move
					//var enemy = unit.Kind == 'E' ? 'G' : 'E';
					var attackables = unit.Position
						.LookAround()
						.Select(p => targets.TryGetValue(p, out var u) ? u : null)
						.Where(t => t != null)
						// .Where(p => _map[p] == enemy)
						// .Select(p => targets.Single(t => t.Position == p))
						.ToArray();
					if (!attackables.Any())
					{
						// Move
						var step = Move(unit, targets.Values.ToArray());
						if (step != null)
						{
							_map[unit.Position] = '.';
							unit.Position = step;
							_map[unit.Position] = unit.Kind;
							attackables = unit.Position
								.LookAround()
								.Select(p => targets.TryGetValue(p, out var u) ? u : null)
								.Where(t => t != null)
								// .Where(p => _map[p] == enemy)
								// .Select(p => targets.Single(t => t.Position == p))
								.ToArray();
							// attackables = unit.Position
							// 	.LookAround()
							// 	.Where(p => _map[p] == enemy)
							// 	.Select(p => targets.Single(t => t.Position == p))
							// 	.ToArray();
//							_distancesCache.Clear();
							moves++;
						}
						else
							cantmoves++;
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
//							_distancesCache.Clear();
						}
					}
				}

				_rounds++;
//				Console.WriteLine($"Round {_rounds}: units:{_units.Count()} moves:{moves} cantmoves:{cantmoves}");
//				ConsoleWrite();
			}



			public int _frontierCount = 0;

			// private Dictionary<Point, Dictionary<Point, Dictionary<Point, int>>> _distancesCache = new Dictionary<Point, Dictionary<Point, Dictionary<Point, int>>>();

			private Point Move(Unit unit, Unit[] targets)
			{
				// Find all open squares in range of each target
				var inrange = targets
					.SelectMany(t => t.Position.LookAround())
					.Where(p => _map[p] == '.')
					.Distinct()
					.ToHashSet();

				// if (!_distancesCache.TryGetValue(unit.Position, out var distances))
				// {
					// Calculate at once distancefield from the unit
					//var lookAround = new Dictionary<Point, Point[]>();
					var lookAround = new List<(int,int)>[_width,_height];
					var inrange2 = new bool[_width,_height];
					foreach (var p in inrange)
					{
						inrange2[p.X, p.Y] = true;
					}
					int minDistance = int.MaxValue;
					var distances = unit.Position
						.LookAround()
						.Where(p => _map[p] == '.')
						.ToDictionary(p => p, p => CalcDistanceField(p));

					//_distancesCache[unit.Position] = distances;

					Dictionary<Point, int> CalcDistanceField(Point start)
					{
						//var frontier = new Queue<(int, int)>();
						var frontier = new Queue<(int, int)>();
						var distance = new int[_width,_height];
						distance[start.X, start.Y] = 1;
						frontier.Enqueue((start.X, start.Y));
						while (frontier.Any())
						{
							var (x, y) = frontier.Dequeue();
							var dist = distance[x, y];
							if (dist >= minDistance)
								break;
							var adjacent = lookAround[x, y];
							if (adjacent == null)
							{
								adjacent = lookAround[x, y] = new List<(int, int)>();
								if (_map[x-1][y] == '.') adjacent.Add((x-1, y));
								if (_map[x+1][y] == '.') adjacent.Add((x+1, y));
								if (_map[x][y-1] == '.') adjacent.Add((x, y-1));
								if (_map[x][y+1] == '.') adjacent.Add((x, y+1));
							}
							foreach (var (nx, ny) in adjacent)
							{
								if (distance[nx, ny] > 0)
									continue;
								distance[nx, ny] = dist + 1;
								if (inrange2[nx, ny])
								{
									minDistance = dist + 1;
									break;
								}
								frontier.Enqueue((nx, ny));
								//_frontierCount++;
							}
						}

						var distdict = inrange
							.Where(p => distance[p.X, p.Y] > 0)
							.ToDictionary(p => p, p => distance[p.X, p.Y]);

						return distdict;
					}

				// }
				// // else
				// // 	Console.Write(".");


				var reachables3 = inrange
					.Where(p => distances.Values.Any(d => d.ContainsKey(p)))
					.ToHashSet();
				if (!reachables3.Any())
					return null;

				var mindistances = reachables3
					.ToDictionary(p => p, p => distances.Values.Min(d => d.TryGetValue(p, out var dist) ? dist : int.MaxValue));
				var mindist3 = mindistances.Values.Min();
				var nearest3 = reachables3
					.Where(p => mindistances[p] == mindist3);

				var chosen3 = nearest3
					.OrderBy(p => p.Y)
					.ThenBy(p => p.X)
					.First();

				var step3 = distances
					.Where(d => (d.Value.TryGetValue(chosen3, out var dist) ? dist : int.MaxValue) == mindist3)
					.Select(d => d.Key)
					.OrderBy(p => p.Y)
					.ThenBy(p => p.X)
					.First();

				return step3;



				// // var reachable = inrange
				// // 	.SelectMany(p => ShortestPathFromTo(unit.Position, p))
				// // 	.ToArray();
				// // if (!reachable.Any())
				// // 	return null;

				// // var mindist = reachable.Min(x => x.Distance);
				// // var nearest = reachable
				// // 	.Where(x => x.Distance == mindist)
				// // 	.ToArray();

				// // var chosen = nearest
				// // 	.OrderBy(x => x.Adjacent.Y)
				// // 	.ThenBy(x => x.Adjacent.X)
				// // 	.First();

				// // var step = nearest
				// // 	.Where(x => x.Adjacent == chosen.Adjacent)
				// // 	.OrderBy(x => x.FirstStep.Y)
				// // 	.ThenBy(x => x.FirstStep.X)
				// // 	.First().FirstStep;
				
				// // return step;
			}

			private void ConsoleWrite()
			{
				Console.Clear();
				Console.WriteLine($"After {_rounds} rounds:");
				_map.ConsoleWrite();
				// var (min, max) = _map.Area();
				// for (var y = min.Y; y <= max.Y; y++)
				// {
				// 	for (var x = min.X; x <= max.X; x++)
				// 	{
				// 		var u = _units.Where(u => u.IsAlive && u.Position == Point.From(x, y)).FirstOrDefault();
				// 		if (u != null)
				// 		{
				// 			Console.Write($"{u.Kind}({u.Hitpoints}) ");
				// 		}
				// 	}
				// 	Console.WriteLine();
				// }
				Console.ReadKey();
			}

			// struct PathInfo
			// {
			// 	public Point FirstStep;
			// 	public Point Adjacent;
			// 	public int Distance;
			// }

			// private Dictionary<(Point, Point), List<PathInfo>> shortestPathCache = new Dictionary<(Point, Point), List<PathInfo>>();

			// private List<PathInfo> ShortestPathFromTo(Point from, Point adjacent)
			// {
			// 	if (shortestPathCache.TryGetValue((from, adjacent), out var result))
			// 	{
			// 		Console.Write(".");
			// 		return result;
			// 	}

			// 	// Firststep, dest, target
			// 	var queue = new Queue<(Point, Point, int, HashSet<Point>)>();

			// 	// var seen = new HashSet<Point>();
			// 	// //var seen = new Dictionary<Point, int>();

			// 	foreach (var p in from.LookAround().Where(p => _map[p] == '.'))
			// 	{
			// 		var seen = new HashSet<Point>();
			// 		queue.Enqueue((p, p, 1, seen));
			// 		seen.Add(p);
			// 		//seen[p] = 1;
			// 	}

			// 	var shorteststeps = new List<PathInfo>();
			// 	var mindist = int.MaxValue;

			// 	// queue.Enqueue((from, 0));
			// 	while (queue.Any())
			// 	{
			// 		var (firststep, pos, dist, seen) = queue.Dequeue();
			// 		if (dist > mindist)
			// 			continue;
			// 		if (pos == adjacent)
			// 		{
			// 			if (mindist > dist && shorteststeps.Any())
			// 				throw new Exception();
			// 			mindist = dist;
			// 			shorteststeps.Add(new PathInfo
			// 			{
			// 				FirstStep = firststep,
			// 				Adjacent = pos,
			// 				Distance = dist
			// 			});
			// 			continue;
			// 		}
			// 		// if (dist == mindist)
			// 		// 	continue;
			// 		var adjacents = pos.LookAround().Where(p => _map[p] == '.' && !seen.Contains(p)).ToArray();
			// 		//var adjacents = pos.LookAround().Where(p => _map[p] == '.' && (!seen.ContainsKey(p) || seen[p] > dist+1)).ToArray();
			// 		foreach (var p in adjacents)
			// 		{
			// 			queue.Enqueue((firststep, p, dist+1, seen));
			// 			seen.Add(p);
			// 			//seen[p] = dist+1;
			// 		}
			// 	}

			// 	shortestPathCache[(from, adjacent)] = shorteststeps;

			// 	return shorteststeps;
			// }

		}

	}
}
