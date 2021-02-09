using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2016.Day11
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Radioisotope Thermoelectric Generators";
		public override int Year => 2016;
		public override int Day => 11;

		public void Run()
		{
			RunPart1For("test1", 11);
			RunFor("test2", 25, 49); // https://www.reddit.com/r/adventofcode/comments/5hoia9/2016_day_11_solutions/db4omkn/
			RunFor("test3", 33, 57); // https://www.reddit.com/r/adventofcode/comments/5hoia9/2016_day_11_solutions/db5ctc6
			RunFor("input", 37, 61);
		}

		protected override int Part1(string[] input)
		{
			return Solve(input);
		}

		protected override int Part2(string[] input)
		{
			var extra = new []
			{
				"An elerium generator.",
				"An elerium-compatible microchip.",
				"A dilithium generator.",
				"A dilithium-compatible microchip."
			};
			input[0] += string.Concat(extra);
			return Solve(input);
		}

		private int Solve(string[] input)
		{
			var floor0 = new Floor(input);
			//floor0.WriteToConsole();

			var seen = new HashSet<uint>();
			var queue = new Queue<Floor>();

			queue.Enqueue(floor0);
			while (queue.Any())
			{
				var floor = queue.Dequeue();
				if (seen.Contains(floor.Id))
				{
					continue;
				}
				seen.Add(floor.Id);
				foreach (var f in floor.NextMoves().Where(x => !seen.Contains(x.Id)))
				{
					if (f.AtTopLevel)
					{
						return f.Steps;
					}
					queue.Enqueue(f);
				}
			}

			throw new Exception("Unsolveable");
		}
	}

	internal struct MicrochipGeneratorPair
	{
		public int MicrochipLevel { get; set; }
		public int GeneratorLevel { get; set; }
	}

	static class Extensions
	{
		public static MicrochipGeneratorPair[] WithMicrochipLevel(this MicrochipGeneratorPair[] objects, int index, int level)
		{
			objects[index].MicrochipLevel = level;
			return objects;
		}

		public static MicrochipGeneratorPair[] WithGeneratorLevel(this MicrochipGeneratorPair[] objects, int index, int level)
		{
			objects[index].GeneratorLevel = level;
			return objects;
		}
	}

	internal class Floor
	{
		private const int TopLevel = 3;
		private MicrochipGeneratorPair[] _objects;
		private int _elevator;

		public int Steps { get; private set; }
		public uint Id { get; private set;}

		public Floor(string[] input)
		{
			// The first floor contains a strontium generator, a strontium-compatible microchip, a plutonium generator, and a plutonium-compatible microchip.
			// The second floor contains a thulium generator, a ruthenium generator, a ruthenium-compatible microchip, a curium generator, and a curium-compatible microchip.
			// The third floor contains a thulium-compatible microchip.
			// The fourth floor contains nothing relevant.
			var rxGenerator = new Regex(@"\w+(?= generator)");
			var names = rxGenerator
				.Matches(string.Join("", input))
				.Select(m => m.Value)
				.ToArray();

			var floors = input.ToList();

			_elevator = 0;
			_objects = names
				.Select(name => new MicrochipGeneratorPair
				{
					GeneratorLevel = floors.FindIndex(s => s.Contains($"{name} generator")),
					MicrochipLevel = floors.FindIndex(s => s.Contains($"{name}-compatible microchip"))
				})
				.ToArray();

			Steps = 0;
			FixateId();
		}

		private Floor(MicrochipGeneratorPair[] objects, int elevator, int steps)
		{
			_elevator = elevator;
			_objects = objects;
			Steps = steps;
			FixateId();
		}

		private void FixateId()
		{
			Id = 0U;
			foreach (var o in _objects.OrderBy(x => x.MicrochipLevel).ThenBy(x => x.GeneratorLevel))
			{
				Id = Id << 2 | (uint)o.MicrochipLevel;
				Id = Id << 2 | (uint)o.GeneratorLevel;
			}
			Id = Id << 2 | (uint)_elevator;
		}

		public bool AtTopLevel => _objects.All(o => o.MicrochipLevel == TopLevel && o.GeneratorLevel == TopLevel);

		public IEnumerable<Floor> NextMoves()
		{
			if (_elevator < TopLevel)
			{
				// return moves going up
				foreach (var floor in NextMovesTo(_elevator + 1))
				{
					yield return floor;
				}
			}
			if (_elevator > 0 && HasObjectsOnFloorOrBelow(_elevator - 1))
			{
				// return moves going down
				foreach (var floor in NextMovesTo(_elevator - 1))
				{
					yield return floor;
				}
			}

			bool HasObjectsOnFloorOrBelow(int floor) => _objects.Any(x => x.MicrochipLevel <= floor || x.GeneratorLevel <= floor);

			IEnumerable<Floor> NextMovesTo(int dest)
			{
				foreach (var objects in NextMoveCandidates())
				{
					if (IsValidMove(objects))
					{
						yield return new Floor(objects, dest, Steps + 1);
					}
				}

				static bool IsValidMove(MicrochipGeneratorPair[] objects)
				{
					foreach (var o in objects.Where(o => o.GeneratorLevel != o.MicrochipLevel))
					{
						if (objects.Any(x => x.GeneratorLevel == o.MicrochipLevel))
						{
							return false;
						}
					}
					return true;
				}

				IEnumerable<MicrochipGeneratorPair[]> NextMoveCandidates()
				{
					// Move 1 or 2 microchips
					for (var i = 0; i < _objects.Length; i++)
					{
						if (_objects[i].MicrochipLevel == _elevator)
						{
							yield return Objects()
								.WithMicrochipLevel(i, dest);
							for (int j = i + 1; j < _objects.Length; j++)
							{
								if (_objects[j].MicrochipLevel == _elevator)
								{
									yield return Objects()
										.WithMicrochipLevel(i, dest)
										.WithMicrochipLevel(j, dest);
								}
							}
						}
					}

					// Move 1 or 2 generators
					for (var i = 0; i < _objects.Length; i++)
					{
						if (_objects[i].GeneratorLevel == _elevator)
						{
							yield return Objects()
								.WithGeneratorLevel(i, dest);
							for (int j = i + 1; j < _objects.Length; j++)
							{
								if (_objects[j].GeneratorLevel == _elevator)
								{
									yield return Objects()
										.WithGeneratorLevel(i, dest)
										.WithGeneratorLevel(j, dest);
								}
							}
						}
					}

					// Move 1 microchip and 1 generator
					for (var i = 0; i < _objects.Length; i++)
					{
						if (_objects[i].MicrochipLevel == _elevator && _objects[i].GeneratorLevel == _elevator)
						{
							yield return Objects()
								.WithMicrochipLevel(i, dest)
								.WithGeneratorLevel(i, dest);
						}
					}

					MicrochipGeneratorPair[] Objects()
					{
						var objects = new MicrochipGeneratorPair[_objects.Length];
						Array.Copy(_objects, objects, _objects.Length);
						return objects;
					}
				}
			}
		}

		public void WriteToConsole()
		{
			Console.WriteLine($"Step {Steps}:");
			for (var floor = TopLevel; floor >= 0; floor--)
			{
				Console.Write($"F{floor + 1} ");
				Console.Write(floor == _elevator ? "E " : ". ");
				for (var i = 0; i < _objects.Length; i++)
				{
					Console.Write(_objects[i].GeneratorLevel == floor ? $"{i}G " : ".  ");
				}
				for (var i = 0; i < _objects.Length; i++)
				{
					Console.Write(_objects[i].MicrochipLevel == floor ? $"{i}M " : ".  ");
				}
				Console.WriteLine();
			}
		}
	}
}
