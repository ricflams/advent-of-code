using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventOfCode.Y2015.Day12
{
	internal class Puzzle12
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllText("Y2015/Day12/input.txt");

			var obj = JsonConvert.DeserializeObject<JToken>(input);
			var sum = CountValues(obj);
			Console.WriteLine($"Day 12 Puzzle 1: {sum}");
			Debug.Assert(sum == 156366);

			int CountValues(JToken o)
			{
				return o.Type == JTokenType.Integer
					? o.Value<int>()
					: o.Children().Sum(c => CountValues(c));
			}
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllText("Y2015/Day12/input.txt");

			var obj = JsonConvert.DeserializeObject<JToken>(input);
			var sum = CountNonRedValues(obj);
			Console.WriteLine($"Day 12 Puzzle 2: {sum}");
			Debug.Assert(sum == 96852);

			int CountNonRedValues(JToken o)
			{
				return o.Type == JTokenType.Integer
					? o.Value<int>()
					: o.Children().Where(Include).Sum(c => CountNonRedValues(c));
			}

			bool Include(JToken jt)
			{
				if (jt.Type != JTokenType.Object)
				{
					return true;
				}
				foreach (var c in jt.Children<JProperty>().Select(p => p.Value))
				{
					if (c.Type == JTokenType.String && c.Value<string>() == "red")
						return false;
				}
				return true;
			}
		}
	}
}
