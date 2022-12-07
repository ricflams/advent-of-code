using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day07
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "No Space Left On Device";
		public override int Year => 2022;
		public override int Day => 7;

		public void Run()
		{
			Run("test1").Part1(95437).Part2(24933642);
			Run("input").Part1(1444896).Part2(404395);
		}

		protected override long Part1(string[] input)
		{
			var root = ReadFilesystem(input);
			return SumOfSmallSizes(root);

			static int SumOfSmallSizes(Dir dir) =>
				(dir.Size <= 100000 ? dir.Size : 0) + dir.Subdirs.Sum(SumOfSmallSizes);
		}

		protected override long Part2(string[] input)
		{
			var root = ReadFilesystem(input);

			var disksize = 70000000;
			var free = 30000000;
			var needed = root.Size + free - disksize;

			return Flatten(root)
				.OrderBy(d => d.Size)
				.First(d => d.Size >= needed)
				.Size;

			static IEnumerable<Dir> Flatten(Dir dir)
			{
				yield return dir;
				foreach (var subdir in dir.Subdirs.SelectMany(Flatten))
					yield return subdir;
			}
		}

		private class Dir
		{
			public int Size;
			public List<Dir> Subdirs = new List<Dir>();
			public void AddSubdir(Dir d) { Subdirs.Add(d); Size += d.Size; }
		}

		private static Dir ReadFilesystem(string[] input)
		{
			var pos = 1; // skip "$ cd /"
			return ReadDir();

			Dir ReadDir()
			{
				pos++; // skip "$ ls"
				var dir = new Dir();
				while (pos < input.Length)
				{
					var line = input[pos++];
					if (line == "$ cd ..")
						break;
					if (line.StartsWith("$ cd"))
						dir.AddSubdir(ReadDir());
					else if (line.StartsWith("dir"))
						{} // do nothing, they're visited by the cd
					else
						dir.Size += int.Parse(line.Split()[0]);
				}
				return dir;
			}
		}
	}
}
