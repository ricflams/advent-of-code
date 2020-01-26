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
			var yyyy = $"Y{year:D4}";
			var template = File.ReadAllText("Helpers/PuzzleDay/template.txt");
			
			Console.WriteLine("  <ItemGroup>");
			for (var day = 1; day <= 25; day++)
			{
				var dd = $"{day:D2}";
				var source = template
					.Replace("{YEAR}", yyyy)
					.Replace("{DAY}", dd);
				var folder = $"{yyyy}/Day{dd}";
				Directory.CreateDirectory(folder);
				File.WriteAllText($"{folder}/Puzzle{dd}.cs", source);
				File.Create($"{folder}/input.txt");

				Console.WriteLine($"    <None Update=\"{yyyy}\\Day{dd}\\input.txt\">");
				Console.WriteLine($"      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
				Console.WriteLine($"    </None>");
			}
			Console.WriteLine("  </ItemGroup>");

			//  <ItemGroup>
			//    <None Update="Y2019\Day22\input.txt">
			//      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
			//    </None>
			//    ....
			//  </ItemGroup>

		}
	}
}
