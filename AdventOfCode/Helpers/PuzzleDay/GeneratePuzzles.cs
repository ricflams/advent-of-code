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
			var csproj = File.CreateText($"Templates/Y{yyyy}/itemgroup.txt");

			csproj.WriteLine("  <ItemGroup>");
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
				File.Create($"{folder}/input.txt");

				csproj.WriteLine($"    <None Update=\"Y{yyyy}\\Day{dd}\\test1.txt\">");
				csproj.WriteLine($"      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
				csproj.WriteLine($"    </None>");
				csproj.WriteLine($"    <None Update=\"Y{yyyy}\\Day{dd}\\test2.txt\">");
				csproj.WriteLine($"      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
				csproj.WriteLine($"    </None>");
				csproj.WriteLine($"    <None Update=\"Y{yyyy}\\Day{dd}\\input.txt\">");
				csproj.WriteLine($"      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
				csproj.WriteLine($"    </None>");
			}
			csproj.WriteLine("  </ItemGroup>");
			csproj.Close();

			//  <ItemGroup>
			//    <None Update="Y2019\Day22\input.txt">
			//      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
			//    </None>
			//    ....
			//  </ItemGroup>

		}
	}
}
