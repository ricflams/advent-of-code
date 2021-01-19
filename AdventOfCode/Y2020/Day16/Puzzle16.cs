using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;


namespace AdventOfCode.Y2020.Day16
{
	internal class Puzzle : Puzzle<long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Ticket Translation";
		public override int Year => 2020;
		public override int Day => 16;

		public void Run()
		{
			RunPart1For("test1", 71);
			RunFor("input", 20048, 4810284647569);
		}

		internal class TicketField
		{
			public string Name { get; set; }
			public int R1min { get; set; }
			public int R1max { get; set; }
			public int R2min { get; set; }
			public int R2max { get; set; }
			public bool IsValid(int x) => R1min <= x && x <= R1max || R2min <= x && x <= R2max;
		}

		internal static (TicketField[], int[], int[][])  ParseInput(string[] lines)
		{
			var parts = lines.GroupByEmptyLine().ToArray();
			var fields = parts[0]
				.Select(x =>
				{
					var (name, opt1min, opt1max, opt2min, opt2max) = x
						.RxMatch("%*: %d-%d or %d-%d")
						.Get<string, int, int, int, int>();
					return new TicketField
					{
						Name = name,
						R1min = opt1min,
						R1max = opt1max,
						R2min = opt2min,
						R2max = opt2max
					};
				})
				.ToArray();
			var yourTicket = parts[1][1].Split(",").Select(int.Parse).ToArray();
			var otherTickets = parts[2]
				.Skip(1)
				.Select(x => x.Split(",").Select(int.Parse).ToArray())
				.ToArray();
			return (fields, yourTicket, otherTickets);
		}

		protected override long Part1(string[] input)
		{
			var (fields, _, otherTickets) = ParseInput(input);
			var invalids = otherTickets
				.SelectMany(t => t.Where(v => !fields.Any(f => f.IsValid(v))))
				.Sum();
			return invalids;
		}

		protected override long Part2(string[] input)
		{
			var (fields, yourTicket, otherTickets) = ParseInput(input);

			// Determine the required fields
			var allTickets = otherTickets
				.Where(t => t.All(v => fields.Any(f => f.IsValid(v))))
				.Prepend(yourTicket)
				.ToArray();
			var fieldMustHaveTheseValues = new int[yourTicket.Length][];
			for (var i = 0; i < fieldMustHaveTheseValues.Length; i++)
			{
				fieldMustHaveTheseValues[i] = new int[allTickets.Length];
				for (var j = 0; j < allTickets.Length; j++)
				{
					fieldMustHaveTheseValues[i][j] = allTickets[j][i];
				}
			}

			// Find all candidates for each field, ordered by fewest matches first so the
			// fields with the fewest matches can be examined first
			var fieldCandidates = fields
				.Select(f => new 
				{
					f,
					matches = fieldMustHaveTheseValues.Select((fv,index) => fv.All(v => f.IsValid(v)) ? index : -1).Where(x => x != -1).ToList() 
				
				})
				.OrderBy(x => x.matches.Count)
				.ToArray();

			// Fields are ordered by least matches, and they just happens to be 1, 2, ... N
			// so we can pick them out, one by one removing selected candidates along the way.
			var order = new TicketField[fieldCandidates.Length];
			foreach (var fc in fieldCandidates)
			{
				var fieldno = fc.matches[0];
				order[fieldno] = fc.f;
				foreach (var fc2 in fieldCandidates)
				{
					fc2.matches.Remove(fieldno);
				}
			}

			// Calc the product of all departure-fields on your ticket
			var product = 1L;
			for (var i = 0; i < order.Length; i++)
			{
				if (order[i].Name.StartsWith("departure"))
				{
					product *= yourTicket[i];
				}
			}
			return product;
		}
	}
}