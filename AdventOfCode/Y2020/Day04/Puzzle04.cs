using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2020.Day04
{
	internal class Puzzle04
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllText("Y2020/Day04/input.txt");
			var xx = input.Split("\r\n\r\n");
			var passports = xx.Select(p =>
			{
				p = p.Replace("\r\n", " ");
				var fields = p.Split(" ").Select(x => x.Split(":")).ToDictionary(x => x[0], x => x[1]);
				return new Passport
				{
					byr = fields.TryGetValue("byr", out var byr) ? byr : null,
					iyr = fields.TryGetValue("iyr", out var iyr) ? iyr : null,
					eyr = fields.TryGetValue("eyr", out var eyr) ? eyr : null,
					hgt = fields.TryGetValue("hgt", out var hgt) ? hgt : null,
					hcl = fields.TryGetValue("hcl", out var hcl) ? hcl : null,
					ecl = fields.TryGetValue("ecl", out var ecl) ? ecl : null,
					pid = fields.TryGetValue("pid", out var pid) ? pid : null,
					cid = fields.TryGetValue("cid", out var cid) ? cid : null
				};
			});

			var result1 = passports.Count(x =>
				x.byr != null &&
				x.iyr != null &&
				x.eyr != null &&
				x.hgt != null &&
				x.hcl != null &&
				x.ecl != null &&
				x.pid != null);

			var result2 = passports.Count(x =>
			{
				if (!(x.byr != null &&
				x.iyr != null &&
				x.eyr != null &&
				x.hgt != null &&
				x.hcl != null &&
				x.ecl != null &&
				x.pid != null))
					return false;



				var byr = int.Parse(x.byr);
				if (byr < 1920 || byr > 2002)
					return false;
				var iyr = int.Parse(x.iyr);
				if (iyr < 2010 || iyr > 2020)
					return false;
				var eyr = int.Parse(x.eyr);
				if (eyr < 2020 || eyr > 2030)
					return false;
				SimpleRegex.Capture(x.hgt, "%d%s")
					.Get(out int heightValue)
					.Get(out string heightUnit);
				switch (heightUnit)
				{
					case "cm":
						if (heightValue < 150 || heightValue > 193)
							return false;
						break;
					case "in":
						if (heightValue < 59 || heightValue > 76)
							return false;
						break;
					default:
						return false;
				}

				if (!SimpleRegex.IsMatch(x.hcl, "#%s", out var haircolor))
					return false;
				if (haircolor[0].Length != 6)
					return false;
				if (!haircolor[0].All(x => char.IsDigit(x) || "abcdef".Contains(x)))
					return false;

				if (x.ecl != "amb" && x.ecl != "blu" && x.ecl != "brn" && x.ecl != "gry" && x.ecl != "grn" && x.ecl != "hzl" && x.ecl != "oth")
					return false;
				if (x.pid.Length != 9)
					return false;
				if (!x.pid.All(c => char.IsDigit(c)))
					return false;
				return true;
			});


			Console.WriteLine($"Day 04 Puzzle 1: {result1}");
			Console.WriteLine($"Day 04 Puzzle 1: {result2}");
			Debug.Assert(result2 == 210);
			Debug.Assert(result2 == 131);
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("Y2020/Day04/input.txt");

			//Console.WriteLine($"Day 04 Puzzle 2: {result}");
			//Debug.Assert(result == );
		}

		private class Passport
		{
			public string byr { get; set; }
			public string iyr { get; set; }
			public string eyr { get; set; }
			public string hgt { get; set; }
			public string hcl { get; set; }
			public string ecl { get; set; }
			public string pid { get; set; }
			public string cid { get; set; }
		}

	}
}
