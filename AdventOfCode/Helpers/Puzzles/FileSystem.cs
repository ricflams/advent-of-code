using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdventOfCode.Helpers.Puzzles
{
	internal class FileSystem(string root)
	{
		public static readonly FileSystem Instance = new(".");
		private readonly string _root = root;

		public string[] ReadFile(int year, int day, string name)
		{
			return name switch
			{
				"input" => ReadInputFile("github", year, day),
				"extra" => ReadInputFile("google", year, day),
				_ => File.ReadAllLines(Path.Combine(_root, $"Y{year}/Day{day:D2}/{name}.txt"))
			};
		}


		private string[] ReadInputFile(string profile, int year, int day)
		{
			var cache = Path.Combine(_root, "cache", profile);
			var filename = Path.Combine(cache, $"{year}_{day:D2}_input.txt");

			if (!File.Exists(filename))
			{
				var cookieFilename = Path.Combine(_root, $"{profile}.sessioncookie");
				if (!File.Exists(cookieFilename))
				{
					throw new Exception($"Missing cookie file '{cookieFilename}'");
				}
				var sessionCookie = File.ReadAllText(cookieFilename);


				var apiUrl = $"https://adventofcode.com/{year}/day/{day}/input";
				using var client = new HttpClient();
				client.DefaultRequestHeaders.Add("Cookie", $"session={sessionCookie}");
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "github.com/ricflams/advent-of-code by richard@flamsholt.dk");

				var response = client.GetAsync(apiUrl).Result;
				if (!response.IsSuccessStatusCode)
				{
					return [];
				}

				var content = response.Content.ReadAsStringAsync().Result;
				Directory.CreateDirectory(cache);
				File.WriteAllText(filename, content);

				Task.Delay(2000).Wait();
			}

			var input = File.ReadAllLines(filename);
			return input;
		}
	}
}
