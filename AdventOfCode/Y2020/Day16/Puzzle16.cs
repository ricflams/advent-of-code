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
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 16;

		public void Run()
		{
			RunPart1For("test1", 71);
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

		protected override long Part1(string[] input)
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

			var nearby = parts[2]
				.Skip(1)
				.Select(x => x.Split(",").Select(int.Parse).ToArray())
				.ToArray();

			var errors = nearby
				.SelectMany(t => t.Where(v => !fields.Any(f => f.IsValid(v))))
				.Sum();


			return errors;
		}

		protected override long Part2(string[] input)
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


			//var candidates = all
			//	.Select(ticket =>
			//	{
			//		var validfor = fields.Select((f, i) => f.IsValid())
			//	})
			//	.ToArray();


			foreach (var f in fields)
			{
			//	Console.WriteLine($"For field {f.Name}:");
				var matches = 0;
				for (var i = 0; i < fieldMustHaveTheseValues.Length; i++)
				{
					var values = fieldMustHaveTheseValues[i];
					var allMatch = values.All(v => f.IsValid(v));
				//	Console.WriteLine($"   index {i:D2}: {allMatch}:");
					if (allMatch)
						matches++;
				}
			//	Console.WriteLine($"   matches={matches}:");
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

			//var fieldCandidates = fieldMustHaveTheseValues
			//	.Select(values => fields.Where(f => values.All(v => f.IsValid(v))).ToList())
			//	//.OrderBy(x => x.Count)
			//	.ToArray();

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
				System.Diagnostics.Debug.Assert(fc.matches.Count == 1);
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
				//	Console.WriteLine($"  mult by {yourvalue}: sum={sum}");
				}
			}

			//for (var i = 0; i < fieldCandidates.Length; i++)
			//{
			//	//System.Diagnostics.Debug.Assert(fieldCandidates[i].Count == 1);
			//	////var nextfc = fieldCandidates.First(x => x.Count == 1);
			//	var field = fields2[i].f;
			//	var yourvalue = (ulong)yourticket[i];
			//	Console.WriteLine($"{field.Name}: {yourvalue}");
			//	if (field.Name.StartsWith("departure"))
			//	{
			//		sum *= yourvalue;
			//		Console.WriteLine($"  mult by {yourvalue}: sum={sum}");
			//	}
			//	//foreach (var fc in fieldCandidates)
			//	//{
			//	//	fc.Remove(field);
			//	//}
			//}


			//your ticket:
			//61,151,59,101,173,71,103,167,127,157,137,73,181,97,179,149,131,139,67,53

			//var sum = 1UL;
			//for (var i = 0; i < fieldCandidates.Length; i++)
			//{
			//	System.Diagnostics.Debug.Assert(fieldCandidates[i].Count == 1);
			//	//var nextfc = fieldCandidates.First(x => x.Count == 1);
			//	var field = fieldCandidates[i].First();
			//	var yourvalue = (ulong)yourticket[i];
			//	Console.WriteLine($"{field.Name}: {yourvalue}");
			//	if (field.Name.StartsWith("departure"))
			//	{
			//		sum *= yourvalue;
			//		Console.WriteLine($"  mult by {yourvalue}: sum={sum}");
			//	}
			//	foreach (var fc in fieldCandidates)
			//	{
			//		fc.Remove(field);
			//	}
			//}

			//for (var i = 0; i < fieldCandidates.Length; i++)
			//{
			//	if (fieldCandidates[i].Name.StartsWith("departure"))
			//	{
			//		sum *= yourticket[i];
			//	}
			//}
		//	Console.WriteLine(sum);

			//var fieldorder = new TicketField[fieldMustHaveTheseValues.Length];
			//for (var i = 0; i < fieldorder.Length; i++)
			//{
			//	var fc = fi
			//}


			////void ExploreField(int fieldNo)
			////{
			////	Console.WriteLine(string.Join(" ", fieldorder[0..fieldNo].Select(x => x.Name)));

			////	if (fieldNo == fieldMustHaveTheseValues.Length)
			////	{
			////		Console.WriteLine("Done!");

			////		var sum = 0;
			////		for (var i = 0; i < fieldorder.Length; i++)
			////		{
			////			if (fieldorder[i].Name.StartsWith("departure"))
			////			{
			////				sum *= yourticket[i];
			////			}
			////		}
			////		Console.WriteLine(sum);

			////	}
			////	var validFieldValues = fieldMustHaveTheseValues[fieldNo];
			////	var fieldCandidates = fields
			////		.Where(f => !fieldorder.Contains(f))
			////		.Where(f => validFieldValues.All(v => f.IsValid(v)));
			////	foreach (var fc in fieldCandidates)
			////	{
			////		fieldorder[fieldNo] = fc;
			////		ExploreField(fieldNo + 1);
			////	}
			////	fieldorder[fieldNo] = null;
			////}

			//ExploreField(0);

			return sum;
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