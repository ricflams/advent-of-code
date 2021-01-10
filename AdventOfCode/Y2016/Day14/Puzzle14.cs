using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace AdventOfCode.Y2016.Day14
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2016;
		public override int Day => 14;

		public void Run()
		{
			RunFor("test1", 22728, 22551);
			//RunFor("test2", 0, 0);
			RunFor("input", 25427, 22045);
			//RunPart2For("input", 22045);
		}

		protected override int Part1(string[] input)
		{
			var salt = input[0];

			// It contains three of the same character in a row, like 777. Only consider the first such triplet in a hash.
			// One of the next 1000 hashes in the stream contains that same character five times in a row, like 77777.

			//bool HasTripleSequence(byte[] hash, out byte val) => MathHelper.HasAny4BitSequence(hash, 3, out val);
//			bool HasTripleSequence(byte[] hash) => MathHelper.HasAny4BitSequence(hash, 3, out var _);

//			var tripletFinder = new Md5HashFinder(HasTripleSequence);
			var start = 0;
			var index = 0;
			for (var i = 0; i < 64; i++)
			{
				// var triplet = tripletFinder.FindMatches(salt, start).First();
				while (true)
				{
					byte hexval = 0;
					var tripletFinder = new Md5HashFinder((byte[] hash, int _) => MathHelper.HasAnyHexSequence(hash, 3, out hexval));
					var seq3 = tripletFinder.FindMatches(salt, start, int.MaxValue).First();
					index = seq3.Iterations;
					start = seq3.Iterations + 1;

					var seq5Finder = new Md5HashFinder((byte[] hash, int _) => MathHelper.HasHexSequence(hash, 5, hexval));
					var next = seq5Finder.FindMatches(salt, start, start + 1000).Count();

					if (next == 1)
					{
						//Console.WriteLine($"At i={i} index {index} found hex={hexval}");
						break;
					}
				}
			}

			// var match = finder.FindMatches(salt, 0).Skip(63).First();

			return index;
		}

// 		protected override int Part2(string[] input)
// 		{
// 			var salt = input[0];

// 			// It contains three of the same character in a row, like 777. Only consider the first such triplet in a hash.
// 			// One of the next 1000 hashes in the stream contains that same character five times in a row, like 77777.

// 			//bool HasTripleSequence(byte[] hash, out byte val) => MathHelper.HasAny4BitSequence(hash, 3, out val);
// //			bool HasTripleSequence(byte[] hash) => MathHelper.HasAny4BitSequence(hash, 3, out var _);

// //			var tripletFinder = new Md5HashFinder(HasTripleSequence);
// 			var start = 0;
// 			var index = 0;

// 			var memo = new Dictionary<int, byte[]>();

// 			byte[] Rehash2016x(byte[] hash, int iter)
// 			{
// 				if (!memo.TryGetValue(iter, out var result))
// 				{
// 					var md5 = MD5.Create();
// 					for (var i = 0; i < 2016; i++)
// 					{
// 						var sb = new StringBuilder();
// 						foreach (var b in hash)
// 						{
// 							sb.Append(b.ToString("x2"));
// 						}
// 						var ba = Encoding.ASCII.GetBytes(sb.ToString());
// 						hash = md5.ComputeHash(ba, 0, ba.Length);
// 					}
// 					memo[iter] = hash;
// 					result = hash;
// 				}
// 				return result;
// 			}

// 			for (var i = 0; i < 64; i++)
// 			{
// 				// var triplet = tripletFinder.FindMatches(salt, start).First();
// 				while (true)
// 				{
// 					byte hexval = 0;
// 					var tripletFinder = new Md5HashFinder((byte[] hash, int iter) => Rehash2016x(hash, iter).HasAnyHexSequence(3, out hexval));
// 					var seq3 = tripletFinder.FindMatches(salt, start, int.MaxValue).First();
// 					index = seq3.Iterations;
// 					start = seq3.Iterations + 1;

// 					var seq5Finder = new Md5HashFinder((byte[] hash, int iter) => Rehash2016x(hash, iter).HasHexSequence(5, hexval));
// 					var next = seq5Finder.FindMatches(salt, start, start + 1000).Count();

// 					if (next == 1)
// 					{
// 						//Console.WriteLine($"[i={i} {index}]");
// 						break;
// 					}
// 				}
// 			}
// 			//Console.WriteLine();

// 			// var match = finder.FindMatches(salt, 0).Skip(63).First();

// 			return index;

// 		}



		protected override int Part2(string[] input)
		{
			var salt = input[0];

			// It contains three of the same character in a row, like 777. Only consider the first such triplet in a hash.
			// One of the next 1000 hashes in the stream contains that same character five times in a row, like 77777.

			//bool HasTripleSequence(byte[] hash, out byte val) => MathHelper.HasAny4BitSequence(hash, 3, out val);
//			bool HasTripleSequence(byte[] hash) => MathHelper.HasAny4BitSequence(hash, 3, out var _);

//			var tripletFinder = new Md5HashFinder(HasTripleSequence);
			var start = 0;
			var index = 0;

			var memo = new Dictionary<int, byte[]>();

			var byteAsString = new byte[256][];
			for (var i = 0; i < 256; i++)
			{
				byteAsString[i] = Encoding.ASCII.GetBytes(i.ToString("x2"));
			}


			byte[] Rehash2016x(byte[] hash, int iter)
			{
				if (!memo.TryGetValue(iter, out var result))
				{
					var md5 = MD5.Create();
					for (var i = 0; i < 2016; i++)
					{
						var bi = 0;
						var ba = new byte[hash.Length * 2];
						for (var j = 0; j < hash.Length; j++)
						{
							var s = byteAsString[hash[j]];
							ba[bi++] = s[0];
							ba[bi++] = s[1];
						}
						hash = md5.ComputeHash(ba, 0, ba.Length);
					}
					memo[iter] = hash;
					result = hash;
				}
				return result;
			}

			for (var i = 0; i < 64; i++)
			{
				// var triplet = tripletFinder.FindMatches(salt, start).First();
				while (true)
				{
					byte hexval = 0;
					var tripletFinder = new Md5HashFinder((byte[] hash, int iter) => Rehash2016x(hash, iter).HasAnyHexSequence(3, out hexval));
					var seq3 = tripletFinder.FindMatches(salt, start, int.MaxValue).First();
					index = seq3.Iterations;
					start = seq3.Iterations + 1;

					var seq5Finder = new Md5HashFinder((byte[] hash, int iter) => Rehash2016x(hash, iter).HasHexSequence(5, hexval));
					var next = seq5Finder.FindMatches(salt, start, start + 1000).Count();

					if (next == 1)
					{
						//Console.WriteLine($"[i={i} {index}]");
						break;
					}
				}
			}
			//Console.WriteLine();

			// var match = finder.FindMatches(salt, 0).Skip(63).First();

			return index;

		}


	}




	//  internal class Puzzle : ComboParts<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//		public override string Name => "";
	//  	public override int Year => 2016;
	//  	public override int Day => 14;
	//  
	//  	public void Run()
	//  	{
	//  		RunFor("test1", 0, 0);
	//  		RunFor("test2", 0, 0);
	//  		RunFor("input", 0, 0);
	//  	}
	//  
	//  	protected override (int, int) Part1And2(string[] input)
	//  	{
	//  
	//  
	//  
	//  
	//  
	//  		return (0, 0);
	//  	}
	//  }

}
