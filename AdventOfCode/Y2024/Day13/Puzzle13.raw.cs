using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using Microsoft.VisualBasic;
using System.Runtime.Intrinsics;

namespace AdventOfCode.Y2024.Day13.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(480).Part2(0);
//			Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		class Machine
		{
			public Point A;
			public Point B;
			public Point Prize;

			public bool CanSolve(out (long, long) Result)
			{
				var a1 = A.X;
				var b1 = B.X;
				var c1 = -(Prize.X + 10000000000000L);
				var a2 = A.Y;
				var b2 = B.Y;
				var c2 = -(Prize.Y + 10000000000000L);
				var det = a1*b2 - a2*b1;
				if (det != 0)
				{
					var v1 = (b1*c2 - b2*c1);
					var v2 = (c1*a2 - c2*a1);
					if (v1 % det == 0 && v2 % det == 0)
					{
						var a = v1/det;
						var b = v2/det;

						var x = a*A.X + b*B.X;
						var y = a*A.Y + b*B.Y;
						// Debug.Assert(x == Prize.X);
						// Debug.Assert(y == Prize.Y);

						Result = (a, b);
						return true;
					}
				}
				Result = (0,0);
				return false;
			}
		}

		protected override long Part1(string[] input)
		{
			var machines = input.GroupByEmptyLine()
				.Select(x => new Machine
				{ 
					A = Point.From(x[0].RxMatch("Button A: X+%d, Y+%d").Get<int, int>()),
					B = Point.From(x[1].RxMatch("Button B: X+%d, Y+%d").Get<int, int>()),
					Prize = Point.From(x[2].RxMatch("Prize: X=%d, Y=%d").Get<int, int>())
				})
				.ToArray();

			var sum = 0L;
			foreach (var m in machines)
			{
				if (m.CanSolve(out var pushes))
				{
					sum += pushes.Item1*3 + pushes.Item2;
				}
			}

			return sum;


			// if (a1 == a2)
			// 	continue; // parallel lines

			// Solve y=a1x + b1, y=a2x + b2
			// var x = (b2 - b1) / (a1 - a2);
			// var y = a1*x + b1;
		}

		protected override long Part2(string[] input)
		{
			var machines = input.GroupByEmptyLine()
				.Select(x => new Machine
				{ 
					A = Point.From(x[0].RxMatch("Button A: X+%d, Y+%d").Get<int, int>()),
					B = Point.From(x[1].RxMatch("Button B: X+%d, Y+%d").Get<int, int>()),
					Prize = Point.From(x[2].RxMatch("Prize: X=%d, Y=%d").Get<int, int>())
				})
				.ToArray();

			var sum = 0L;
			foreach (var m in machines)
			{
				if (m.CanSolve(out var pushes))
				{
					sum += pushes.Item1*3 + pushes.Item2;
				}
			}

			return sum;

			return 0;
		}
	}
}
