using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day04
{
	internal class Puzzle04
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllText("Y2020/Day04/input.txt");
			var passports = input
				.Split("\r\n\r\n")
				.Select(raw =>
				{
					var info = raw.Replace("\r\n", " ");
					var fields = info.Split(" ").Select(x => x.Split(":")).ToDictionary(x => x[0], x => x[1]);
					return Passport.Create(fields);
				});

			var result1 = passports.Count(x => x.AreAllFieldsPresent);

			var result2 = passports.Count(x =>
				x.AreAllFieldsPresent &&
				x.IsByrValid &&
				x.IsIyrValid &&
				x.IsEyrValid &&
				x.IsHgtValid &&
				x.IsHclValid &&
				x.IsEclValid &&
				x.IsPidValid
			);

			Console.WriteLine($"Day 04 Puzzle 1: {result1}");
			Console.WriteLine($"Day 04 Puzzle 1: {result2}");
			Debug.Assert(result1 == 210);
			Debug.Assert(result2 == 131);
		}

		private class Passport
		{
			public static Passport Create(Dictionary<string, string> fields)
			{
				return new Passport
				{
					Byr = fields.TryGetValue("byr", out var byr) ? byr : null,
					Iyr = fields.TryGetValue("iyr", out var iyr) ? iyr : null,
					Eyr = fields.TryGetValue("eyr", out var eyr) ? eyr : null,
					Hgt = fields.TryGetValue("hgt", out var hgt) ? hgt : null,
					Hcl = fields.TryGetValue("hcl", out var hcl) ? hcl : null,
					Ecl = fields.TryGetValue("ecl", out var ecl) ? ecl : null,
					Pid = fields.TryGetValue("pid", out var pid) ? pid : null,
					Cid = fields.TryGetValue("cid", out var cid) ? cid : null
				};
			}

			public string Byr { get; set; }
			public string Iyr { get; set; }
			public string Eyr { get; set; }
			public string Hgt { get; set; }
			public string Hcl { get; set; }
			public string Ecl { get; set; }
			public string Pid { get; set; }
			public string Cid { get; set; }

			public bool AreAllFieldsPresent =>
				Byr != null &&
				Iyr != null &&
				Eyr != null &&
				Hgt != null &&
				Hcl != null &&
				Ecl != null &&
				Pid != null;

			public bool IsByrValid => int.Parse(Byr) >= 1920 && int.Parse(Byr) <= 2002;
			public bool IsIyrValid => int.Parse(Iyr) >= 2010 && int.Parse(Iyr) <= 2020;
			public bool IsEyrValid => int.Parse(Eyr) >= 2020 && int.Parse(Eyr) <= 2030;
			public bool IsHgtValid
			{
				get
				{
					SimpleRegex.Capture(Hgt, "%d%s")
						.Get(out int h)
						.Get(out string unit);
					return unit == "cm" && h >= 150 && h <= 193 || unit == "in" && h >= 59 && h <= 76;
				}
			}
			public bool IsHclValid =>
				SimpleRegex.IsMatch(Hcl, "#%s", out var haircolor) &&
					haircolor[0].Length == 6 &&
					haircolor[0].All(x => char.IsDigit(x) || "abcdef".Contains(x));
			public bool IsEclValid => new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(Ecl);
			public bool IsPidValid => Pid.Length == 9 && Pid.All(c => char.IsDigit(c));
		}
	}
}
