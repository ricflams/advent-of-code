using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day07.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Bridge Repair";
		public override int Year => 2024;
		public override int Day => 7;

		public override void Run()
		{
			Run("test1").Part1(3749).Part2(11387);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(12940396350192).Part2(106016735664498);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var tests = input.Select(x => x.Split(':')).Select(x => new Test
			{
				Sum = long.Parse(x[0]),
				Values = x[1].SplitSpace().Select(long.Parse).ToArray()
			}).ToArray();

			var sum = tests.Where(t => t.IsValid()).Sum(t => t.Sum);

			return sum;
		}

		internal class Test
		{
			public long Sum { get; set; }
			public long[] Values { get; set; }

			public bool IsValid()
			{
				var maxops = 1 << (Values.Length - 1);
				for (var ops = 0u; ops < maxops; ops++)
				{
					var v = Values[0]; 
					for (var i = 1; i < Values.Length; i++)
					{
						if ((ops & (1u<<(i-1))) == 0)
							v += Values[i];
						else
							v *= Values[i];
					}
					if (v == Sum)
						return true;
				}
				return false;
			}

			public bool IsValidWithConcat()
			{
				//Console.WriteLine("Testing sum " + Sum);
				foreach (var ops in MathHelper.CountInBaseX(3, Values.Length - 1))
					;
				foreach (var ops in MathHelper.CountInBaseX(3, Values.Length - 1))
				{
					//Console.WriteLine(string.Join(' ', ops.Select(t => t.ToString()).ToArray()));

					var v = Values[0]; 
					for (var i = 1; i < Values.Length; i++)
					{
						var op = ops[i-1];
						if (op == 0)
							v += Values[i];
						else if (op == 1)
							v *= Values[i];
						else
						{
							v = v.Concat(Values[i]);
						}
						if (v > Sum)
							break;
					}
					if (v == Sum)
						return true;

				}
				// var maxops = 1 << (Values.Length - 1);
				// for (var ops = 0u; ops < maxops; ops++)
				// {
				// 	var v = Values[0]; 
				// 	for (var i = 1; i < Values.Length; i++)
				// 	{
				// 		if ((ops & (1u<<(i-1))) == 0)
				// 			v += Values[i];
				// 		else
				// 			v *= Values[i];
				// 	}
				// 	if (v == Sum)
				// 		return true;
				// }
				return false;
			}
		}

		protected override long Part2(string[] input)
		{
			var tests = input.Select(x => x.Split(':')).Select(x => new Test
			{
				Sum = long.Parse(x[0]),
				Values = x[1].SplitSpace().Select(long.Parse).ToArray()
			}).ToArray();

			var sum = 0L;
			foreach (var t in tests)
			{
				if (t.IsValid() || t.IsValidWithConcat())
				{
					sum += t.Sum;
				}


				
				// var maxops = 1 << (t.Values.Length - 1);
				// for (var ops = 0u; ops < maxops; ops++)
				// {
				// 	var vals = t.Values.Select(x => x.ToString()).ToList();
				// 	for (var i = vals.Count() - 1; i-- > 0; )
				// 	{
				// 		if ((ops & (1u<<(i-1))) == 0)
				// 		{
				// 			vals[i] += vals[i+1];
				// 			vals.RemoveAt(i+1);
				// 		}
				// 	}
				// 	var t2 = new Test { Sum = t.Sum, Values = vals.Select(long.Parse).ToArray() };
				// 	if (t2.IsValid())
				// 	{
				// 		sum += t2.Sum;
				// 		break;
				// 	}
				// }
			}

			return sum;
		}
	}
}
