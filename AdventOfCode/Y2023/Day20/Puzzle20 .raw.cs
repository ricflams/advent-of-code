using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Collections;
using static AdventOfCode.Y2023.Day20.Puzzle;

namespace AdventOfCode.Y2023.Day20.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 20;

		public override void Run()
		{
			Run("test1").Part1(32000000);
			Run("test2").Part1(11687500);
			Run("input").Part1(731517480).Part2(0);
			// 244178746156662 too high
			// 244178746156666 too high
			//	Run("extra").Part1(0).Part2(0);
		}

		internal enum Pulse { Low, High};

		protected override long Part1(string[] input)
		{
			var config = input.Select(s => {
				// case '%': sb.Append('%'); break;
				// case '*': sb.Append(@"(.+)"); break;
				// case 's': sb.Append(@"(\w+)"); break;
				// case 'c': sb.Append(@"(.)"); break;
				// case 'd': sb.Append(@"([-+]?\d+)"); break;
				// case 'D': sb.Append(@"\s*([-+]?\d+)"); break;					
				var xxx = s.Split(" -> ").ToArray();
				var dest = xxx[1].Split(',').Select(x => x.Trim()).ToArray();
				var type = xxx[0][0];
				return type is '%' or '&'
					? (Typ: type, Name: xxx[0][1..], Dest: dest)
					: (Typ: type, Name: xxx[0], Dest: dest);
			})
			.ToDictionary(x => x.Name, x => x);

			var broadcaster = config["broadcaster"];
			var flipFlops = config.Where(x => x.Value.Typ == '%').ToDictionary(x => x.Key, _ => false);
			var conjunction = config.Where(x => x.Value.Typ == '&')
				.ToDictionary(x => x.Key, x => config.Where(c => c.Value.Dest.Any(nam => nam == x.Key)).ToDictionary(x => x.Key, _ => Pulse.Low));

			var pulses = new List<Pulse>();

			void PushButton()
			{
				var queue = new Queue<(string, string, Pulse)>();

				pulses.Add(Pulse.Low);

				foreach (var dest in broadcaster.Dest)
				{
					queue.Enqueue((broadcaster.Name, dest, Pulse.Low));
					pulses.Add(Pulse.Low);
				}

				while (queue.TryDequeue(out var signal))
				{
					var (from, to, pulse) = signal;
					//Console.WriteLine($"{from} {pulse} {to}");

					if (!config.ContainsKey(to))
						continue;
					var dest = config[to];
					if (dest.Typ == '%')
					{
						if (pulse == Pulse.Low)
						{
							var val = flipFlops[to] = !flipFlops[to];
							foreach (var dest2 in dest.Dest)
							{
								queue.Enqueue((to, dest2, val ? Pulse.High : Pulse.Low));
								pulses.Add(val ? Pulse.High : Pulse.Low);
							}
						}
					}
					if (dest.Typ == '&')
					{
						//if (conjunction[to].ContainsKey(from))
						{
							conjunction[to][from] = pulse;
							var pulse2 = conjunction[to].Values.All(x => x == Pulse.High) ? Pulse.Low : Pulse.High;
							foreach (var dest2 in dest.Dest)
							{
								queue.Enqueue((to, dest2, pulse2));
								pulses.Add(pulse2);
							}
						}
					}
				}
			}

			//PushButton();

			for (var i = 0; i < 1000; i++)
				PushButton();

			return pulses.Count(p => p == Pulse.Low) * pulses.Count(p => p == Pulse.High);
		}


		protected override long Part2(string[] input)
		{
			var config = input.Select(s => {
				// case '%': sb.Append('%'); break;
				// case '*': sb.Append(@"(.+)"); break;
				// case 's': sb.Append(@"(\w+)"); break;
				// case 'c': sb.Append(@"(.)"); break;
				// case 'd': sb.Append(@"([-+]?\d+)"); break;
				// case 'D': sb.Append(@"\s*([-+]?\d+)"); break;					
				var xxx = s.Split(" -> ").ToArray();
				var dest = xxx[1].Split(',').Select(x => x.Trim()).ToArray();
				var type = xxx[0][0];
				return type is '%' or '&'
					? (Typ: type, Name: xxx[0][1..], Dest: dest)
					: (Typ: type, Name: xxx[0], Dest: dest);
			})
			.ToDictionary(x => x.Name, x => x);

			var broadcaster = config["broadcaster"];
			var flipFlops = config.Where(x => x.Value.Typ == '%').ToDictionary(x => x.Key, _ => false);
			var conjunction = config.Where(x => x.Value.Typ == '&')
				.ToDictionary(x => x.Key, x => config.Where(c => c.Value.Dest.Any(nam => nam == x.Key)).ToDictionary(x => x.Key, _ => Pulse.Low));

			//var pulses = new List<Pulse>();
			var rxLowSent = false;

			var pushes = 0;
			var seen = new Dictionary<string, long>();

			void PushButton()
			{
				var queue = new Queue<(string, string, Pulse)>();

				//pulses.Add(Pulse.Low);

				foreach (var dest in broadcaster.Dest)
				{
					queue.Enqueue((broadcaster.Name, dest, Pulse.Low));
					if (dest == "rx")
						rxLowSent = true;
				}

				while (queue.TryDequeue(out var signal))
				{
					var (from, to, pulse) = signal;
					//Console.WriteLine($"{from} {pulse} {to}");

					if (to == "rx")
						{ }
					if (!config.ContainsKey(to))
						continue;

					var dest = config[to];
					if (dest.Typ == '%')
					{
						if (pulse == Pulse.Low)
						{
							var val = flipFlops[to] = !flipFlops[to];
							foreach (var dest2 in dest.Dest)
							{
								queue.Enqueue((to, dest2, val ? Pulse.High : Pulse.Low));
								if (to == "rx" && !val)
									rxLowSent = true;
							}
						}
					}
					if (dest.Typ == '&')
					{
						//if (conjunction[to].ContainsKey(from))
						{
							conjunction[to][from] = pulse;
							var pulse2 = conjunction[to].Values.All(x => x == Pulse.High) ? Pulse.Low : Pulse.High;
							if (pulse == Pulse.High && (from=="pv" || from == "qh" || from == "xm" || from == "hz"))
							{
								Console.WriteLine($"{to} {pushes}");
								if (seen.ContainsKey(from))
								{
									var prev = seen[from];
									if (pushes % prev != 0)
										throw new Exception();
								}
								else
									seen[from] = pushes;
								if (seen.Count == 4)
								{
									var p = MathHelper.LeastCommonMultiple(seen.Values.ToArray());
									//var pp = p + 5;
									Console.WriteLine(p);
									;
								}
							}
							foreach (var dest2 in dest.Dest)
							{
								queue.Enqueue((to, dest2, pulse2));
								if (to == "rx" && pulse2 == Pulse.Low)
									rxLowSent = true;
							}
						}
					}
				}
			}

			//PushButton();

			do
			{
				pushes++;
				PushButton();
			} while (!rxLowSent);

			return pushes;
		}
	}
}
