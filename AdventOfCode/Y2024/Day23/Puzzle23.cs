using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day23
{
	internal class Puzzle : Puzzle<long, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(7).Part2("co,de,ka,ta");
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(1046).Part2("de,id,ke,ls,po,sn,tf,tl,tm,uj,un,xw,yz");
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var conns = input.Select(s => s.Split('-')).Select(x => (a: x[0], b: x[1])).ToArray();

			var sets = new SafeDictionary<string, HashSet<string>>(() => []);
			foreach (var (a, b) in conns)
			{
				sets[a].Add(b);
				sets[b].Add(a);
			}


			// Console.WriteLine("digraph {");
			// foreach (var node in sets)
			// {
			// 	var name = node.Key;
			// 	foreach (var name2 in node.Value)
			// 	{
			// 		Console.WriteLine($"  \"{name}\" -> \"{name2}\"");
			// 	}
			// }
			// Console.WriteLine("}");


			// var sizes = sets.OrderByDescending(x => x.Value.Count).Select(x => $"{x.Key} {x.Value.Count}").ToArray();

			var n = 0;
			foreach (var node in sets)
			{
				var name = node.Key;
				foreach (var name2 in node.Value)
				{
					var node2 = sets[name2];
					foreach (var name3 in node2)
					{
						var node3 = sets[name3];
						if (node3.Contains(name))
						{
							// found set of three
							if (name[0] == 't' || name2[0] == 't' || name3[0] == 't')
								n++;
						}
					}
				}
			}

			n /= 6;

			return n;
		}

		private class Set : HashSet<string>
		{
			public Set() : base() { }
			public Set(IEnumerable<string> arg) : base(arg) { }
		}

		protected override string Part2(string[] input)
		{
			var conns = input.Select(s => s.Split('-')).Select(x => (a: x[0], b: x[1])).ToArray();

			var connection = new SafeDictionary<string, Set>(() => []);
			foreach (var (a, b) in conns)
			{
				connection[a].Add(b);
				connection[b].Add(a);
			}

			var groups = connection.Keys.Select(key => new Set([key])).ToList();

			while (groups.Count > 1)
			{
				Console.WriteLine(groups.Count);
				var expanded = new Dictionary<string, Set>();
				foreach (var name in connection.Keys)
				{
					foreach (var group in groups)
					{
						if (group.Contains(name))
							continue;
						var groupNames = group.Append(name).OrderBy(x => x).ToArray();
						var key = string.Join('-', groupNames);
						if (expanded.ContainsKey(key))
							continue;
						if (IsGroup(groupNames))
						{
							expanded[key] = new Set(groupNames);
						}
					}
				}

				groups = [.. expanded.Values];
			}

			var code = string.Join(',', groups.Single().OrderBy(x => x));
			return code;


			// var largestGroup = FindLargestGroup([.. connection.Keys]);

			// Set FindLargestGroup(List<string> names)
			// {
			// 	if (names.Count == 1)
			// 		return new Set([names[0]]);

			// 	if (names.Count == 2)
			// 		return connection[names[0]].Contains(names[1]) ? new Set(names) : [];

			// 	var groups = names.Select(key => new Set([key])).ToList();
			// 	while (groups.Count > 1)
			// 	{
			// 		var expanded = new List<Set>();
			// 		var all = new Set(groups.SelectMany(x => x));
			// 		foreach (var group in groups)
			// 		{
			// 			var neighborhood = all
			// 				.Where(x => !group.Contains(x))
			// 				.Where(x => group.All(g => connection[g].Contains(x)))
			// 				.ToList();
			// 			var friends = FindLargestGroup(neighborhood);
			// 			if (friends.Any())
			// 			{
			// 				var newgroup = new Set(group.Concat(friends));
			// 				expanded.Add(newgroup);
			// 			}
			// 		}
			// 		groups = expanded;
			// 	}

			// 	return groups.Count > 0 ? groups.First() : [];
			// }

			bool IsGroup(string[] names)
			{
				for (var i = 0; i < names.Length; i++)
				{
					var a = names[i];
					for (var j = i + 1; j < names.Length; j++)
					{
						var b = names[j];
						if (!connection[a].Contains(b))
							return false;
					}
				}
				return true;
			}

		}
	}
}





// // var all = new Set(connection.Keys);
// // var groups = all.Select(key => new Set([key])).ToList();
// var largestGroup = FindLargestGroup([.. connection.Keys]);

// Set FindLargestGroup(List<string> names)
// {
// 	if (names.Count == 1)
// 		return new Set([names[0]]);

// 	if (names.Count == 2)
// 		return connection[names[0]].Contains(names[1]) ? new Set(names) : [];

// 	var groups = names.Select(key => new Set([key])).ToList();
// 	while (groups.Count > 1)
// 	{
// 		var expanded = new List<Set>();
// 		var all = new Set(groups.SelectMany(x => x));
// 		foreach (var group in groups)
// 		{
// 			var neighborhood = all
// 				.Where(x => !group.Contains(x))
// 				.Where(x => group.All(g => connection[g].Contains(x)))
// 				.ToList();
// 			var friends = FindLargestGroup(neighborhood);
// 			if (friends.Any())
// 			{
// 				var newgroup = new Set(group.Concat(friends));
// 				expanded.Add(newgroup);
// 			}
// 		}
// 		groups = expanded;
// 	}

// 	return groups.Count > 0 ? groups.First() : [];
// }