using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AdventOfCode.Y2015.Day12
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "JSAbacusFramework.io";
		public override int Year => 2015;
		public override int Day => 12;

		public void Run()
		{
			Run("input").Part1(156366).Part2(96852);
		}

		protected override int Part1(string[] input)
		{
			var alltext = string.Join(Environment.NewLine, input);
			var obj = JsonConvert.DeserializeObject<JToken>(alltext);
			var sum = CountValues(obj);
			return sum;

			static int CountValues(JToken o)
			{
				return o.Type == JTokenType.Integer
					? o.Value<int>()
					: o.Children().Sum(c => CountValues(c));
			}
		}

		protected override int Part2(string[] input)
		{
			var alltext = string.Join(Environment.NewLine, input);
			var obj = JsonConvert.DeserializeObject<JToken>(alltext);
			var sum = CountNonRedValues(obj);
			return sum;

			static int CountNonRedValues(JToken o)
			{
				return o.Type == JTokenType.Integer
					? o.Value<int>()
					: o.Children().Where(Include).Sum(c => CountNonRedValues(c));
			}

			static bool Include(JToken jt)
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
