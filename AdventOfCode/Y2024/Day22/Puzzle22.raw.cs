using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day22.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Monkey Market";
		public override int Year => 2024;
		public override int Day => 22;

		public override void Run()
		{
			Run("test1").Part1(37327623);
			Run("test2").Part2(23);
			Run("input").Part1(17577894908).Part2(1931);

			// 1962 too high
			// 1946 too high
			// 1909 too low

			Run("extra").Part1(20411980517).Part2(2362);
		}

		protected override long Part1(string[] input)
		{
			var numbers = input.Select(uint.Parse).ToArray();

			var sum = numbers.Sum(n =>
			{
				for (var i = 0; i < 2000; i++)
				{
					n ^= n << 6;
					n %= 0x1000000;
					n ^= n / (2 << 4);
					n %= 0x1000000;
					n ^= n << 11;
					n %= 0x1000000;
				}
				return n;
			});

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var numbers = input.Select(uint.Parse).ToArray();

			var buyers = numbers.Select(n =>
			{
				var N = 2001;
				//n = 123;

				var digits = new int[N];
				for (var i = 0; i < N; i++)
				{
					digits[i] = (int)(n % 10);
					n ^= n << 6;
					n %= 0x1000000;
					n ^= n / (2 << 4);
					n %= 0x1000000;
					n ^= n << 11;
					n %= 0x1000000;
				}

				var diffs = new int[N]; // 0 is unused
				for (var i = 1; i < diffs.Length; i++)
				{
					diffs[i] = digits[i] - digits[i - 1];
				}

				//var prices = new Dictionary<uint, int>();
				var prices = new Dictionary<string, int>();
				for (var i = 4; i < diffs.Length; i++)
				{
					var id = PriceKey(diffs, i);
					var val = digits[i];
					if (prices.ContainsKey(id))
						continue;
					prices[id] = val;
				}

				return prices;
			});

			//static uint PriceKey(int[] v, int i) => (uint)((v[i - 3] + 10) * 20 * 20 * 20 + (v[i - 2] + 10) * 20 * 20 + (v[i - 1] + 10) * 20 + v[i] + 10);
			static string PriceKey(int[] v, int i) => $"{v[i - 3]},{v[i - 2]},{v[i - 1]},{v[i]}";//(uint)((v[i - 3] + 10) * 20 * 20 * 20 + (v[i - 2] + 10) * 20 * 20 + (v[i - 1] + 10) * 20 + v[i] + 10);

			//var maxBananas = new SafeDictionary<uint, int>();
			var maxBananas = new SafeDictionary<string, int>();
			foreach (var b in buyers)
			{
				foreach (var (key, v) in b)
				{
					//maxBananas[key] += v;
					maxBananas[key] = maxBananas[key] + v;
				}
			}

			//var bananas = maxBananas.OrderByDescending(x => x.Value).Take(10).ToArray();
			return maxBananas.Values.Max();


			// var maxBananas = 0;
			// var pricekey = new int[4];
			// for (var i1 = 0; i1 < 10; i1++)
			// {
			// 	pricekey[0] = i1;
			// 	for (var i2 = 0; i2 < 10; i2++)
			// 	{
			// 		pricekey[1] = i2;
			// 		for (var i3 = 0; i3 < 10; i3++)
			// 		{
			// 			pricekey[2] = i3;
			// 			for (var i4 = 0; i4 < 10; i4++)
			// 			{
			// 				pricekey[3] = i4;
			// 				var key = PriceKey(pricekey, 0);
			// 				var bananas = buyers.Sum(b => b.TryGetValue(key, out var v) ? v : 0);
			// 				if (bananas > maxBananas)
			// 					maxBananas = bananas;
			// 			}
			// 		}
			// 	}
			// }

			// return maxBananas;
		}
	}
}
