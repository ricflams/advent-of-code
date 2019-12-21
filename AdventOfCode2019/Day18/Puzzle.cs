using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day18
{
	internal static class Puzzle
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			//var steps1 = ShortestPath(ReadMap("Day18/input-1.txt"));
			//Debug.Assert(steps1 == 8);

			//Console.WriteLine("########################");
			//var steps2 = ShortestPath(ReadMap("Day18/input-2.txt"));
			//Debug.Assert(steps2 == 86);

			//Console.WriteLine("########################");
			//var steps3 = ShortestPath(ReadMap("Day18/input-3.txt"));
			//Debug.Assert(steps3 == 132);

			//Console.WriteLine("########################");
			//var steps4 = ShortestPath(ReadMap("Day18/input-4.txt"));
			//Debug.Assert(steps4 == 136);

			////Console.WriteLine("########################");
			////var steps5 = ShortestPath(ReadMap("Day18/input-5.txt"));
			////Debug.Assert(steps5 == 81);

			var steps = ShortestPath(ReadMap("Day18/input.txt"));
			Console.WriteLine("########################");
			Console.WriteLine($"Day 18 Puzzle 1: {steps}");
			//Debug.Assert(intersections == 5056);
		}

		private static void Puzzle2()
		{

			//Console.WriteLine($"Day 17 Puzzle 2: {dust}");
			//Debug.Assert(dust == 942367);
		}

		class BestPositionFinding
		{
			public Dictionary<string, int> Findings { get; set; }
		}

		class Pending
		{
			public int Step;
			public Point Prev;
			public Point Pos;
			public Keys Keys;
		}

		private static int ShortestPath(CharMap map)
		{
			var debug = false;
			var TooBigStep = int.MaxValue;

			var numberOfKeys = map.AllPoints(ch => char.IsLower(ch)).Count();
			var pos0 = map.AllPoints(ch => ch == '@').First();
			map[pos0] = '.'; // not that nice
			var keys0 = new Keys();

			foreach (var line in map.Render((p, ch) => pos0.Is(p) ? '@' : ch))
			{
				Console.WriteLine(line);
			}

			var ongoing = new Dictionary<string, BestPositionFinding>();
			foreach (var p in map.AllPoints(ch => ch != '#'))
			{
				if (!ongoing.TryGetValue(p.ToString(), out var posfindings))
				{
					posfindings = ongoing[p.ToString()] = new BestPositionFinding { Findings = new Dictionary<string, int>() };
				}
			}

			var shortest = int.MaxValue;
			var totalsteps = 0;

			void DebugClear() { if (debug) Console.Clear(); }
			void DebugWriteLine(string s) { if (debug) Console.WriteLine(s); }
			void DebugReadKey() { if (debug) Console.ReadKey(); }


			foreach (var p in pos0.LookAround().Where(p => IsVacant(map[p], keys0)))
			{
				ShortestPath(pos0.ToString(), 1, pos0, p, keys0);
			}
			Console.WriteLine($"Total steps: {totalsteps}");

			return shortest;
			
			void ShortestPath(string fullpath, int step, Point prev, Point pos, Keys startkeys)
			{
				fullpath = $"[{fullpath.Length}]";
				DebugWriteLine("");
				DebugWriteLine($"[{step}/{shortest}] {fullpath} Start exploring");

				var keys = startkeys.Copy();

				var pending = new List<Pending>();


				var path = new Dictionary<string, int>();
				while (step < shortest - 1)
				{
					DebugClear();
					DebugWriteLine($"Position {pos}");

					if (totalsteps % 1000 == 0)
					{
						Console.Write($".");
					}

					if (keys.FoundNewKey(map[pos]))
					{
						DebugWriteLine($"Found new key {map[pos]} - keys: {keys.AllKeys}");
						prev = null;
					}
					if (debug)
					{
						foreach (var line in map.Render((p, ch) => pos.Is(p) ? '@' : ch))
						{
							DebugWriteLine($"[{step}/{shortest}] {fullpath} {line}");
						}
						DebugReadKey();
					}

					if (!ongoing.TryGetValue(pos.ToString(), out var posfindings))
					{
						posfindings = ongoing[pos.ToString()] = new BestPositionFinding { Findings = new Dictionary<string, int>() };
					}
					var better = posfindings.Findings.FirstOrDefault(k => (keys.AreEqual(k.Key) || keys.IsSubsetOf(k.Key)) && k.Value < step);
					if (better.Key != null)
					{
						// Others have done better than us at finding these keys; abort, abort
						DebugWriteLine($"[{step}/{shortest}] {fullpath} Previously found in fewer steps: {better.Value}");
						//Console.Write("-");
						break;
					}

					// This is the best effort registered on this position
					if (!posfindings.Findings.ContainsKey(keys.AllKeys))
					{
						posfindings.Findings[keys.AllKeys] = int.MaxValue;
					}
					posfindings.Findings[keys.AllKeys] = Math.Min(posfindings.Findings[keys.AllKeys], step);

					if (keys.Count == numberOfKeys)
					{
						DebugWriteLine($"[{step}/{shortest}] {fullpath} Found all keys in {step}");
						shortest = step;
						Console.Write($"({shortest})");
						break;
					}






					var directions = pos
						.LookAround()
						.Where(p => !p.Is(prev) && IsVacant(map[p], keys))
						.ToList();
					if (directions.Count() == 0)
					{
						if (!Keys.IsKey(map[pos]))
						{
							DebugWriteLine($"[{step}/{shortest}] {fullpath} Dead-end without a key; abort");
							break;
						}
						var temp = pos;
						pos = prev;
						prev = temp;
						step++;
						totalsteps++;
					}
					else if (directions.Count() == 1)
					{
						prev = pos;
						pos = directions.First();
						step++;
						totalsteps++;
					}
					else
					{
						DebugWriteLine($"[{step}/{shortest}] {fullpath} About to explore {string.Join(" ", directions)}");
						foreach (var p in directions.Skip(1))
						{
							pending.Add(new Pending
							{
								Step = step + 1,
								Prev = pos,
								Pos = p,
								Keys = keys.Copy()
							});
						}


						// Follow the first route here in this function
						prev = pos;
						pos = directions.First();
						step++;
						totalsteps++;
					}

					if (path.TryGetValue(pos.ToString(), out var keycount) && keys.Count == keycount)
					{
						// Going in circles, not adding any keys - dead end
						DebugWriteLine($"[{step}/{shortest}] {fullpath} Going in circles after {step}");
						step = TooBigStep;
					}
					path[pos.ToString()] = keys.Count;

				}

				// This is the best effort registered on this position
				var posfindings2 = ongoing[pos.ToString()];
				if (!posfindings2.Findings.ContainsKey(keys.AllKeys))
				{
					posfindings2.Findings[keys.AllKeys] = int.MaxValue;
				}
				posfindings2.Findings[keys.AllKeys] = Math.Min(posfindings2.Findings[keys.AllKeys], step);

				foreach (var p in pending)
				{
					ShortestPath($"{fullpath} {p.Pos}", p.Step, p.Prev, p.Pos, p.Keys);
				}

			}

			bool IsVacant(char ch, Keys keys) => ch == '.' || Keys.IsKey(ch) || (keys?.IsDoorOpen(ch) ?? false);
		}

		private class Keys
		{
			public string AllKeys { get; private set; }
			public Keys(string keys = "")
			{
				AllKeys = keys.ToString();
			}
			public bool FoundNewKey(char key)
			{
				if (IsKey(key) && !AllKeys.Contains(key))
				{
					// Keep keys sorted for faster lookups and comparisons
					AllKeys = new string((AllKeys + key).ToCharArray().OrderBy(c => c).ToArray());
					return true;
				}
				return false;
			}
			public static bool IsKey(char key) => char.IsLower(key);
			public int Count => AllKeys.Length;
			public bool IsDoorOpen(char door) => AllKeys.Contains(char.ToLower(door));
			public Keys Copy() => new Keys(AllKeys);
			public bool AreEqual(Keys other) => AreEqual(other.AllKeys);
			public bool AreEqual(string rawkeys) => AllKeys == rawkeys;
			public bool IsSubsetOf(Keys other) => IsSubsetOf(other.AllKeys);
			public bool IsSubsetOf(string rawkeys) => rawkeys.Length > Count && AllKeys.ToCharArray().Except(rawkeys.ToCharArray()).Count() == 0;
		}

		private static CharMap ReadMap(string filename)
		{
			var lines = File.ReadAllLines(filename);
			var map = new CharMap();
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					map[x][y] = line[x];
				}
			}
			return map;
		}
	}
}

