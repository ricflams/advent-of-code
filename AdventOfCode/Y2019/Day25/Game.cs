using AdventOfCode.Helpers;
using AdventOfCode.Y2019.Intcode;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2019.Day25
{
	internal class Game
	{
		private readonly Engine _engine;
		private IGameController _controller;
		private StringBuilder _outputBuffer = new StringBuilder();

		public string RawOutput { get; private set; }
		public List<string> Inventory { get; } = new List<string>();
		public Dictionary<string, Room> Rooms { get; } = new Dictionary<string, Room>();
		public Room CurrentRoom { get; private set; }
		public string[] Message { get; private set; }

		public int Password { get; private set; }

		public Game()
		{
			_engine = new Engine()
				.WithMemoryFromFile("Y2019/Day25/input.txt")
				.OnOutput(engine =>
				{
					var result = engine.Output.TakeAll().ToList().Select(v => (char)v).ToList();
					foreach (var ch in result)
					{
						_outputBuffer.Append(ch);
						if (_outputBuffer.Length > 2000) // Bail on infinite output
						{
							engine.Halt = true;
						}
						if (ch == '\n' && _outputBuffer.ToString().EndsWith("Command?\n"))
						{
							RawOutput = _outputBuffer.ToString();
							UpdateState(RawOutput);
							var instruction = _controller.WhatNext(this).Trim() + '\n';
							//System.Console.WriteLine($"Instruction: {instruction}");
							foreach (var c in instruction.ToArray())
							{
								engine.Input.Add(c);
							}
							_outputBuffer.Clear();
						}
					}
				});
		}

		public Game WithController(IGameController controller)
		{
			_controller = controller;
			return this;
		}

		public Game Run()
		{
			_engine.Execute();
			_controller.OnGameOver(this);

			var match = Regex.Match(_outputBuffer.ToString(), @"\d{6,}");
			Password = match.Success ? int.Parse(match.Value) : 0;

			return this;
		}

		private void UpdateState(string output)
		{
			var outputLines = output.Split('\n')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim())
				.Where(x => x != "Command?")
				.ToList();

			UpdateState(outputLines);

			void UpdateState(List<string> lines)
			{
				string PeekLine() => lines.FirstOrDefault() ?? "";
				string ReadLine() { var line = lines.First(); lines.RemoveAt(0); return line; }

				// == Stables ==
				// Reindeer-sized. They're all empty.
				//
				// Doors here lead:
				// - east
				// - west
				//
				// Items here:
				// - space law space brochure
				//
				// Command?
				if (PeekLine().StartsWith("=="))
				{
					var name = ReadLine().Replace("==", "").Trim();
					if (!Rooms.TryGetValue(name, out var room))
					{
						room = new Room
						{
							Name = name,
							IsPressureSensitiveFloor = name == "Pressure-Sensitive Floor",
							IsSecurityCheckpoint = name == "Security Checkpoint"
						};
						Rooms[name] = room;
					}
					CurrentRoom = room;

					room.Description = ReadLine();

					room.Doors.Clear();
					if (PeekLine() == "Doors here lead:")
					{
						ReadLine();
						while (PeekLine().StartsWith("-"))
						{
							var door = ReadLine().TrimStart('-').Trim();
							room.Doors.Add(door);
						}
					}

					room.Items.Clear();
					if (PeekLine() == "Items here:")
					{
						ReadLine();
						while (PeekLine().StartsWith("-"))
						{
							var item = ReadLine().TrimStart('-').Trim();
							room.Items.Add(item);
						}
					}
				}

				// take boulder
				//   
				// You take the boulder.
				//   
				// Command?
				var YouTakeThe = "You take the ";
				if (PeekLine().StartsWith(YouTakeThe))
				{
					var message = ReadLine();
					var item = message.Substring(YouTakeThe.Length).TrimEnd('.');
					Inventory.Add(item);
					CurrentRoom.Items.Remove(item);
				}

				// drop boulder
				//   
				// You drop the boulder.
				//   
				// Command?
				var YouDropThe = "You drop the ";
				if (PeekLine().StartsWith(YouDropThe))
				{
					var message = ReadLine();
					var item = message.Substring(YouDropThe.Length).TrimEnd('.');
					Inventory.Remove(item);
					CurrentRoom.Items.Add(item);
				}

				// Special treatment for pressure-sensitive floor
				//
				// == Pressure-Sensitive Floor ==
				//   Analyzing...
				//   
				// Doors here lead:
				// 	-north
				//   
				// A loud, robotic voice says "Alert! Droids on this ship are heavier than the detected value!" and you are ejected back to the checkpoint.
				//   
				//   
				//   
				// == Security Checkpoint ==
				// In the next room, a pressure-sensitive floor will verify your identity.
				//   
				// Doors here lead:
				// 	-north
				// 	-south
				//   
				// Command ?
				// ......
				// == Pressure-Sensitive Floor ==
				// Analyzing...
				// 
				// Doors here lead:
				// - north
				// 
				// A loud, robotic voice says "Analysis complete! You may proceed." and you enter the cockpit.
				// Santa notices your small droid, looks puzzled for a moment, realizes what has happened, and radios your ship directly.
				// "Oh, hello! You should be able to get in by typing 33624080 on the keypad at the main airlock."

				if (CurrentRoom.IsPressureSensitiveFloor && PeekLine().Contains("Alert!"))
				{
					ReadLine();
					UpdateState(lines);
				}
				else
				{
					Message = lines.ToArray();
				}
			}
		}
	}

}
