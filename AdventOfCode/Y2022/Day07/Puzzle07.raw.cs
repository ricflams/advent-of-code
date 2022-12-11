using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day07.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 7";
		public override int Year => 2022;
		public override int Day => 7;

		public void Run()
		{
			Run("test1").Part1(95437).Part2(24933642);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(1444896).Part2(404395);
		}


// $ cd /
// $ ls
// dir a
// 14848514 b.txt
// 8504156 c.dat
// dir d
// $ cd a
// $ ls
// dir e
// 29116 f
// 2557 g

		class File
		{
			public string Name;
			public int Size;
		}
		class Dir
		{
			//public string Name;
			public List<File> Files = new List<File>();
			public Dictionary<string, Dir> Subdirs = new Dictionary<string, Dir>();
			public int Size => Files.Sum(x => x.Size) + Subdirs.Sum(d => d.Value.Size);
		}

		private void TraverseForSmallSizes(Dir dir, ref int totalsum)
		{
			if (dir.Size <= 100000)
			{
				totalsum += dir.Size;
			}
			foreach (var subdir in dir.Subdirs.Values)
			{
				TraverseForSmallSizes(subdir, ref totalsum);
			}
		}

		private void ReadFs(Dir dir, string[] input, ref int pos)
		{
			var line = input[pos++];
			if (line != "$ ls")
				throw new Exception();
			while (pos < input.Length)
			{
				var entry = input[pos++];
				if (entry == "$ cd ..")
					break;
				if (entry.StartsWith("$ cd "))
				{
					var name = entry[5..];
					var subdir = dir.Subdirs[name];
					ReadFs(subdir, input, ref pos);
				}
				else if (entry.StartsWith("dir "))
				{
					var name = entry[4..];
					dir.Subdirs[name] = new Dir();
				}
				else
				{
					var (size, name) = entry.RxMatch("%d %s").Get<int,string>();
					var file = new File { Name = name, Size = size};
					dir.Files.Add(file);
				}
			}
		}



		protected override long Part1(string[] input)
		{
			var pos = 1;
			var root = new Dir();
			ReadFs(root, input, ref pos);

			var totalsize = 0;
			TraverseForSmallSizes(root, ref totalsize);



			return totalsize;
		}

		protected override long Part2(string[] input)
		{
			var pos = 1;
			var root = new Dir();
			ReadFs(root, input, ref pos);

			var fssize = 70000000;
			var free = 30000000;
			var used = root.Size;
			var needed = -(fssize - free - used);
			Console.WriteLine(used);
			Console.WriteLine(needed);

			var dirs = FindAllDirs(root)
				.ToArray();
			var dirsize = dirs
				.OrderBy(d => d.Size)
				.Where(d => d.Size >= needed)
				.First()
				.Size;

			return dirsize;

			// 40389918 not right
		}


		private IEnumerable<Dir> FindAllDirs(Dir dir)
		{
			yield return dir;
			foreach (var subdir in dir.Subdirs.Values)
			{
				foreach (var d in FindAllDirs(subdir))
				{
					yield return d;
				}
			}
		}

	}
}
