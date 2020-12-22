using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;


namespace AdventOfCode.Y2020.Day16
{
	// TODO: Cleanup
	internal class Puzzle : ComboParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 16;

		public void Run()
		{
			//RunPart1For("test1", 71);
			//RunPart2For("test2", 0);
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

		protected override (long, long) Part1And2(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();

			var fields = parts[0]
				.Select(x =>
				{
					x.RegexCapture("%*: %d-%d or %d-%d")
						.Get(out string name)
						.Get(out int opt1min)
						.Get(out int opt1max)
						.Get(out int opt2min)
						.Get(out int opt2max);
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

			var yourticket = parts[1][1].Split(",").Select(int.Parse).ToArray();

			var allNearby = parts[2]
				.Skip(1)
				.Select(x => x.Split(",").Select(int.Parse).ToArray())
				.ToArray();

			var errorCount = allNearby
				.SelectMany(t => t.Where(v => !fields.Any(f => f.IsValid(v))))
				.Sum();


			var all = allNearby
				.Where(t => t.All(v => fields.Any(f => f.IsValid(v))))
				.Prepend(yourticket)
				.ToArray();

			// Turn the matrix
			var fieldMustHaveTheseValues = new int[yourticket.Length][];
			for (var i = 0; i < fieldMustHaveTheseValues.Length; i++)
			{
				fieldMustHaveTheseValues[i] = new int[all.Length];
				for (var j = 0; j < all.Length; j++)
				{
					fieldMustHaveTheseValues[i][j] = all[j][i];
				}
			}


			foreach (var f in fields)
			{
				var matches = 0;
				for (var i = 0; i < fieldMustHaveTheseValues.Length; i++)
				{
					var values = fieldMustHaveTheseValues[i];
					var allMatch = values.All(v => f.IsValid(v));
					if (allMatch)
						matches++;
				}
			}


			var fields2 = fields
				.Select(f =>
				{
					var matches = 0;
					for (var i = 0; i < fieldMustHaveTheseValues.Length; i++)
					{
						var values = fieldMustHaveTheseValues[i];
						var allMatch = values.All(v => f.IsValid(v));
						//Console.WriteLine($"   index {i:D2}: {allMatch}:");
						if (allMatch)
							matches++;
					}
					return new { f, matches };
				})
				.OrderBy(x => x.matches)
				.ToArray();

			var fieldCandidates = fields
				.Select(f => new 
				{
					f,
					matches = fieldMustHaveTheseValues.Select((fv,index) => fv.All(v => f.IsValid(v)) ? index : -1).Where(x => x != -1).ToList() 
				
				})
				.OrderBy(x => x.matches.Count)
				.ToArray();

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


			var sum = 1L;
			for (var i = 0; i < order.Length; i++)
			{
				if (order[i].Name.StartsWith("departure"))
				{
					var yourvalue = (long)yourticket[i];
					sum *= yourvalue;
				}
			}

			return (errorCount, sum);
		}

	}




	//  internal class Puzzle : ComboPart<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//  	protected override int Year => 2020;
	//  	protected override int Day => 16;
	//  
	//  	public void Run()
	//  	{
	//  		RunFor("test1", null, null);
	//  		//RunFor("test2", null, null);
	//  		//RunFor("input", null, null);
	//  	}
	//  
	//  	protected override (int, int) Part1And2(string[] input)
	//  	{
	//  
	//  
	//  
	//  
	//  
	//  		return (0, 0);
	//  	}
	//  }

}