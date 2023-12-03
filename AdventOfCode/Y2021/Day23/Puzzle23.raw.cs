using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day23.Raw
{


	//           6         5         4         3         2         1
	//  bit   3210987654321098765432109876543210987654321098765432109876543210
	//  has          DDDDDDCCCCCCBBBBBBAAAAAA000999888777666555444333222111000
	//               +------ rooms A-D -----++--------hallway----------------+

	static class MazeExtensions
	{
		public static byte GetHallway(this ulong v, int x)
		{
			return (byte)((v >> (x * 3)) & 7);
		}
		public static ulong SetHallway(this ulong v, int x, byte pod)
		{
			var mask = 7UL << (x * 3);
			return (v & ~mask) | (ulong)pod << (x * 3);
		}

		public static byte GetRoomTop(this ulong v, int room)
		{
			return (byte)((v >> (33 + room * 6)) & 7);
		}
		public static ulong SetRoomTop(this ulong v, int room, byte pod)
		{
			var mask = 7UL << (33 + room * 6);
			return (v & ~mask) | (ulong)pod << (33 + room * 6);
		}

		public static byte GetRoomBot(this ulong v, int room)
		{
			return (byte)((v >> (33 + 3 + room * 6)) & 7);
		}
		public static ulong SetRoomBot(this ulong v, int room, byte pod)
		{
			var mask = 7UL << (33 + 3 + room * 6);
			return (v & ~mask) | (ulong)pod << (33 + 3 + room * 6);
		}

		public static int X(this int room)
        {
			return 2 + room * 2;
        }
		public static byte DestinyPod(this int room)
		{
			return (byte)(room + 1);
		}
		public static int DestinyRoom(this byte pod)
		{
			return pod - 1;
		}
		private static readonly (int[], int[])[] RoomVicinity = new []
		{
			// Mark with x where pods may go:
			//  01234567890
			// #xx.x.x.x.xx#
			// ###.#.#.#.###
			(new int[] { 1, 0 }, new int[] { 3, 5, 7, 9, 10 }),
			(new int[] { 3, 1, 0 }, new int[] { 5, 7, 9, 10 }),
			(new int[] { 5, 3, 1, 0 }, new int[] { 7, 9, 10 }),
			(new int[] { 7, 5, 3, 1, 0 }, new int[] { 9, 10 }),
		};
		public static (int[] Left, int[] Right) Vicinity(this int room)
        {
			return RoomVicinity[room];
        }

		public static int EnergyPerMove(this byte pod)
        {
			return pod switch
			{
				1 => 1,
				2 => 10,
				3 => 100,
				4 => 1000,
				_ => throw new Exception($"Unknown pod {pod}")
			};
		}

		//  0123456789012
		// 0#############
		// 1#...........#
		// 2###D#A#D#C###
		// 3  #C#A#B#B#
		// 4  #########

		public static ulong ToMaze(this CharMap map)
        {

			static byte CharToByte(char ch) => (byte)(ch == '.' ? 0 : ch - 'A' + 1);
			return 0UL
				.SetRoomTop(0, CharToByte(map[3][2]))
				.SetRoomBot(0, CharToByte(map[3][3]))
				.SetRoomTop(1, CharToByte(map[5][2]))
				.SetRoomBot(1, CharToByte(map[5][3]))
				.SetRoomTop(2, CharToByte(map[7][2]))
				.SetRoomBot(2, CharToByte(map[7][3]))
				.SetRoomTop(3, CharToByte(map[9][2]))
				.SetRoomBot(3, CharToByte(map[9][3]));
        }

		public static CharMap ToMap(this ulong v)
		{
			var map = CharMap.FromArray(new []
            {
				"#############",
				"#...........#",
				"###.#.#.#.###",
				"  #.#.#.#.#  ",
				"  #########  ",
			});
			static char ByteToChar(byte b) => b == 0 ? '.' : (char)(b - 1 + 'A');
			map[3][2] = ByteToChar(v.GetRoomTop(0));
			map[3][3] = ByteToChar(v.GetRoomBot(0));
			map[5][2] = ByteToChar(v.GetRoomTop(1));
			map[5][3] = ByteToChar(v.GetRoomBot(1));
			map[7][2] = ByteToChar(v.GetRoomTop(2));
			map[7][3] = ByteToChar(v.GetRoomBot(2));
			map[9][2] = ByteToChar(v.GetRoomTop(3));
			map[9][3] = ByteToChar(v.GetRoomBot(3));
			for (var x = 0; x < 11; x++)
            {
				map[x + 1][1] = ByteToChar(v.GetHallway(x));
            }
			return map;
		}
	}

	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 23";
		public override int Year => 2021;
		public override int Day => 23;


		private static readonly ulong FinalState = CharMap.FromArray(new[]
		{
			"#############",
			"#...........#",
			"###A#B#C#D###",
			"  #A#B#C#D#  ",
			"  #########  ",
		}).ToMaze();


		public override void Run()
		{
			Run("test1").Part1(12521).Part2(44169);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(14546).Part2(0);
			// 14540 too low
			// 16346 too high
		}

		public static IEnumerable<(ulong, int)> NextMoves(ulong state)
        {
			// Check if any pods can move from hallway into their destination room
			for (var x = 0; x < 11; x++)
            {
				var pod = state.GetHallway(x);
				if (pod == 0)
					continue;
				var room = pod.DestinyRoom();
				var top = state.GetRoomTop(room);
				if (top != 0)
					continue; // occupado
				var roomX = room.X();
				var (xmin, xmax) = x < roomX ? (x + 1, roomX - 1) : (roomX + 1, x - 1);
				var canMove = true;
				for (var hx = xmin; hx <= xmax && canMove; hx++)
                {
					canMove &= state.GetHallway(hx) == 0;
                }
				if (canMove)
                {
					var newStateTop = state
						.SetHallway(x, 0)
						.SetRoomTop(room, pod);
					var energy = pod.EnergyPerMove() * (Math.Abs(roomX - x) + 1);
					yield return (newStateTop, energy);
					////if (state.GetRoomBot(room) == 0)
     ////               {
					////	var newStateBot = state
					////		.SetHallway(x, 0)
					////		.SetRoomBot(room, pod);
					////	energy += pod.EnergyPerMove();
					////	yield return (newStateBot, energy);
					////}
				}
			}

			// Check if any pods can (should) move out of their room
			for (var room = 0; room < 4; room++)
            {
				var top = state.GetRoomTop(room);
				var bot = state.GetRoomBot(room);
				if (top == 0 && bot == 0)
					continue;
				var destinyPod = room.DestinyPod();
                //if (top == destinyPod && bot == destinyPod) // already in place, no need to move
                //    continue;
                var roomX = room.X();
				var vicinity = room.Vicinity();
				if (top != 0)
                {
					foreach (var x in vicinity.Left)
                    {
						var p = state.GetHallway(x);
						if (p != 0)
							break;
						var newState = state
							.SetHallway(x, top)
							.SetRoomTop(room, 0);
						var energy = top.EnergyPerMove() * (1 + Math.Abs(roomX - x));
						yield return (newState, energy);
					}
					foreach (var x in vicinity.Right)
					{
						var p = state.GetHallway(x);
						if (p != 0)
							break;
						var newState = state
							.SetHallway(x, top)
							.SetRoomTop(room, 0);
						var energy = top.EnergyPerMove() * (1 + Math.Abs(roomX - x));
						yield return (newState, energy);
					}
					if (bot == 0)
                    {
						// Move down into the bottom
						var newState = state
							.SetRoomTop(room, 0)
							.SetRoomBot(room, top);
						var energy = top.EnergyPerMove();
						yield return (newState, energy);
					}
				}
				else
                {
                    // Only examine the bot pod
                    if (bot == destinyPod) // already in place, no need to move
                        continue;

                    foreach (var x in vicinity.Left)
					{
						var p = state.GetHallway(x);
						if (p != 0)
							break;
						var newState = state
							.SetHallway(x, bot)
							.SetRoomBot(room, 0);
						var energy = bot.EnergyPerMove() * (2 + Math.Abs(roomX - x));
						yield return (newState, energy);
					}
					foreach (var x in vicinity.Right)
					{
						var p = state.GetHallway(x);
						if (p != 0)
							break;
						var newState = state
							.SetHallway(x, bot)
							.SetRoomBot(room, 0);
						var energy = bot.EnergyPerMove() * (2 + Math.Abs(roomX - x));
						yield return (newState, energy);
					}
				}
			}
        }


        protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
//			map.ConsoleWrite();

			var maze = map.ToMaze();
            //var map2 = maze.ToMap();
            //map2.ConsoleWrite();

   //         var testMap = CharMap.FromArray(new[]
   //         {
   //             "#############",
   //             "#...........#",
   //             "###A#B#C#D###",
   //             "  #B#C#D#A#  ",
   //             "  #########  ",
   //         });
   //         var maze2 = testMap.ToMaze();

			//maze2.ToMap().ConsoleWrite();

			//for (var x = 0; x < 11; x++)
   //         {
   //             maze2 = maze2.SetHallway(x, (byte)((x % 3) + 1));
   //         }
			//maze2.ToMap().ConsoleWrite();
			//for (var x = 0; x < 11; x++)
   //         {
   //             var pod = maze2.GetHallway(x);
   //             maze2 = maze2.SetHallway(x, (byte)(pod + 1));
   //             var pod2 = maze2.GetHallway(x);
   //             if (pod2 != pod + 1)
   //                 ;
   //         }
   //         maze2.ToMap().ConsoleWrite();


            var seen = new HashSet<ulong>();
            var queue = new PriorityQueue<ulong, int>();
            queue.Enqueue(maze, 0);
			var seenhits = 0;
            while (queue.TryDequeue(out var state, out var energy))
            {
                if (seen.Contains(state))
                {
					seenhits++;
                    continue;
                }
                seen.Add(state);

                if (seen.Count % 100000 == 0)
                    Console.WriteLine($"{seen.Count}: {energy}");

                //Console.WriteLine(state.Key);
                //state.Map.ConsoleWrite();
                //Console.WriteLine();

                if (state == FinalState)
                {
                    return energy;
                }
                foreach (var (s,e) in NextMoves(state).Where(x => !seen.Contains(x.Item1)))
                {
                    queue.Enqueue(s, energy + e);
                }
            }

            throw new Exception();
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}
	}
}
