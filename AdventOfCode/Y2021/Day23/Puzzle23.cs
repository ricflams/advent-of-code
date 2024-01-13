using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Amphipod";
		public override int Year => 2021;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(12521).Part2(44169);
			Run("input").Part1(14546).Part2(42308);
			Run("extra").Part1(11120).Part2(49232);
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
              "  #D#C#B#A#",
              "  #D#B#A#C#"
            });
			return Cost(lines.ToArray());
		}

		private static int Cost(string[] input)
        {
			var map = CharMap.FromArray(input);
			var burrow = new Burrow(map);
			return burrow.Solve();
		}
	}


	internal class Burrow
	{
		public readonly int RoomSize;
		public readonly char[] Hallway = Enumerable.Repeat('.', 11).ToArray();
		public readonly Room[] Rooms;
		public readonly ulong FinalState;

		internal class Room
		{
			public Room(int size, char pod, int x, int[] left, int[] right)
			{
				X = x;
				Pod = pod;
				Pods = Enumerable.Repeat(pod, size).ToArray();
				HallwayVicinity = new int[][] { left, right };
			}
			public int X { get; private set; }
			public char Pod { get; private set; }
			public char[] Pods { get; private set; }
			public int[][] HallwayVicinity { get; private set; }

			public bool HasWrongPods()
            {
				foreach (var pod in Pods)
                {
					if (pod != '.' && pod != Pod)
						return true;
                }
				return false;
			}
		}

		public Burrow(CharMap map)
		{
			var y0 = 2; // Rooms start at y==2
			RoomSize = map.MinMax().Item2.Y - y0;
			Rooms = new Room[]
			{
				new Room(RoomSize, 'A', 2, new int[] { 1, 0 }, new int[] { 3, 5, 7, 9, 10 }),
				new Room(RoomSize, 'B', 4, new int[] { 3, 1, 0 }, new int[] { 5, 7, 9, 10 }),
				new Room(RoomSize, 'C', 6, new int[] { 5, 3, 1, 0 }, new int[] { 7, 9, 10 }),
				new Room(RoomSize, 'D', 8, new int[] { 7, 5, 3, 1, 0 }, new int[] { 9, 10 }),
			};

			// Rooms starts out containing the right pods (AAAA, BBBB, etc) so the
			// initial state is also the desired final state
			FinalState = State;

			// Populate the rooms with the pods from the map
			foreach (var room in Rooms)
			{
				for (var i = 0; i < room.Pods.Length; i++)
				{
					room.Pods[i] = map[room.X + 1][y0 + i];
				}
			}
		}

		public int Solve()
		{
			var seen = new HashSet<ulong>();
			var queue = new PriorityQueue<(ulong, int), int>();
			queue.Enqueue((State, 0), 0);

			while (queue.TryDequeue(out var item, out var _))
			{
				var (state, energy) = item;
				if (seen.Contains(state))
				{
					continue;
				}
				seen.Add(state);

				if (state == FinalState)
				{
					return energy;
				}
				State = state;

				foreach (var (s, e, minE) in NextMoves().Where(x => !seen.Contains(x.state)))
				{
					queue.Enqueue((s, energy + e), energy + e + minE);
				}
			}

			throw new Exception("Could not solve");
		}

		public IEnumerable<(ulong state, int, int)> NextMoves()
		{
			// Check if pods can move from hallway into their destination room
			for (var x = 0; x < Hallway.Length; x++)
			{
				// Only act on pods, not empty spaces
				var pod = Hallway[x];
				if (pod == '.')
					continue;

				// If there are no wrong pods in the room then there will always be
				// room for any pods found in the hallway, so just check for the
				// presence of wrong pods in the room
				var room = DestinationRoom(pod);
				if (room.HasWrongPods())
					continue;

				// Check if path to destination room is all clear; if so then move pod
				var step = x < room.X ? 1 : -1;
				for (var hx = x + step; Hallway[hx] == '.'; hx += step)
				{
					if (hx == room.X)
					{
						// Pod may move to lowest vacant spot in its room
						for (var i = RoomSize; i-- > 0;)
						{
							if (room.Pods[i] == '.')
							{
								Hallway[x] = '.';
								room.Pods[i] = pod;
								var energy = EnergyPerMove(pod) * (Math.Abs(room.X - x) + i + 1);
								yield return (State, energy, MinimumRemainingEnergy);
								Hallway[x] = pod;
								room.Pods[i] = '.';
								break;
							}
						}
						break;
					}
				}
			}

			// Check if any pods should move out of their room
			foreach (var room in Rooms.Where(r => r.HasWrongPods()))
			{
				for (var i = 0; i < room.Pods.Length; i++)
				{
					var pod = room.Pods[i];
					if (pod == '.')
						continue;
					// Explore the room's vicinities, left and right, for vacancies
					foreach (var vicinity in room.HallwayVicinity)
					{
						foreach (var x in vicinity)
						{
							if (Hallway[x] != '.')
								break;
							Hallway[x] = pod;
							room.Pods[i] = '.';
							var energy = EnergyPerMove(pod) * (Math.Abs(room.X - x) + i + 1);
							yield return (State, energy, MinimumRemainingEnergy);
							Hallway[x] = '.';
							room.Pods[i] = pod;
						}
					}
					break;
				}
			}
		}

		private Room DestinationRoom(char ch) => Rooms[ch - 'A'];

		private static int EnergyPerMove(char pod)
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

		private int MinimumRemainingEnergy
		{
			get
			{
				// Calculate how much energy it will take to move every pod from
				// its current location and straight to the lowest position in its
				// destination room.
				var e = 0;
				foreach (var room in Rooms)
				{
					for (var i = 0; i < RoomSize; i++)
					{
						var pod = room.Pods[i];
						if (pod == '.')
							continue;
						var destination = DestinationRoom(pod);
						if (destination == room)
						{
							// Move to bottom of room
							e += EnergyPerMove(pod) * (RoomSize - (i + 1));
						}
						else
						{
							// Move out of the wrong room, down the hallway, to bottom of right room
							e += EnergyPerMove(pod) * (i + 1 + Math.Abs(room.X - destination.X) + RoomSize);
						}
					}
				}
				for (var x = 0; x < Hallway.Length; x++)
				{
					var pod = Hallway[x];
					if (pod == '.')
						continue;
					var destination = DestinationRoom(pod);
					e += EnergyPerMove(pod) * (Math.Abs(x - destination.X) + RoomSize);
				}

				// The calculated energy is for moving all pods to the bottom of their
				// destination room. But that's more than what's needed; it's 1+2+3+...
				// more moves than needed. Subtract that many moves for each of the
				// 4 types of pods, ie for 1+10+100+1000 = 1111 energies.
				var tooManyMoves = RoomSize * (RoomSize - 1) / 2;
				e -= 1111 * tooManyMoves;

				return e;
			}
		}

		private ulong State
		{
			get
			{
				// Serialize the state into a ulong. Going bit by bit requires more
				// than 64 bits, but the total number of states can be encoded as
				// values of 5.
				var v = 0UL;
				for (var x = 0; x < Hallway.Length; x++)
				{
					v = v * 5 + Serialize(Hallway[x]);
				}
				foreach (var room in Rooms)
                {
					foreach (var pod in room.Pods)
                    {
						v = v * 5 + Serialize(pod);
					}
				}
				return v;
				static uint Serialize(char ch) => (uint)(ch == '.' ? 0 : ch - 'A' + 1);
			}
			set
			{
				// For deserialization make sure to do the operations in reverse
				foreach (var room in Rooms.Reverse())
				{
                    for (var j = RoomSize; j-- > 0;)
                    {
                        var vv = value;
                        value /= 5;
                        room.Pods[j] = Deserialize(vv - value * 5);
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
	}
}
