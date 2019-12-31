using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019.Day25.Game.Output
{
	abstract class Response
	{
		public static IEnumerable<Response> ParseFrom(string input)
		{
			var lines = input.Split('\n')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim())
				.Where(x => x != "Command?")
				.ToArray();
			var index = 0;
			string ReadLine() => lines[index++];
			string PeekLine() => index < lines.Length ? lines[index] : "";

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

			// == Passages ==
			// They're a little twisty and starting to look all alike.
			//   
			// Doors here lead:
			// -west
			//   
			// Items here:
			// -boulder
			//   
			// Command?
			if (PeekLine().StartsWith("=="))
			{
				var name = ReadLine().Replace("==", "").Trim();
				var description = ReadLine();

				var directions = new List<Direction>();
				if (PeekLine() == "Doors here lead:")
				{
					ReadLine();
					while (PeekLine().StartsWith("-"))
					{
						var door = ReadLine().TrimStart('-').Trim();
						switch (door)
						{
							case "north": directions.Add(Direction.Up); break;
							case "east": directions.Add(Direction.Right); break;
							case "south": directions.Add(Direction.Down); break;
							case "west": directions.Add(Direction.Left); break;
							default: throw new Exception("Unknown door " + door);
						}
					}
				}

				var items = new List<string>();
				if (PeekLine() == "Items here:")
				{
					ReadLine();
					while (PeekLine().StartsWith("-"))
					{
						var item = ReadLine().TrimStart('-').Trim();
						items.Add(item);
					}
				}

				//if (name == "Pressure-Sensitive Floor" && PeekLine().Contains("you are ejected back to the checkpoint"))
				//{
				//	yield return new MessageResponse { Message = ReadLine() };
				//	yield return new EjectedBackToCheckpointResponse();
				//	yield break;
				//}

				if (name == "Pressure-Sensitive Floor")
				{
					//Console.WriteLine(string.Concat(lines));
					var message = ReadLine();
					yield return new MessageResponse { Message = message };
					if (message.Contains("you are ejected back to the checkpoint"))
					{
						yield return new EjectedBackToCheckpointResponse();
						yield break;
					}
					yield break;
				}

				yield return new RoomResponse
				{
					Name = name,
					Description = description,
					Directions = directions,
					Items = items
				};
				yield break;
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
				yield return new MessageResponse { Message = message };
				yield return new TakeItemResponse
				{
					Item = message.Substring(YouTakeThe.Length).TrimEnd('.')
				};
				yield break;
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
				yield return new MessageResponse { Message = message };
				yield return new DropItemResponse
				{
					Item = message.Substring(YouDropThe.Length).TrimEnd('.')
				};
				yield break;
			}

			yield return new MessageResponse
			{
				Message = string.Join('\n', lines)
			};
		}
	}
}
