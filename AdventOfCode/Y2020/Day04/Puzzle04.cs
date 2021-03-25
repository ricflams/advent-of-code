using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Passport Processing";
		public override int Year => 2020;
		public override int Day => 4;

		public void Run()
		{
			Run("test1").Part1(2);
			Run("input").Part1(210).Part2(131);
		}

		protected override int Part1(string[] input)
		{
			var passports = GetPassports(input);
			var n = passports.Count(x => x.AreAllFieldsPresent);
			return n;
		}

		protected override int Part2(string[] input)
		{
			var passports = GetPassports(input);
			var n = passports.Count(x =>
				x.AreAllFieldsPresent &&
				x.IsByrValid &&
				x.IsIyrValid &&
				x.IsEyrValid &&
				x.IsHgtValid &&
				x.IsHclValid &&
				x.IsEclValid &&
				x.IsPidValid
			);
			return n;
		}

		private static Passport[] GetPassports(string[] input)
		{
			return input
				.GroupByEmptyLine()
				.Select(raw =>
				{
					var info = string.Join(" ", raw);
					var fields = info.Split(" ").Select(x => x.Split(":")).ToDictionary(x => x[0], x => x[1]);
					return Passport.Create(fields);
				})
				.ToArray();
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
					var (h, unit) = Hgt.RxMatch("%d%s").Get<int, string>();
					return unit == "cm" && h >= 150 && h <= 193 || unit == "in" && h >= 59 && h <= 76;
				}
			}
			public bool IsHclValid
			{
				get
				{
					if (Hcl.IsRxMatch("#%s", out var captures))
					{
						var haircolor = captures.Get<string>();
						return haircolor.Length == 6 && haircolor.All(x => char.IsDigit(x) || "abcdef".Contains(x));
					}
					return false;
				}
			}
			public bool IsEclValid => new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(Ecl);
			public bool IsPidValid => Pid.Length == 9 && Pid.All(c => char.IsDigit(c));
		}
	}
}
