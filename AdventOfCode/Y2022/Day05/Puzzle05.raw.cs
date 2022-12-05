using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day05.Raw
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 5";
		public override int Year => 2022;
		public override int Day => 5;

		public void Run()
		{
			Run("test1").Part1("CMZ").Part2("MCD");
			//Run("test2").Part1("").Part2(0);
			Run("input").Part1("QGTHFZBHV").Part2("MGDMPSZTM");
		}

		protected override string Part1(string[] input)
		{
			var x = input.GroupByEmptyLine().ToArray();
			var crates = x[0]
				.Select(s => {
					var crate = new Stack<char>();
					foreach (var ch in s)
					{
						crate.Push(ch);
					}
					return crate;
				})
				.ToArray();

			;
			var moves = x[2]
				.Select(s => {
					var (crate, from, to) = s.RxMatch("move %d from %d to %d").Get<int, int, int>();
					return (Crate: crate, From: from-1, To: to-1);
				})
				.ToArray();

			foreach (var m in moves)
			{
				for (var i = 0; i < m.Crate; i++)
				{
					crates[m.To].Push(crates[m.From].Pop());
				}
			}

			var s = "";
			foreach (var c in crates)
			{
				s += c.Pop();
			}

			return s;
		}

		protected override string Part2(string[] input)
		{

			var x = input.GroupByEmptyLine().ToArray();
			var crates = x[0]
				.Select(s => {
					var crate = new Stack<char>();
					foreach (var ch in s)
					{
						crate.Push(ch);
					}
					return crate;
				})
				.ToArray();

			;
			var moves = x[2]
				.Select(s => {
					var (crate, from, to) = s.RxMatch("move %d from %d to %d").Get<int, int, int>();
					return (Crate: crate, From: from-1, To: to-1);
				})
				.ToArray();

			foreach (var m in moves)
			{
				var ss = new Stack<char>();
				for (var i = 0; i < m.Crate; i++)
				{
					ss.Push(crates[m.From].Pop());
				}
				for (var i = 0; i < m.Crate; i++)
				{
					crates[m.To].Push(ss.Pop());
				}
			}

			var s = "";
			foreach (var c in crates)
			{
				s += c.Pop();
			}

			return s;
		}
	}
}
