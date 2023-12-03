using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2021.Day18.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 18";
		public override int Year => 2021;
		public override int Day => 18;

		public override void Run()
		{
			//Run("test1").Part1(0).Part2(0);

			Run("test2").Part1(4140).Part2(3993);
			//Run("test3").Part1(4140).Part2(0);
			//Run("test4").Part1(0).Part2(0);

			Run("input").Part1(3869).Part2(0);
			// 4708 not it


			// 1666865644
			// 279395733
		}

		protected override long Part1(string[] input)
		{
			var fishes = input
				.Select(Snailfish.Parse)
				.ToArray();

			//Console.WriteLine();

			var sum = fishes[0];
			foreach (var f in fishes.Skip(1))
			{
				var fish = new Snailfish {/* Level = 0,*/ Parent = null };
				fish.Left = sum;
				fish.Right = f;
				f.Parent = fish;
				sum.Parent = fish;
				//sum.Increaselevels();
				//f.Increaselevels();
				//Console.WriteLine($"Added:   {fish.Dump()}");
				fish.ReduceIt();
				//Console.WriteLine($"Reduced: {fish.Dump()}");
				//Console.WriteLine();
				sum = fish;
			}

			var magnitude = sum.Magnitude;

			return magnitude;
		}

		protected override long Part2(string[] input)
		{
			var fishes = input
				.Select(Snailfish.Parse)
				.ToArray();

			//Console.WriteLine();

			var largest = 0;

			foreach (var fish1 in fishes)
			{
				foreach (var fish2 in fishes)
				{
					if (fish1 == fish2)
						continue;

					var f1 = Snailfish.Parse(fish1.Dump());
					var f2 = Snailfish.Parse(fish2.Dump());

					var fish = new Snailfish {/* Level = 0,*/ Parent = null };
					fish.Left = f1;
					fish.Right = f2;
					f1.Parent = fish;
					f2.Parent = fish;
					fish.ReduceIt();
					var mag = fish.Magnitude;
					if (mag > largest)
						largest = mag;
				}
			}

			return largest;
		}


		internal class Snailfish
		{
			//public int Level;
			public Snailfish Parent;
			public Snailfish Left;
			public int LeftValue;
			public Snailfish Right;
			public int RightValue;

			//[DebuggerDisplay]
			//public override string ToString() => $"[{GetHashCode()} leftval={LeftValue} rightval={RightValue} left={Left?.GetHashCode()} right={Right?.GetHashCode()}";
			public override string ToString() => $"[{GetHashCode()} {Dump()}";
//			public override string ToString() => Dump();
			public string Dump()
			{
				//if (Level == 0)
				//	return $"{Left.Dump()}";
				var v1 = Left != null ? Left.Dump() : LeftValue.ToString();
				var v2 = Right != null ? Right.Dump() : RightValue.ToString();
				return $"[{v1},{v2}]";
			}

			public int Magnitude =>
				3 * (Left != null ? Left.Magnitude : LeftValue) +
				2 * (Right != null ? Right.Magnitude : RightValue);

			//public Smallfish Reduce()
			//{
			//	var s = Dump();
			//	while (true)
			//	{
			//		var s2 = DoAnySplit(s);
			//		var s3 = DoAnyExplode(s2);
			//		if (s == s3)
			//		{
			//			return Parse(s);
			//		}
			//	}
			//}

			//private string DoAnySplit(string s)
			//{
			//	var match = Regex.Match(s, @"\d{2,}");

			//	if (!match.Success)
			//	{
			//		val = null;
			//		return false;
			//	}

			//	val = match.Groups.Values.Skip(1).Select(g => g.Value).ToArray();
			//}

			//private string DoAnyExplode(string s)
			//{

			//}


			//public void Increaselevels()
			//{
			//	Level++;
			//	Left?.Increaselevels();
			//	Right?.Increaselevels();
			//}



			private static Snailfish FindRoot(Snailfish f)
			{
				while (f.Parent != null)
					f = f.Parent;
				return f;
			}

			private static IEnumerable<Snailfish> GetAll(Snailfish root)
			{
				if (root.Left != null)
				{
					foreach (var f in GetAll(root.Left))
						yield return f;
				}
				yield return root;
				if (root.Right != null)
				{
					foreach (var f in GetAll(root.Right))
						yield return f;
				}
			}

			public void Exploded(Snailfish fish)
			{
				var root = FindRoot(fish);
				var fishes = GetAll(root)
					.Where(f => f.Left == null || f.Right == null)
					.ToArray();


				var leftval = fish.LeftValue;
				var prev = fishes.TakeWhile(f => f != fish).LastOrDefault();
				if (prev != null)
				{
					if (prev.Right == null)
					{
						prev.RightValue += leftval;
					//	prev.Explode(ref prev.Right, ref prev.RightValue);
					}
					else if (prev.Left == null)
					{
						prev.LeftValue += leftval;
					//	prev.Explode(ref prev.Left, ref prev.LeftValue);
					}
					else
						throw new Exception();
				}

				var rightval = fish.RightValue;
				var next = fishes.SkipWhile(f => f != fish).Skip(1).FirstOrDefault();
				if (next != null)
				{
					if (next.Left == null)
					{
						next.LeftValue += rightval;
					//	next.Explode(ref next.Left, ref next.LeftValue);
					}
					else if (next.Right == null)
					{
						next.RightValue += rightval;
					//	next.Explode(ref next.Right, ref next.RightValue);
					}
					else
						throw new Exception();
				}

				if (fish == Left)
				{
					Left = null;
					LeftValue = 0;
				}
				else
				{
					Right = null;
					RightValue = 0;
				}

			}

			public void ReduceIt()
			{
				var changed = false;
				do
				{
					changed = false;
					while (ReduceExplode())
						changed = true;
					if (ReduceSplit())
						changed = true;
				}
				while (changed);
			}

			public bool ReduceExplode()
			{
				if (Left?.ReduceExplode() ?? false)
					return true;

				if (Parent?.Parent?.Parent?.Parent != null && Left == null && Right == null)
				{
					Debug.Assert(Left == null);
					Debug.Assert(Right == null);
					//Console.WriteLine($"before explode: {FindRoot(this).Dump()}");
					Parent.Exploded(this);
					//Console.WriteLine($"after explode: {FindRoot(this).Dump()}");
					return true;
				}

				if (Right?.ReduceExplode() ?? false)
					return true;

				return false;
			}

			public bool ReduceSplit()
			{
				if (Left?.ReduceSplit() ?? false)
					return true;

				if (Split(ref Left, ref LeftValue))
					return true;
				if (Split(ref Right, ref RightValue))
					return true;

				if (Right?.ReduceSplit() ?? false)
					return true;

				return false;
			}

			private bool Split(ref Snailfish child, ref int childval)
			{
				if (child == null && childval >= 10)
				{
					// split left value
					var leftval = childval / 2;
					var rightval = childval - leftval;
					childval = 0;
					child = new Snailfish
					{
						Parent = this,
						//Level = Level + 1,
						LeftValue = leftval,
						RightValue = rightval
					};
					//Console.WriteLine($"after split:   {FindRoot(this).Dump()}");
					return true;
				}
				return false;
			}

			public static Snailfish Parse(string s)
			{
				var index = 1;
				var input = s.ToCharArray();
				var fish = Parse(input, null, 0, ref index);
				//Console.WriteLine($"Parse fish: {fish.Dump()}");
				return fish;
			}

			private static Snailfish Parse(char[] s, Snailfish parent, int level, ref int index)
			{
				var fish = new Snailfish
				{
					Parent = parent,
					//Level = level,
				};

				if (s[index] == '[')
				{
					index++;
					fish.Left = Parse(s, fish, level + 1, ref index);
				}
				else
				{
					fish.Left = null;
					fish.LeftValue = s[index++] - '0';
				}

				if (s[index++] != ',')
					throw new Exception();
				if (s[index] == '[')
				{
					index++;
					fish.Right = Parse(s, fish, level + 1, ref index);
				}
				else
				{
					fish.Right = null;
					fish.RightValue = s[index++] - '0';
				}

				if (s[index++] != ']')
					throw new Exception();

				return fish;
			}

		}




		//private int Parse(char[] s, int nesting, int index, out Explode explode)
		//{
		//	if (s[index++] != '[')
		//		throw new Exception();

		//	Explode ex1 = new Explode(), ex2 = new Explode();
		//	int v1, v2;

		//	if (s[index] == '[')
		//	{
		//		v1 = Parse(s, nesting + 1, index, out ex1);
		//	}
		//	else
		//	{
		//		v1 = s[index++] - '0';
		//	}
		//	if (s[index++] != ',')
		//		throw new Exception();
		//	if (s[index] == '[')
		//	{
		//		v2 = Parse(s, nesting + 1, index, out ex2);
		//	}
		//	else
		//	{
		//		v2 = s[index++] - '0';
		//	}

		//	if (s[index++] != ']')
		//		throw new Exception();

		//	v1 += ex1.Left + ex2.Left;

		//	if (nesting == 4)
		//	{
		//		explode = new Explode { }
		//		}

		//}

	}
}
