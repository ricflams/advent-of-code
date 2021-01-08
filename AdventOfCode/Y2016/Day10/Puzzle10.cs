using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2016.Day10
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Balance Bots";
		public override int Year => 2016;
		public override int Day => 10;

		public void Run()
		{
			//RunFor("test1", 0, 0);
			//RunFor("test2", 0, 0);
			RunFor("input", 98, 4042);
		}

		internal interface IReceiver
		{
			void Send(int chip);
		}

		internal class Output : IReceiver
		{
			public int Chip;
			public void Send(int chip)
			{
				Chip = chip;
			}
		}

		internal class Bot : IReceiver
		{
			private int _chip;
			private int _id;
			private IReceiver _low;
			private IReceiver _high;

			public Bot(int id)
			{
				_id = id;
			}

			public void SendOutputTo(IReceiver low, IReceiver high)
			{
				_low = low;
				_high = high;
			}

			public void Send(int chip)
			{
				if (_chip == 0)
				{
					_chip = chip;
				}
				else
				{
					if (_chip == 61 && chip == 17 || _chip == 17 && chip == 61)
					{

					}
					_low.Send(Math.Min(_chip, chip));
					_high.Send(Math.Max(_chip, chip));
					_chip = 0;
				}
			}
		}



		protected override int Part1(string[] input)
		{
			// var result = 0;
			// foreach (var line in input)
			// {
			// 	if (line.MaybeRegexCapture("bot %d gives low to bot %d and high to bot %d").Get(out int id).Get(out int idlow).Get(out int idhigh).IsMatch)
			// 	{
			// 		if (idlow == 61 && idhigh == 17 || idlow == 17 && idhigh == 61)
			// 		{
			// 			result = id;
			// 			break;
			// 		}
			// 	}
			// }
			// return result;


			var bots = new Dictionary<int, Bot>();
			var outputs = new Dictionary<int, Output>();
			foreach (var line in input)
			{
				if (line.MaybeRegexCapture("bot %d gives low to %s %d and high to %s %d").Get(out int id).Get(out string outlow).Get(out int idlow).Get(out string outhigh).Get(out int idhigh).IsMatch)
				{
					var bot = GetOrCreateBot(id);
					var reclo = GetOrCreateReceiver(outlow, idlow);
					var rechi = GetOrCreateReceiver(outhigh, idhigh);
					bot.SendOutputTo(reclo, rechi);
					// {
					// 	Console.WriteLine($"Send from {id} to {idlow} and {idhigh}");
					// 	if (low == 61 && high == 17 || low == 17 && high == 61)
					// 	{
					// 		Console.WriteLine("########### Bam: bot " + id);
					// 	}
					// 	botlo.Send(low);
					// 	bothi.Send(high);
					// });
				}
				// if (line.MaybeRegexCapture("value %d goes to bot %d").Get(out int chip).Get(out int id2).IsMatch)
				// {
				// 	var bot = GetOrCreateBot(id2);
				// 	bot.Send(chip);
				// }
			}
			foreach (var line in input)
			{
				if (line.MaybeRegexCapture("value %d goes to bot %d").Get(out int chip).Get(out int id2).IsMatch)
				{
					var bot = GetOrCreateBot(id2);
					bot.Send(chip);
				}
			}
			// foreach (var line in input)
			// {
			// 	if (line.MaybeRegexCapture("value %d goes to bot %d").Get(out int chip).Get(out int id).IsMatch)
			// 	{
			// 		var bot = GetOrCreateBot(id);
			// 		bot.Send(chip);
			// 	}
			// }

			IReceiver GetOrCreateReceiver(string type, int id)
			{
				switch (type)
				{
					case "bot": return GetOrCreateBot(id);
					case "output": return GetOrCreateOutput(id);
					default: throw new Exception($"Unknown type {type}");
				}
			}

			Bot GetOrCreateBot(int id)
			{
				if (!bots.TryGetValue(id, out var bot))
				{
					bot = bots[id] = new Bot(id);
				}
				return bot;
			}

			Output GetOrCreateOutput(int id)
			{
				if (!outputs.TryGetValue(id, out var output))
				{
					output = outputs[id] = new Output();
				}
				return output;
			}


		//	var botx = bots.Where(x => x.Value.Outputs.Item1 == 61 && x.Value.Outputs.Item2 == 17 || x.Value.Outputs.Item1 == 17 && x.Value.Outputs.Item2 == 61).Select(x => x.Key).First();

			return (int)outputs.Where(x => x.Key < 3).Select(x => x.Value.Chip).Prod();
		}

		protected override int Part2(string[] input)
		{





			return 0;
		}
	}




	//  internal class Puzzle : ComboParts<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//		public override string Name => "";
	//  	public override int Year => 2016;
	//  	public override int Day => 10;
	//  
	//  	public void Run()
	//  	{
	//  		//RunFor("test1", 0, 0);
	//  		//RunFor("test2", 0, 0);
	//  		RunFor("input", 0, 0);
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
