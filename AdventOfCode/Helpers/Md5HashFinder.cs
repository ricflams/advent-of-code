using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode.Helpers
{
    public class Md5HashFinder
    {
		private readonly int N = Environment.ProcessorCount;
		private readonly int BatchSize = 50_000;
		private readonly Func<byte[], int, bool> _condition;

		public Md5HashFinder(Func<byte[], bool> condition)
		{
			_condition = (hash, _) => condition(hash);
		}

		public Md5HashFinder(Func<byte[], int, bool> condition)
		{
			_condition = condition;
		}

		public static bool Condition5x0(byte[] hash) => hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xf0) == 0;
		public static bool Condition6x0(byte[] hash) => hash[0] == 0 && hash[1] == 0 && hash[2] == 0;

		public class Match
		{
			public int Iterations { get; set; }
			public byte[] Hash { get; set; }
		}

		public IEnumerable<Match> FindMatches(string input, int start)
		{
			while (true)
			{
				var hashes = Enumerable.Range(0, N)
					.AsParallel()
					.WithDegreeOfParallelism(N)
					.SelectMany(i => FindMatches(input, start + i*BatchSize, start + (i+1)*BatchSize).ToArray())
					.AsSequential()
					.OrderBy(x => x.Iterations);
				foreach (var h in hashes)
				{
					yield return h;
				}
				start += N * BatchSize;
			}
		}

		public IEnumerable<Match> FindMatches(string input, int start, int end)
		{
			var md5 = MD5.Create();
			var secret = input.ToCharArray().Select(x => (byte)x).ToArray();
			var buffer = new byte[100]; // more than big enough
			Array.Copy(secret, 0, buffer, 0, secret.Length);
			for (var i = start; i < end; i++)
			{
				var guess = Encoding.ASCII.GetBytes(i.ToString());
				Array.Copy(guess, 0, buffer, secret.Length, guess.Length);
				var hash = md5.ComputeHash(buffer, 0, secret.Length + guess.Length);
				if (_condition(hash, i))
				{
					yield return new Match
					{
						Iterations = i,
						Hash = hash
					};
				}
			}
		}			
    }
}