using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdventOfCode.Helpers.Puzzles
{
	internal class FileSystem
	{
		private readonly string _root;
		private readonly FileCache _cache;

		public static readonly FileSystem Instance = new(".");

		public FileSystem(string root)
		{
 			_root = root;
			_cache = new FileCache(root);
		}

		public string[] ReadFile(int year, int day, string name)
		{
			var filename = name == "input"
				? _cache.CachedInputFile(year, day).Result
				: Path.Combine(_root, $"Y{year}/Day{day:D2}/{name}.txt");
			return File.ReadAllLines(filename);
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
			_sessionCookie = File.Exists(CookieFilename) ? File.ReadAllText(CookieFilename) : null;
			_cacheRoot = Path.Combine(parent, "cache");
		}

		public async Task<string> CachedInputFile(int year, int day)
		{
			var filename = Path.Combine(_cacheRoot, $"{year}_{day:D2}_input.txt");

			if (!File.Exists(filename))
			{
				if (_sessionCookie == null)
				{
					throw new Exception($"Missing cookie file '{CookieFilename}'");
				}

				// Ensure cache folder exists
				Directory.CreateDirectory(_cacheRoot);

				var apiUrl = $"https://adventofcode.com/{year}/day/{day}/input";
				using var client = new HttpClient();
				client.DefaultRequestHeaders.Add("Cookie", $"session={_sessionCookie}");

				var response = await client.GetAsync(apiUrl);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				File.WriteAllText(filename, content);

				await Task.Delay(_throttleInterval);
				if (_throttleInterval < 2000)
					_throttleInterval += 1000;
			}
			return filename;
		}
	}
}
