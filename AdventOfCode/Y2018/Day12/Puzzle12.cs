using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day12
{
	internal class Puzzle : Puzzle<int, ulong>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2018;
		public override int Day => 12;

		public void Run()
		{
		//	Run("test1").Part1(325).Part2(0);
			//Run("test2").Part1(0).Part2(0); 3679 too low,  3797 too high
			Run("input").Part1(3738).Part2(3900000002467);
		}

		protected override int Part1(string[] input)
		{
			var state = input[0].RxMatch("initial state: %*").Get<string>();
			var rules = input[2..]
				.Select(line => line.Split(" => ").ToArray())
				.ToDictionary(x => x[0], x => x[1][0]);

//			var plants = 0;
			var offset = 0;//-state.IndexOf('#');
			for (var g = 0; g < 20; g++)
			{
		//		Console.WriteLine(state);

				state = "...." + state;
				offset += 4;
				// if (state[0] == '#')
				// {
				// 	state = ".." + state;
				// 	offset += 2;
				// }
				// else if (state[1] == '#')
				// {
				// 	state = "." + state;
				// 	offset++;
				// }

				// if (state[state.Length-1] == '#')
				// 	state = state + "..";
				// else if (state[state.Length-2] == '#')
				// 	state = ".." + state;
				state += "....";
				// Console.WriteLine(state);
				// Console.WriteLine();

				var newstate = state.ToCharArray();
				for (var i = 0; i < state.Length - 4; i++)
				{
					var pat = state[i..(i+5)];
					if (rules.TryGetValue(pat, out var ch))
					{
						newstate[i+2] = ch;
					}
					else
					{
						newstate[i+2] = '.';
					}
				}

				state = new string(newstate);
			}

			var plants = 0;
			for (var i = 0; i < state.Length; i++)
			{
				if (state[i] == '#')
				{
					var id = i - offset;
					plants += id;
				}
			}

			return plants;
		}

		protected override ulong Part2(string[] input)
		{
			// 3244 too low
			// 500011047 too low
			// 2340000006367 too low
			// 1416000002455 not right
			// 2400000000487 not right
			// 3900000002389 not right
			// 3900000002467

			var state = input[0].RxMatch("initial state: %*").Get<string>();
			var orgstate = state;
			var rules = input[2..]
				.Select(line => line.Split(" => ").ToArray())
				.ToDictionary(x => x[0], x => x[1][0]);

			var seen = new HashSet<string>();
//			var plants = 0;
			var offset = 0;//-state.IndexOf('#');

			var N = 50000000000 % 99;

			state = $"..........{state}...............................................................................................................................";
			var dotlen0 = state.TakeWhile(c => c == '.').Count();

			for (var g = 0; g <444; g++)
			{
			//	Console.WriteLine(state);

				var bare = state.Trim('.');
				if (seen.Contains(bare))
				{
					var dotlen = state.TakeWhile(c => c == '.').Count();
					Console.WriteLine($"Bingo at {g}");
					break;
				}
				seen.Add(bare);

				//state += "....";

		//		Console.WriteLine(state);

				// state = "...." + state;
				// offset += 4;
				// if (state[0] == '#')
				// {
				// 	state = ".." + state;
				// 	offset += 2;
				// }
				// else if (state[1] == '#')
				// {
				// 	state = "." + state;
				// 	offset++;
				// }

				// if (state[state.Length-1] == '#')
				// 	state = state + "..";
				// else if (state[state.Length-2] == '#')
				// 	state = ".." + state;
//				state += "....";
				// Console.WriteLine(state);
				// Console.WriteLine();

				var newstate = state.ToCharArray();
				for (var i = 0; i < state.Length - 4; i++)
				{
					var pat = state[i..(i+5)];
					if (rules.TryGetValue(pat, out var ch))
					{
						newstate[i+2] = ch;
					}
					else
					{
						newstate[i+2] = '.';
					}
				}

				state = new string(newstate);


			}

			// offset = 59;
			//var add = 59 + 50_000_000_000UL - 100;
			var add = 60 + 50_000_000_000UL - 100;
			//var add = (ulong)offset*loops;


			var plants = 0UL;
			var bare2 = state.Trim('.');
			for (var i = 0; i < bare2.Length; i++)
			{
				if (bare2[i] == '#')
				{
					var id = (ulong)i + add;
					plants += id;
				}
			}

			return plants;

		}
	}
}
