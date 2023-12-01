using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace AdventOfCode.Helpers.Puzzles
{
	internal class FileSystem(string root)
	{
		public static readonly FileSystem Instance = new(".");

		private readonly string _root = root;
		private readonly FileCache _cache = new(root);

		public string[] ReadFile(int year, int day, string name)
		{
			var input = name == "input"
				? _cache.CachedInputFile(year, day)
				: File.ReadAllLines(Path.Combine(_root, $"Y{year}/Day{day:D2}/{name}.txt"));
			return input;
		}
	}

	internal class FileCache
	{
		private readonly string CookieFilename = "aoc-sessioncookie";
		private readonly string _cacheRoot;
		private readonly string _sessionCookie;
		private int _throttleInterval = 0;

		public FileCache(string parent)
		{
			Console.WriteLine(Directory.GetCurrentDirectory());
			_sessionCookie = File.Exists(CookieFilename) ? File.ReadAllText(CookieFilename) : null;
			_cacheRoot = Path.Combine(parent, "cache");
		}

		public string[] CachedInputFile(int year, int day)
		{
			var filename = Path.Combine(_cacheRoot, $"{year}_{day:D2}_input.txt");

			if (!File.Exists(filename))
			{
				if (_sessionCookie == null)
				{
					throw new Exception($"Missing cookie file '{CookieFilename}'");
				}
				if (string.IsNullOrEmpty(_sessionCookie))
				{
					throw new Exception($"Missing cookie in file '{CookieFilename}'");
				}

				// Ensure cache folder exists
				Directory.CreateDirectory(_cacheRoot);

				var apiUrl = $"https://adventofcode.com/{year}/day/{day}/input";
				using var client = new HttpClient();
				client.DefaultRequestHeaders.Add("Cookie", $"session={_sessionCookie}");
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "github.com/ricflams/advent-of-code by richard@flamsholt.dk");

				var response = client.GetAsync(apiUrl).Result;
				if (!response.IsSuccessStatusCode)
				{
					Console.Error.WriteLine($"No input yet for day {day}, {year}");
					return [];
				}

				var content = response.Content.ReadAsStringAsync().Result;
				File.WriteAllText(filename, content);

				Task.Delay(_throttleInterval).Wait();

				if (_throttleInterval < 2000)
					_throttleInterval += 1000;
			}

			var input = File.ReadAllLines(filename);
			return input;
		}
	}
}
