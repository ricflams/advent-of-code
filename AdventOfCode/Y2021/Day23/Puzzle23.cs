using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day23
{



	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 23";
		public override int Year => 2021;
		public override int Day => 23;

		public void Run()
		{
			Run("test1").Part1(12521).Part2(44169);
			Run("input").Part1(14546).Part2(42308);
		}

        protected override long Part1(string[] input)
		{
			return Cost(input);
		}

		protected override long Part2(string[] input)
		{
			var lines = input.ToList();
			lines.InsertRange(3, new string[]
			{
              "  #D#C#B#A#  ",
              "  #D#B#A#C#  "
            });
			return Cost(lines.ToArray());
		}

		private static int Cost(string[] input)
        {
			var map = CharMap.FromArray(input);
			var map2 = new Burrow(map);
			//map2.Render().ConsoleWrite();

            var seen = new HashSet<ulong>();
			var queue = new PriorityQueue<(ulong, int), int>();
			queue.Enqueue((map2.State, 0), 0);
			var seenhits = 0;
			while (queue.TryDequeue(out var item, out var _))
			{
				var (state9, energy) = item;
				if (seen.Contains(state9))
				{
					seenhits++;
					continue;
				}
				seen.Add(state9);

				map2.State = state9;

				if (seen.Count % 100000 == 0)
					Console.WriteLine($"{seen.Count}: {energy}");

				//Console.WriteLine(state.Key);
				//state.Map.ConsoleWrite();
				//Console.WriteLine();

				if (state9 == map2.FinalState)
				{
					return energy;
				}

                foreach (var (s, e, minE) in map2.NextMoves().Where(x => !seen.Contains(x.Item1)))
                {
                    queue.Enqueue((s, energy + e), energy + e + minE);
                }
            }

			throw new Exception();
		}
	}


	internal class Burrow
	{
		public readonly int RoomHeight;
		public readonly char[] Hallway = new char[11];
		public readonly Room[] Rooms;
		private readonly Burrow _copy;
		public readonly ulong FinalState;

		internal class Room
		{
			public Room(int height, int x, int[] left, int[] right)
			{
				X = x;
				Pods = new char[height];
				HallwayVicinity = new int[][] { left, right };
			}
			public int X { get; private set; }
			public char[] Pods { get; private set; }
			public int[][] HallwayVicinity { get; private set; }
		}

		public Burrow(CharMap map)
		{
			var y0 = 2;
			RoomHeight = map.Max().Y - y0;
			Rooms = new Room[]
			{
				new Room(RoomHeight, 2, new int[] { 1, 0 }, new int[] { 3, 5, 7, 9, 10 }),
				new Room(RoomHeight, 4, new int[] { 3, 1, 0 }, new int[] { 5, 7, 9, 10 }),
				new Room(RoomHeight, 6, new int[] { 5, 3, 1, 0 }, new int[] { 7, 9, 10 }),
				new Room(RoomHeight, 8, new int[] { 7, 5, 3, 1, 0 }, new int[] { 9, 10 }),
			};
			for (var x = 0; x < 11; x++)
			{
				Hallway[x] = map[x + 1][1];
			}
			foreach (var room in Rooms)
			{
				for (var i = 0; i < RoomHeight; i++)
				{
					room.Pods[i] = map[room.X + 1][y0 + i];
				}
			}
			_copy = new Burrow(RoomHeight);

			for (var i = 0; i < Rooms.Length; i++)
			{
				for (var j = 0; j < RoomHeight; j++)
				{
					_copy.Rooms[i].Pods[j] = (char)('A' + i);
				}
			}
			FinalState = _copy.State;
		}

		public Burrow(int roomHeight)
		{
			RoomHeight = roomHeight;
			Array.Fill(Hallway, '.');
			Rooms = new Room[]
			{
				new Room(RoomHeight, 2, new int[] { 1, 0 }, new int[] { 3, 5, 7, 9, 10 }),
				new Room(RoomHeight, 4, new int[] { 3, 1, 0 }, new int[] { 5, 7, 9, 10 }),
				new Room(RoomHeight, 6, new int[] { 5, 3, 1, 0 }, new int[] { 7, 9, 10 }),
				new Room(RoomHeight, 8, new int[] { 7, 5, 3, 1, 0 }, new int[] { 9, 10 }),
			};
		}

		public Burrow Copy()
		{
			var copy = _copy;
			Array.Copy(Hallway, copy.Hallway, Hallway.Length);
			for (var i = 0; i < Rooms.Length; i++)
			{
				var dst = copy.Rooms[i].Pods;
				var src = Rooms[i].Pods;
				for (var j = 0; j < RoomHeight; j++)
				{
					dst[j] = src[j];
				}
			}
			return copy;
		}

		public int DestinationRoomIndex(char ch) => ch - 'A';
		public Room DestinationRoom(char ch) => Rooms[DestinationRoomIndex(ch)];

		public static int EnergyPerMove(char pod)
		{
			return pod switch
			{
				'A' => 1,
				'B' => 10,
				'C' => 100,
				'D' => 1000,
				_ => throw new Exception($"Unknown pod {pod}")
			};
		}

		public int MinRemainingEnergy
		{
			get
			{
				var e = 0;
				foreach (var room in Rooms)
				{
					for (var j = 0; j < RoomHeight; j++)
					{
						var pod = room.Pods[j];
						if (pod == '.')
							continue;
						var destRoom = DestinationRoom(pod);
						if (destRoom == room)
						{
							e += EnergyPerMove(pod) * (RoomHeight - (j + 1));
						}
						else
						{
							e += EnergyPerMove(pod) * ((j + 1) + Math.Abs(room.X - destRoom.X) + RoomHeight);
						}
					}
				}
				for (var x = 0; x < Hallway.Length; x++)
				{
					var pod = Hallway[x];
					if (pod == '.')
						continue;
					var destX = DestinationRoom(pod).X;
					e += EnergyPerMove(pod) * (Math.Abs(x - destX) + RoomHeight);
				}
				var tooMuch = RoomHeight * (RoomHeight - 1) / 2;
				e -= 1111 * tooMuch;
				//if (e < 0)
				//	;
				return e;
			}
		}

		public CharMap Render()
		{
			var map = RoomHeight == 2
				? CharMap.FromArray(new[]
					{
						"#############",
						"#...........#",
						"###.#.#.#.###",
						"  #.#.#.#.#  ",
						"  #########  ",
					})
				: CharMap.FromArray(new[]
					{
						"#############",
						"#...........#",
						"###.#.#.#.###",
						"  #.#.#.#.#  ",
						"  #.#.#.#.#  ",
						"  #.#.#.#.#  ",
						"  #########  ",
					});
			for (var x = 0; x < Hallway.Length; x++)
			{
				map[x + 1][1] = Hallway[x];
			}
			foreach (var room in Rooms)
			{
				for (var j = 0; j < RoomHeight; j++)
				{
					map[room.X + 1][2 + j] = room.Pods[j];
				}
			}
			return map;
		}

		public ulong State
		{
			get
			{
				var v = 0UL;
				for (var x = 0; x < Hallway.Length; x++)
				{
					v = v * 5 + Serialize(Hallway[x]);
				}
				for (var i = 0; i < Rooms.Length; i++)
				{
					for (var j = 0; j < RoomHeight; j++)
					{
						v = v * 5 + Serialize(Rooms[i].Pods[j]);
					}
				}
				return v;
				static uint Serialize(char ch) => (uint)(ch == '.' ? 0 : ch - 'A' + 1);
			}
			set
			{
				for (var i = Rooms.Length; i-- > 0;)
				{
					for (var j = RoomHeight; j-- > 0;)
					{
						var vv = value;
						value /= 5;
						Rooms[i].Pods[j] = Deserialize(vv - value * 5);
					}
				}
				for (var x = Hallway.Length; x-- > 0;)
				{
					var vv = value;
					value /= 5;
					Hallway[x] = Deserialize(vv - value * 5);
				}
				static char Deserialize(ulong v) => (char)(v == 0 ? '.' : 'A' + v - 1);
			}
		}

		public IEnumerable<(ulong, int, int)> NextMoves()
		{
			// Check if any pods can move from hallway into their destination room
			for (var x = 0; x < 11; x++)
			{
				// Only act on pods, not empty spaces
				var pod = Hallway[x];
				if (pod == '.')
					continue;
				// Bail if pod won't fit in its destination room
				var rn = DestinationRoomIndex(pod);
				var room = Rooms[rn];

				if (room.Pods[0] != '.')
					continue;

				// Check if path to destination is all clear
				var roomX = room.X;
				var (xmin, xmax) = x < roomX ? (x + 1, roomX - 1) : (roomX + 1, x - 1);
				var canMove = true;
				for (var hx = xmin; hx <= xmax && canMove; hx++)
				{
					canMove &= Hallway[hx] == '.';
				}
				if (canMove)
				{
					var noBlockers = room.Pods.All(p => p == '.' || p == pod);

					if (noBlockers)
					{
						// Okay, pod may move to its room - pick the lowest vacant spot
						for (var i = RoomHeight; i-- > 0;)
						{
							if (room.Pods[i] == '.')
							{
								var move = Copy();
								move.Hallway[x] = '.';
								move.Rooms[rn].Pods[i] = pod;
								var energy = EnergyPerMove(pod) * (Math.Abs(roomX - x) + i + 1);
								yield return (move.State, energy, move.MinRemainingEnergy);
							}
						}
					}
				}
			}

			// Check if any pods can (should) move out of their room
			for (var n = 0; n < Rooms.Length; n++)
			{
				var room = Rooms[n];
				for (var i = 0; i < RoomHeight; i++)
				{
					var pod = room.Pods[i];
					if (pod == '.')
						continue;
					var destination = DestinationRoom(pod);
					if (destination == room)
					{
						// it's in the right room, but is it blocking any others?
						var isBlocking = false;
						for (var j = i + 1; j < RoomHeight; j++)
						{
							isBlocking |= room.Pods[j] != pod;
						}
						if (!isBlocking)
							continue;
					}
					foreach (var vicinity in room.HallwayVicinity)
					{
						foreach (var x in vicinity)
						{
							var p = Hallway[x];
							if (p != '.')
								break;
							var move = Copy();
							move.Hallway[x] = pod;
							move.Rooms[n].Pods[i] = '.';
							var energy = EnergyPerMove(pod) * (i + 1 + Math.Abs(room.X - x));
							yield return (move.State, energy, move.MinRemainingEnergy);
						}
					}
					break;
				}
			}

		}
	}

}
