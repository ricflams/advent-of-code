using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2017.Day24
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Electromagnetic Moat";
		public override int Year => 2017;
		public override int Day => 24;

		public void Run()
		{
			RunFor("test1", 31, 19);
			//RunFor("test2", 0, 0);
			RunFor("input", 1656, 0);
		}

		protected override int Part1(string[] input)
		{
			var components = ReadComponents(input);

			IEnumerable<Component[]> BuildBridges(int startswith, Component[] comps)
			{
				var fittings = comps.Where(c => c.Fits(startswith)).ToArray();
				foreach (var f in fittings)
				{
					yield return new Component[] { f };
					var endswith = f.Pa == startswith ? f.Pb : f.Pa;
					var bridges = BuildBridges(endswith, comps.Where(x => x != f).ToArray()).ToArray();
					foreach (var b in bridges)
					{
						var bridge = new Component[] { f }.ToList().Concat(b).ToArray();
						yield return bridge;
					}
				}
			}

			var bridges = BuildBridges(0, components).ToArray();
			var strengths = bridges.Select(b => b.Sum(x => x.Strength)).Max();


			return strengths;
		}

		protected override int Part2(string[] input)
		{

			var components = ReadComponents(input);

			IEnumerable<Component[]> BuildBridges(int startswith, Component[] comps)
			{
				var fittings = comps.Where(c => c.Fits(startswith)).ToArray();
				foreach (var f in fittings)
				{
					yield return new Component[] { f };
					var endswith = f.Pa == startswith ? f.Pb : f.Pa;
					var bridges = BuildBridges(endswith, comps.Where(x => x != f).ToArray()).ToArray();
					foreach (var b in bridges)
					{
						var bridge = new Component[] { f }.ToList().Concat(b).ToArray();
						yield return bridge;
					}
				}
			}

			var bridges = BuildBridges(0, components).ToArray();
			var maxlen = bridges.Select(b => b.Length).Max();
			var strength = bridges.Where(b => b.Length == maxlen).Select(b => b.Sum(x => x.Strength)).Max();


			return strength;
		}

		internal class Component
		{
			public Component(int a, int b) => (Pa, Pb) = (a, b);
			public int Pa { get; set;}
			public int Pb { get; set;}
			public int Strength => Pa + Pb;
			public bool Fits(int x )=> x == Pa || x ==  Pb;
		}

		private static Component[] ReadComponents(string[] input)
		{
			return input.Select(line =>
			{
				var ports = line.Split('/');
				return new Component(int.Parse(ports[0]), int.Parse(ports[1]));

			})
			.ToArray();
		}
	}
}
