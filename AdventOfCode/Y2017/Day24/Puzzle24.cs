using AdventOfCode.Helpers.Puzzles;
using System.Linq;

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
			Run("test1").Part1(31).Part2(19);
			Run("input").Part1(1656).Part2(1642);
		}

		protected override int Part1(string[] input)
		{
			var root = ReadComponents(input);

			// Traverse all bridges to find the highest strength
			var maxStrength = 0;
			Traverse(root, 0, 0);
			return maxStrength;

			void Traverse(Component c, ulong seen, int bridgeStrength)
			{
				var strength = bridgeStrength + c.Strength;
				if (strength > maxStrength)
				{
					maxStrength = strength;
				}

				// Follow the next possible components not yet picked
				foreach (var next in c.Nexts)
				{
					if ((seen & next.Id) == 0)
					{
						Traverse(next, seen | next.Id, strength);
					}
				}
			}
		}

		protected override int Part2(string[] input)
		{
			var root = ReadComponents(input);

			// Traverse all bridges to find the highest strength
			var maxStrength = 0;
			var maxLength = 0;
			Traverse(root, 0, 0, 0);
			return maxStrength;

			void Traverse(Component comp, ulong seen, int bridgeLength, int bridgeStrength)
			{
				// If the bridge length is as high or higher than what we've seen
				// so far then update max strenght if this strength is higher
				var strength = bridgeStrength + comp.Strength;
				var length = bridgeLength + 1;
				if (length >= maxLength)
				{
					maxLength = length;
					if (strength > maxStrength)
					{
						maxStrength = strength;
					}
				}

				// Follow the next possible components not yet picked
				foreach (var next in comp.Nexts)
				{
					if ((seen & next.Id) == 0)
					{
						Traverse(next, seen | next.Id, length, strength);
					}
				}
			}
		}

		internal class Component
		{
			public Component(ulong id, int portI, int portO, int strength)
				=> (Id, PortI, PortO, Strength) = (id, portI, portO, strength);
			public ulong Id { get; }
			public int PortI { get; }
			public int PortO { get; }
			public int Strength { get; }
			public Component[] Nexts { get; set; }
		}

		private static Component ReadComponents(string[] input)
		{
			// Setup a tree-structure that's really fast to traverse. Add
			// each component twice: one for each way it can be turned, but
			// still with the same bitmask-id used for determining which we
			// have examine so far.
			var components = input
				.SelectMany((line, index) =>
				{
					var p = line.Split('/').Select(int.Parse).ToArray();
					var strength = p[0] + p[1];
					var id = 1UL << index;
					return new Component[]
					{
						new Component(id, p[0], p[1], strength),
						new Component(id, p[1], p[0], strength)
					};
				})
				.ToArray();
			foreach (var c in components)
			{
				c.Nexts = components.Where(x => x.PortI == c.PortO).ToArray();
			}

			// It's easier to just have one single root
			var root = new Component(0, 0, 0, 0);
			root.Nexts = components.Where(c => c.PortI == 0).ToArray();
			return root;
		}
	}
}
