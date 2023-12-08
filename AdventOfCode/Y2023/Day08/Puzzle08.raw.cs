using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using MathNet.Numerics.Optimization.LineSearch;

namespace AdventOfCode.Y2023.Day08.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(2);
			Run("test2").Part2(6);
			
			Run("input").Part1(19241).Part2(9606140307013);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var directions = input[0];
			var network = input
				.Skip(2)
				.Select(s => {
					var (name, left, right) = s.RxMatch("%s = (%s, %s)").Get<string, string, string>();
					return (name, left, right);
				})
				.ToDictionary(x => x.name, x => (x.left, x.right));
			
			var step = 0;
			var node = "AAA";
			while (node != "ZZZ")
			{
				var dir = directions[step++ % directions.Length];
				node = dir == 'L' ? network[node].left : network[node].right;
			}

			return step;
		}

		protected override long Part2(string[] input)
		{
			var directions = input[0];
			var network = input
				.Skip(2)
				.Select(s => {
					var (name, left, right) = s.RxMatch("%s = (%s, %s)").Get<string, string, string>();
					return (name, left, right);
				})
				.ToDictionary(x => x.name, x => (x.left, x.right));
			
			var step = 0;
			var nodes = network.Keys.Where(x => x.EndsWith('A')).ToArray();
			var stepinfo = nodes.Select(n =>
			{
				var step = 0;
				while (n.Last() != 'Z')
				{
					var dir = directions[step++ % directions.Length];
					n = dir == 'L' ? network[n].left : network[n].right;
					//Console.Write($"{n} ");
				}
				var init = step;
				//Console.Write("# ");
				do {
					var dir = directions[step++ % directions.Length];
					n = dir == 'L' ? network[n].left : network[n].right;
					//Console.Write($"{n} ");
				} while (n.Last() != 'Z');
				//Console.WriteLine();
				var cycle = step - init;
				return (init, cycle);
			})
			.ToArray();

			var x = MathHelper.LeastCommonMultiple(stepinfo.Select(x => (long)x.cycle).ToArray());

			return x;
		}
	}
}
