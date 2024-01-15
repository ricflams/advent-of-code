using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;

namespace AdventOfCode.Y2023.Day15.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(1320).Part2(145);
		//	Run("test2").Part1(0).Part2(0);
			Run("input").Part1(516070).Part2(0);
		//	Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var seq = input[0].Split(',').ToArray();
			var sum = seq.Sum(Hash);

			return sum;
		}

		private int Hash(string s)
		{
			var v = 0;
			foreach (var ch in s.ToCharArray())
			{
				v = (char)(((int)(v+ch) * 17) % 256);
			}
			return v;
		}

		//private char Hash(char ch) => (char)((((int)ch + 1) * 17) % 256);

		protected override long Part2(string[] input)
		{
			var seq = input[0].Split(',').ToArray();
			var boxes = Enumerable.Repeat(0, 256).Select(_ => new List<(string Label, int Focal)>()).ToArray();

			foreach (var s in seq)
			{
				if (s.IsRxMatch("%s=%d", out var captures))
				{
					var (label, focal) = captures.Get<string, int>();
					var boxidx = Hash(label);
					var box = boxes[boxidx];
					var found = false;
					for (var i = 0; !found && i < box.Count; i++)
					{
						if (box[i].Label == label)
						{
							box[i] = (label, focal);
							found = true;
						}
					}
					if (!found)
					{
						box.Add((label, focal));
					}
				}
				else
				{
					var label = s.Split('-')[0];
					var box = Hash(label);
					boxes[box].RemoveAll(x => x.Label == label);
				}
			}

			for (var i = 0; i < boxes.Length; i++)
			{
				if (boxes[i].Any())
				{
					var lenses = string.Join(' ', boxes[i].Select(x => $"[{x.Label} {x.Focal}]"));
					Console.WriteLine($"Box {i}: {lenses}");
				}
			}

			var focusingPower = boxes.Select((b,idx) =>
			{
				var fp = 0L;
				for (var i = 0; i < b.Count(); i++)
				{
					fp += (1L + idx) * (long)(i+1)*b[i].Focal;
				}
				return fp;
			})
			.Sum();

			return focusingPower;
		}
	}
}
