using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AdventOfCode.Helpers;

namespace AdventOfCode.Helpers.PuzzleDay
{
    internal static class GeneratePuzzles
    {
		public static void Generate(int year)
		{
			var yyyy = year.ToString();
			var template = File.ReadAllText("Helpers/PuzzleDay/template.txt");

			Directory.CreateDirectory($"Templates/Y{yyyy}");

			for (var day = 1; day <= 25; day++)
			{
				var d = $"{day}";
				var dd = $"{day:D2}";
				var source = template
					.Replace("{YEAR}", yyyy)
					.Replace("{DAY}", d)
					.Replace("{DAY2}", dd)
					;
				var folder = $"Templates/Y{yyyy}/Day{dd}";
				Directory.CreateDirectory(folder);
				File.WriteAllText($"{folder}/Puzzle{dd}.cs", source);
				File.Create($"{folder}/test1.txt");
				File.Create($"{folder}/test2.txt");
				File.Create($"{folder}/test9.txt");
			}
		}
	}
}
