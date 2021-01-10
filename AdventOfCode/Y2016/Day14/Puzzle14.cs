using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AdventOfCode.Y2016.Day14
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "One-Time Pad";
		public override int Year => 2016;
		public override int Day => 14;

		public void Run()
		{
			RunFor("test1", 22728, 22551);
			RunFor("input", 25427, 22045);
		}

		protected override int Part1(string[] input)
		{
			var salt = input[0];
			var hasher = new Hasher(salt, Md5Hash);
			return hasher.FindAtIndex64();
		}

		protected override int Part2(string[] input)
		{
			var salt = input[0];
			var hasher = new Hasher(salt, Md5Hash2016x);
			return hasher.FindAtIndex64();
		}

		private static byte[] Md5Hash(MD5 md5, string salt, int i)
		{
			var str = salt + i.ToString();
			var ba0 = Encoding.ASCII.GetBytes(str);
			var hash = md5.ComputeHash(ba0, 0, ba0.Length);
			return hash;
		}

		private static byte[] Md5Hash2016x(MD5 md5, string salt, int i)
		{
			var hash = Md5Hash(md5, salt, i);
			var ba = new byte[hash.Length * 2];
			for (var loop = 0; loop < 2016; loop++)
			{
				var bi = 0;
				for (var j = 0; j < hash.Length; j++)
				{
					var s = Hasher.ByteAsString[hash[j]];
					ba[bi++] = s[0];
					ba[bi++] = s[1];
				}
				hash = md5.ComputeHash(ba, 0, ba.Length);
			}
			return hash;
		}

		internal class Hasher
		{
			private const int Parallelism = 2000;
			private readonly string _salt;
			private readonly Func<MD5, string, int, byte[]> _hashing;
			private readonly MD5[] _md5s;

			public Hasher(string salt, Func<MD5, string, int, byte[]> hashing)
			{
				_salt = salt;
				_hashing = hashing;
				_md5s = Enumerable.Range(0, Parallelism).Select(_ => MD5.Create()).ToArray();
			}

			public static readonly byte[][] ByteAsString;

			static Hasher()
			{
				ByteAsString = new byte[256][];
				for (var i = 0; i < 256; i++)
				{
					ByteAsString[i] = Encoding.ASCII.GetBytes(i.ToString("x2"));
				}
			}

			public int FindAtIndex64()
			{
				var iteration = 0;
				var seq3Iteration = 0;
				for (var loop = 0; loop < 64; loop++)
				{
					// It contains three of the same character in a row, like 777. Only consider the first such triplet in a hash.
					// One of the next 1000 hashes in the stream contains that same character five times in a row, like 77777.
					for (var found = false; !found; iteration++)
					{
						var hash = GetHash(iteration);
						if (hash.HasAnyHexSequence(3, out var hexval))
						{
							for (var j = 0; j < 1000; j++)
							{
								hash = GetHash(iteration + j + 1);
								if (hash.HasHexSequence(5, hexval))
								{
									seq3Iteration = iteration;
									found = true;
									break;
								}
							}
						}
					}
					//Console.WriteLine($"[#{loop} found at {seq3Iteration}]");
				}
				return seq3Iteration;			
			}

			private readonly Dictionary<int, byte[]> _memo = new Dictionary<int, byte[]>();

			private byte[] GetHash(int iteration)
			{
				if (!_memo.TryGetValue(iteration, out var result))
				{
					Parallel.For(0, Parallelism, j =>
					{
						var md5 = _md5s[j];
						var i = iteration + j;
						var hash = _hashing(md5, _salt, i);
						lock (_memo)
						{
							_memo[i] = hash;
						}
					});
					result = _memo[iteration];
				}
				return result;
			}
		}
	}
}
