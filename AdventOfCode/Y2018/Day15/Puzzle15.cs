using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Beverage Bandits";
		public override int Year => 2018;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(27730).Part2(4988);
			Run("test2").Part1(36334);
			Run("test3").Part1(39514).Part2(31284);
			Run("test4").Part1(27755).Part2(3478);
			Run("test5").Part1(28944).Part2(6474);
			Run("test6").Part1(18740).Part2(1140);
			Run("input").Part1(198531).Part2(90420);
		}

		protected override int Part1(string[] input)
		{
			var combat = new Combat(input, 3);

			while (!combat.Completed)
			{
				combat.BattleRound();
			}
			return combat.Outcome;
		}

		protected override int Part2(string[] input)
		{
			for (var attack = 4;; attack++)
			{
				var combat = new Combat(input, attack);
				while (!combat.Completed && combat.AllElfsSurvived)
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
			private readonly int _width;
			private readonly int _height;
			private Unit[] _units;
			private int _rounds = 0;

			internal class Unit
			{
				public Unit(Point p, char kind, int attack) => (Position, Kind, Attack, Hitpoints) = (p, kind, attack, 200);
				public char Kind { get; set; }
				public Point Position { get; set; }
				public int Attack { get; set; }
				public int Hitpoints { get; set; }
				public bool IsAlive => Hitpoints > 0;
			}

			public Combat(string[] input, int elfAttack)
			{
				_map = CharMap.FromArray(input);

				// Find all units and the size of the map
				_units = _map
					.AllPoints(c => c == 'E' || c == 'G')
					.Select(p => new Unit(p, _map[p], _map[p] == 'E' ? elfAttack : 3))
					.ToArray();
				var (_, max) = _map.MinMax();
				_width = max.X + 1;
				_height = max.Y + 1;

				// For debugging, print out the initial position
				ConsoleWrite();
			}

			public bool Completed { get; private set; }
			public bool AllElfsSurvived { get; private set; } = true;
			public int Outcome { get; private set; }

			public void BattleRound()
			{
				// Units take turns in this order. Also, remove dead units now.
				_units = _units
					.Where(u => u.IsAlive)
					.OrderBy(u => u.Position.Y)
					.ThenBy(u => u.Position.X)
					.ToArray();

				// Each unit still alive take its turn
				foreach (var unit in _units)
				{
					// Check liveness inside loop, as unit will die during the round
					if (!unit.IsAlive)
						continue;

					// Find targets; if none are left then combat is over
					var targets = _units
						.Where(u => u.IsAlive && u.Kind != unit.Kind)
						.ToDictionary(x => x.Position, x => x);
					if (!targets.Any())
					{
						Completed = true;
						var totalHitpoints = _units.Where(u => u.IsAlive).Sum(u => u.Hitpoints);
						Outcome = _rounds * totalHitpoints;
						break;
					}

					// Look around for any attackable targets adjacent to this unit
					var attackables = unit.Position
						.LookAround()
						.Select(p => targets.TryGetValue(p, out var u) ? u : null)
						.Where(t => t != null)
						.ToArray();
					if (!attackables.Any())
					{
						// Nothing to attack so attempt to move instead. If a move is possible
						// then do so and update the attackables for this new position.
						var step = Move(unit.Position, targets.Keys);
						if (step != null)
						{
							_map[unit.Position] = '.';
							unit.Position = step;
							_map[unit.Position] = unit.Kind;
							attackables = unit.Position
								.LookAround()
								.Select(p => targets.TryGetValue(p, out var u) ? u : null)
								.Where(t => t != null)
								.ToArray();
						}
					}

					if (attackables.Any())
					{
						// Pick the weakest enemy in reading order, attack, and remove
						// if enemy is killed
						var hp = attackables.Min(t => t.Hitpoints);
						var weakest = attackables
							.Where(t => t.Hitpoints == hp)
							.OrderBy(u => u.Position.Y)
							.ThenBy(u => u.Position.X)
							.First();
						weakest.Hitpoints -= unit.Attack;
						if (!weakest.IsAlive)
						{
							_map[weakest.Position] = '.';
							if (weakest.Kind == 'E')
							{
								AllElfsSurvived = false;
							}
						}
					}
				}

				_rounds++;
				ConsoleWrite();
			}

			private Point Move(Point pos, IEnumerable<Point> targets)
			{
				// Find the distances from every adjacent point next to pos to all
				// possible targets, in one fell swoop for efficiency.
				var distances = FindTargetDistances(pos, targets);

				// All reachable targets. Don't move iff no targets are reachable.
				var reachables = distances.Values.SelectMany(d => d.Keys).ToHashSet();
				if (!reachables.Any())
					return null;

				// Find the nearest targets; they are the ones that has the shortest distance,
				// so first find the shortest distance and then find all reachables that is
				// within that distance.
				// (There may be routes with longer distances that were found from other
				// adjacent starting points before we found the shortest route)
				var minDistances = reachables
					.ToDictionary(p => p, p => distances.Values.Min(d => d.TryGetValue(p, out var dist) ? dist : int.MaxValue));
				var minDistance = minDistances.Values.Min();
				var nearest = reachables
					.Where(p => minDistances[p] == minDistance);

				// Choose to pursue the nearest target, in reading order
				var chosen = nearest
					.OrderBy(p => p.Y)
					.ThenBy(p => p.X)
					.First();

				// Look at all routes that lead to this particular chosen target and with
				// the specific shortest distance, and pick the step in reading order
				var step = distances
					.Where(d => (d.Value.TryGetValue(chosen, out var dist) ? dist : int.MaxValue) == minDistance)
					.Select(d => d.Key)
					.OrderBy(p => p.Y)
					.ThenBy(p => p.X)
					.First();

				return step;
			}

			private Dictionary<Point, Dictionary<Point, int>> FindTargetDistances(Point pos, IEnumerable<Point> targets)
			{
				// Look for all open squares in range of each target
				var inrange = targets
					.SelectMany(t => t.LookAround())
					.Where(p => _map[p] == '.')
					.Distinct()
					.ToHashSet();

				// We need up to 4 set of distances. Optimize the calculations where
				// possible: cache the adjacent points next to every point, turn the
				// inranges-points into a fast bool[,] lookup, and keep track of the
				// shortest distance seen so far to only search just to the nearest
				// target and no further.
				var adjacentLookup = new List<(int,int)>[_width,_height];
				var inrangeLookup = new bool[_width,_height];
				foreach (var p in inrange)
				{
					inrangeLookup[p.X, p.Y] = true;
				}
				int minDistanceSeen = int.MaxValue;

				// Find distances to all targets from all empty sides of the position
				var distances = pos
					.LookAround()
					.Where(p => _map[p] == '.')
					.ToDictionary(p => p, p => CalcDistanceField(p));
				return distances;

				Dictionary<Point, int> CalcDistanceField(Point start)
				{
					// Do a BFS. For efficiency (several times speedup) do all operations
					// just on int x,y instead of Point; do the lookaround for next steps
					// dumb and fast without any linq-overhead; cache the lookarounds.
					var frontier = new Queue<(int, int)>();
					var distance = new int[_width,_height];
					distance[start.X, start.Y] = 1;
					frontier.Enqueue((start.X, start.Y));
					while (frontier.Any())
					{
						var (x, y) = frontier.Dequeue();
						var dist = distance[x, y];
						if (dist >= minDistanceSeen)
							break;
						var adjacent = adjacentLookup[x, y];
						if (adjacent == null)
						{
							adjacent = adjacentLookup[x, y] = new List<(int, int)>();
							if (_map[x-1][y] == '.') adjacent.Add((x-1, y));
							if (_map[x+1][y] == '.') adjacent.Add((x+1, y));
							if (_map[x][y-1] == '.') adjacent.Add((x, y-1));
							if (_map[x][y+1] == '.') adjacent.Add((x, y+1));
						}
						foreach (var (ax, ay) in adjacent)
						{
							if (distance[ax, ay] > 0)
								continue;
							distance[ax, ay] = dist + 1;
							if (inrangeLookup[ax, ay])
							{
								minDistanceSeen = dist + 1;
								break;
							}
							frontier.Enqueue((ax, ay));
						}
					}

					// Each set of distances is a set of (point, distance)
					var targetDistances = inrange
						.Where(p => distance[p.X, p.Y] > 0)
						.ToDictionary(p => p, p => distance[p.X, p.Y]);
					return targetDistances;
				}
			}

			private void ConsoleWrite()
			{
#if false				
				Console.Clear();
				Console.WriteLine($"After {_completedRounds} rounds:");
				var visualmap = _map.Render();
				for (var y = 0; y < _height; y++)
				{
					Console.Write($"{visualmap[y]}  ");
					for (var x = 0; x < _width; x++)
					{
						var u = _units.Where(u => u.IsAlive && u.Position == Point.From(x, y)).FirstOrDefault();
						if (u != null)
						{
							Console.Write($" {u.Kind}({u.Hitpoints})");
						}
					}
					Console.WriteLine();
				}
				Console.ReadKey();
#endif
			}
		}
	}
}
