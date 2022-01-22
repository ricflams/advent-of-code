using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day18
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Snailfish";
		public override int Year => 2021;
		public override int Day => 18;

		public void Run()
		{
			Run("test1").Part1(4140).Part2(3993);
			Run("input").Part1(3869).Part2(4671);
		}

		protected override long Part1(string[] input)
		{
			var fish = input
				.Select(Snailfish.Parse)
				.ToArray();

			// Sum up all the snailfish numbers and take the final magnitude
			var sum = fish.First();
			foreach (var f in fish.Skip(1))
			{
				sum = new Snailfish(sum, f).Reduce();
			}

			return sum.Magnitude;
		}

		protected override long Part2(string[] input)
		{
			var fishes = input
				.Select(Snailfish.Parse)
				.ToArray();

			// Check the sum of every pair of numbers and grab the max magnitude
			var largestMagnitude = 0;
			foreach (var f1 in fishes)
			{
				foreach (var f2 in fishes)
				{
					if (f1 == f2)
						continue;
					var magnitude = new Snailfish(f1, f2).Reduce().Magnitude;
					if (magnitude > largestMagnitude)
					{
						largestMagnitude = magnitude;
					}
				}
			}

			return largestMagnitude;
		}

		internal class Snailfish
		{
			public Snailfish Parent;
			public SnailfishValue Left;
			public SnailfishValue Right;

			public class SnailfishValue
			{
				public Snailfish Fish;
				public int Value;
				public bool IsFish => Fish != null;
				public bool IsValue => Fish == null;

				public SnailfishValue Copy(Snailfish parent) =>
					IsFish
					? new SnailfishValue { Fish = Fish.Copy(parent) }
					: new SnailfishValue { Value = Value };
				public int Magnitude => IsFish ? Fish.Magnitude : Value;
				public override string ToString() => IsFish ? Fish.ToString() : Value.ToString();
			}

			private Snailfish()
			{
			}

			public Snailfish(Snailfish left, Snailfish right)
			{
				Parent = null;
				Left = new SnailfishValue { Fish = left.Copy() };
				Right = new SnailfishValue { Fish = right.Copy() };
				Left.Fish.Parent = Right.Fish.Parent = this;
			}

			public Snailfish(Snailfish parent, int leftValue, int rightValue)
			{
				Parent = parent;
				Left = new SnailfishValue { Value = leftValue };
				Right = new SnailfishValue { Value = rightValue };
			}

			public int Magnitude => 3 * Left.Magnitude + 2 * Right.Magnitude;

			public Snailfish Reduce()
			{
				// Reduce for as long as there are reductions to make.
				// Only split if no explosions are possible.
				while (ReduceExplode() || ReduceSplit())
					;
				return this;
			}

			private bool ReduceExplode()
			{
				if (Left.Fish?.ReduceExplode() ?? false)
					return true;

				// Any pair nested inside four pairs shall explode
				if (Parent?.Parent?.Parent?.Parent != null)
				{
					// Find the left and right neighbors, if any, and add the exploding
					// values to them. Then remove this exploded fish from its parent.
					var (left, right) = FindNeighbors();
					if (left != null)
					{
						var side = left.Right.IsValue ? left.Right : left.Left;
						side.Value += Left.Value;
					}
					if (right != null)
					{
						var side = right.Left.IsValue ? right.Left : right.Right;
						side.Value += Right.Value;
					}
					var explodee = this == Parent.Left.Fish ? Parent.Left : Parent.Right;
					explodee.Fish = null;
					explodee.Value = 0;
					return true;
				}

				if (Right.Fish?.ReduceExplode() ?? false)
					return true;

				return false;

				(Snailfish, Snailfish) FindNeighbors()
				{
					Snailfish left = null;
					Snailfish right = null;
					Snailfish prev = null;
					bool foundSelf = false;
					var fishWithValue = FindRoot().GetAll().Where(f => f.Left.IsValue || f.Right.IsValue);
					foreach (var f in fishWithValue)
					{
						if (f == this)
						{
							left = prev;
							foundSelf = true;
						}
						else if (foundSelf)
						{
							right = f;
							break;
						}
						prev = f;
					}
					return (left, right);
				}
			}

			private bool ReduceSplit()
			{
				if (Left.Fish?.ReduceSplit() ?? false)
					return true;

				if (Split(ref Left) || Split(ref Right))
					return true;

				if (Right.Fish?.ReduceSplit() ?? false)
					return true;

				return false;

				bool Split(ref SnailfishValue val)
				{
					// Any regular number >= 10 shall split into a new Snailfish
					if (val.IsValue && val.Value >= 10)
					{
						var leftval = val.Value / 2;
						var rightval = val.Value - leftval;
						val.Fish = new Snailfish(this, leftval, rightval);
						val.Value = 0;
						return true;
					}
					return false;
				}
			}

			public override string ToString() => $"[{Left},{Right}]";

			private Snailfish Copy(Snailfish parent = null)
			{
				var copy = new Snailfish
				{
					Parent = parent
				};
				copy.Left = Left.Copy(copy);
				copy.Right = Right.Copy(copy);
				return copy;
			}

			private Snailfish FindRoot()
			{
				var f = this;
				while (f.Parent != null)
					f = f.Parent;
				return f;
			}

			private IEnumerable<Snailfish> GetAll()
			{
				if (Left.IsFish)
				{
					foreach (var f in Left.Fish.GetAll())
						yield return f;
				}
				yield return this;
				if (Right.IsFish)
				{
					foreach (var f in Right.Fish.GetAll())
						yield return f;
				}
			}

			public static Snailfish Parse(string s)
			{
				var index = 1;
				return Parse(s.ToCharArray(), null);

				Snailfish Parse(char[] s, Snailfish parent)
				{
					var fish = new Snailfish(parent, 0, 0);

					if (s[index] == '[')
					{
						index++;
						fish.Left.Fish = Parse(s, fish);
					}
					else
					{
						fish.Left.Value = s[index++] - '0';
					}
					if (s[index++] != ',')
						throw new Exception();
					if (s[index] == '[')
					{
						index++;
						fish.Right.Fish = Parse(s, fish);
					}
					else
					{
						fish.Right.Value = s[index++] - '0';
					}
					if (s[index++] != ']')
						throw new Exception();

					return fish;
				}
			}
		}

	}
}
