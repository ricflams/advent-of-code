using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Security.AccessControl;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace AdventOfCode.Y2023.Day12.Raw
{
	#pragma warning disable CS8321
	#pragma warning disable CS0642

	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(1).Part2(1);
			Run("test2").Part1(4).Part2(16384);
			Run("test3").Part1(1).Part2(1);
			Run("test4").Part1(1).Part2(16);
			Run("test5").Part1(4).Part2(2500);
			Run("test6").Part1(10).Part2(506250);
			//Run("input").Part1(7771);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(7771).Part2(10861030975833);
			// 8298 too high
			Run("extra").Part1(7490).Part2(65607131946466);
		}


		protected override long Part1(string[] input)
		{
			
			// var sum1 = 0;
			// var sum2 = 0;
			// foreach (var s in input)
			// {
			// 	var xx = s.Split(' ');
			// 	var (springs, sizes) = (xx[0], xx[1].ToIntArray());
			// 	var springs2 = "."+springs+".";
			// 	var n1 = Combosxx(springs2, sizes.ToList());
			// 	var n2 = Combos(springs2, sizes);
			// 	Console.Write(n1 == n2 ? '+' : '-');
			// 	if (n1 != n2)
			// 	{
			// 		Console.WriteLine($"{s}: {n1} {n2}");
			// 	}
			// 	sum1 += n1;
			// 	sum2 += n2;
			// }
			// Console.WriteLine(sum1);
			// Console.WriteLine(sum2);

			// return sum2;


			var sum = input
				.Sum(s =>
				{
					var xx = s.Split(' ');
					var (springs, sizes) = (xx[0], xx[1].ToIntArray());
					var n = Combos("."+springs+".", sizes);
					//Console.WriteLine($"{s} - {n}");
					return n;
				});

			return sum;

			int Combosxx(string s, List<int> sizes)
			{
		//		Console.WriteLine();

		//		(s, sizes) = Reduce(s, sizes);

				var qs = s.Select((c, i) => (c, i)).Where(x => x.c == '?').Select(x => x.i).ToArray();
				if (sizes.Count == 0 || qs.Length == 0)
					return 1;

				var combos = 0;
				var N = 1UL<<qs.Length;
			//	Console.WriteLine($"  N={N}");
				for (var i = 0UL; i < N; i++)
				{
					var ss = s.ToCharArray();
					for (var b = 0; b < qs.Length; b++)
					{
						ss[qs[b]] = (i & (1UL<<b)) != 0 ? '#' : '.';
					}
					var lit = new string(ss);					
					if (Match(lit, sizes))
					{
				//		Console.WriteLine($"  match: {lit}");
						combos++;
					}
				}
				return combos;
			}
			bool Match(string s, List<int> sizes)
			{
				var damaged = s.Split('.', StringSplitOptions.RemoveEmptyEntries);
				return damaged.Select(x => x.Length).SequenceEqual(sizes);
			}
			(string, List<int>) Reduce(string s, List<int> sizes)
			{
				var s0 = s;
				var si0 = sizes.ToArray();

				s = s.Trim('.');

				while (true)
				{
					var org = s;
					while (sizes.Any() && s.StartsWith('#'))
					{
						s = s[sizes[0]..];
						s = s.TrimStart('.');
						sizes.RemoveAt(0);
						//Console.WriteLine($"{s0} {string.Join(',', si0)} 1=> {s} {string.Join(',', sizes)}");
					}
					while (sizes.Any() && s[..sizes[0]].Contains('.'))
					{
						s = s[1..];
						//Console.WriteLine($"{s0} {string.Join(',', si0)} 2=> {s} {string.Join(',', sizes)}");
					}
					if (org == s)
						break;
				}

				while (true)
				{
					var org = s;
					while (sizes.Any() && s.EndsWith('#'))
					{
						s = s[..^(sizes.Last())];
						s = s.TrimEnd('.');
						sizes.RemoveAt(sizes.Count - 1);
						//Console.WriteLine($"{s0} {string.Join(',', si0)} 3=> {s} {string.Join(',', sizes)}");
					}
					while (sizes.Any() && s[^(sizes.Last())..].Contains('.'))
					{
						s = s[..^1];
						//Console.WriteLine($"{s0} {string.Join(',', si0)} 4=> {s} {string.Join(',', sizes)}");
					}
					if (org == s)
						break;
				}

				return (s, sizes);
			}

		}

		private Dictionary<string, long> ComboLookup = new();

		private long Combos(string s, int[] sizes)
		{
			var key = s+"/"+string.Join('-', sizes);
			if (ComboLookup.TryGetValue(key, out var n))
				return n;
			n = CombosDo(s, sizes);
			ComboLookup[key] = n;
			return n;
		}

		private long CombosDo(string s, int[] sizes)
		{
			while (s.StartsWith('.'))
				s = s[1..];

			if (s.Length == 0)
				return sizes.Length == 0 ? 1 : 0;

			if (sizes.Length == 0)
				return s.All(c => c is '.' or '?') ? 1 : 0;

			var siz = sizes[0];
			if (s.Length < siz || s[..siz].Contains('.')) // invalid; can't contain .
				return s[0] == '#' ? 0 : Combos(s[1..], sizes);
			
			if (siz < s.Length && s[siz] == '#') // invalid; can't be followed by #
				return s[0] == '#' ? 0 : Combos(s[1..], sizes);

			if (s[..siz].All(c => c == '#'))
				return Combos(s[(siz+1)..], sizes[1..]);
			// if (siz == s.Length)
			// 	return 1;
			
			// if (s[..siz].All(c => c == '#'))
			// {
			// 	return Combos(s[(siz+1)..], sizes[1..]);
			// }

			// if (s[0] == '#')
			// 	return Combos(s[(siz+1)..], sizes[1..]);

			if (s[0] == '#') // must match
			{
				return Combos(s[(siz+1)..], sizes[1..]);
			}

			var c1 = Combos(s[(siz+1)..], sizes[1..]);
			var c2 = Combos(s[1..], sizes);
			//if (c1 == 0 || c2 == 0)
			if (c1 + c2 > 1)
				;
			return c1 + c2;

		//	return c1 * c2;
		}


		protected override long Part2(string[] input)
		{
			var sum = input
				.Sum(s =>
				{
					var xx = s.Split(' ');
					var (springs, sizes) = (xx[0], xx[1].ToIntArray().ToList());
					var springs2 = "." + springs+"?"+springs+"?"+springs+"?"+springs+"?"+springs + ".";
					var sizes2 = sizes.Concat(sizes).Concat(sizes).Concat(sizes).Concat(sizes).ToArray();
					var n = Combos(springs2, sizes2);
					//Console.Write('.');
					//Console.WriteLine($"{s} - {n}");
					return n;
				});
				//Console.WriteLine();

			return sum;

			// int Combos(string s, int[] sizes)
			// {
			// 	while (s.StartsWith('.'))
			// 		s = s[1..];

			// 	if (s.Length == 0 && sizes.Length == 0)
			// 		return 1;
			// 	if (s.Length == 0 || sizes.Length == 0)
			// 		return 0;

			// 	var siz = sizes[0];
			// 	if (s.Length <= siz || s[..siz].Contains('.')) // invalid; can't contain .
			// 		return 0;
				
			// 	if (siz < s.Length && s[siz] == '#') // invalid; can't be followed by #
			// 		return 0;
				
			// 	var c1 = Combos(s[siz..], sizes[1..]);
			// 	var c2 = Combos(s[1..], sizes);
			// 	return c1 + c2;
			// }

			bool Match(string s, List<int> sizes)
			{
				var damaged = s.Split('.', StringSplitOptions.RemoveEmptyEntries);
				return damaged.Select(x => x.Length).SequenceEqual(sizes);
			}

			(string, List<int>) Reduce(string s, List<int> sizes)
			{
				var s0 = s;
				var si0 = sizes.ToArray();

				s = s.Trim('.');

				while (true)
				{
					var org = s;
					while (sizes.Any() && s.StartsWith('#'))
					{
						s = s[sizes[0]..];
						s = s.TrimStart('.');
						sizes.RemoveAt(0);
						Console.WriteLine($"{s0} {string.Join(',', si0)} 1=> {s} {string.Join(',', sizes)}");
					}
					while (sizes.Any() && s[..sizes[0]].Contains('.'))
					{
						s = s[1..];
						Console.WriteLine($"{s0} {string.Join(',', si0)} 2=> {s} {string.Join(',', sizes)}");
					}
					if (org == s)
						break;
				}

				while (true)
				{
					var org = s;
					while (sizes.Any() && s.EndsWith('#'))
					{
						s = s[..^(sizes.Last())];
						s = s.TrimEnd('.');
						sizes.RemoveAt(sizes.Count - 1);
						Console.WriteLine($"{s0} {string.Join(',', si0)} 3=> {s} {string.Join(',', sizes)}");
					}
					while (sizes.Any() && s[^(sizes.Last())..].Contains('.'))
					{
						s = s[..^1];
						Console.WriteLine($"{s0} {string.Join(',', si0)} 4=> {s} {string.Join(',', sizes)}");
					}
					if (org == s)
						break;
				}

				return (s, sizes);
			}
		}
	}
}
