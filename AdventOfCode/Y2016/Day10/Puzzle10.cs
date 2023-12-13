using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2016.Day10
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Balance Bots";
		public override int Year => 2016;
		public override int Day => 10;

		public override void Run()
		{
			Run("input").Part1(98).Part2(4042);
			Run("extra").Part1(47).Part2(2666);
		}

		protected override int Part1(string[] input)
		{
			var (bots, _) = RunInstructions(input);
			var botId = bots.Where(x => x.Value.HasSent17And61).Select(x => x.Key).First();
			return botId;
		}

		protected override int Part2(string[] input)
		{
			var (_, outputs) = RunInstructions(input);
			var product = (int)outputs.Where(x => x.Key < 3).Select(x => x.Value.Chip).Prod();
			return product;
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
			private IReceiver _loReceiver;
			private IReceiver _hiReceiver;
			private int _chip;

			public bool HasSent17And61 { get; private set; }

			public void SendOutputTo(IReceiver loReceiver, IReceiver hiReceiver)
			{
				_loReceiver = loReceiver;
				_hiReceiver = hiReceiver;
			}

			public void Send(int chip)
			{
				if (_chip == 0)
				{
					_chip = chip;
				}
				else
				{
					var loChip = Math.Min(_chip, chip);
					var hiChip = Math.Max(_chip, chip);
					if (loChip == 17 && hiChip == 61)
					{
						HasSent17And61 = true;
					}
					_loReceiver.Send(loChip);
					_hiReceiver.Send(hiChip);
					_chip = 0;
				}
			}
		}

		private static (IDictionary<int, Bot>, IDictionary<int, Output>) RunInstructions(string[] instructions)
		{
			var bots = new Dictionary<int, Bot>();
			var outputs = new Dictionary<int, Output>();

			// First register all the bots, and only then start sending values into them
			foreach (var line in instructions)
			{
				if (line.IsRxMatch("bot %d gives low to %s %d and high to %s %d", out var captures))
				{
					var (id, loType, loId, hiType, hiId) = captures.Get<int, string, int, string, int>();
					var bot = GetOrCreateBot(id);
					var loRecipient = GetOrCreateReceiver(loType, loId);
					var hiRecipient = GetOrCreateReceiver(hiType, hiId);
					bot.SendOutputTo(loRecipient, hiRecipient);
				}
			}
			foreach (var line in instructions)
			{
				if (line.IsRxMatch("value %d goes to bot %d", out var captures))
				{
					var (chip, id) = captures.Get<int, int>();
					var bot = GetOrCreateBot(id);
					bot.Send(chip);
				}
			}

			return (bots, outputs);

			IReceiver GetOrCreateReceiver(string type, int id) =>
				type switch
				{
					"bot" => GetOrCreateBot(id),
					"output" => GetOrCreateOutput(id),
					_ =>  throw new Exception($"Unknown type {type}")
				};
			Bot GetOrCreateBot(int id) => bots.GetOrAdd(id, () => new Bot());
			Output GetOrCreateOutput(int id) => outputs.GetOrAdd(id, () => new Output());
		}
	}
}
