using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day21
{
	internal static class Puzzle
	{
		public static void Run()
		{
			//Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var solution = @"
				not a t
				not t t
				and b t
				and c t
				not d j
				or j t
				not t j
				walk
			";

			while (true)
			{
				var game = new Game()
					.WithController(UserPaddleControl)
					.Run();
			}


			//var shortestPath = 0;// ShortestPath(maze);
			//Console.WriteLine($"Day 20 Puzzle 1: {shortestPath}");
//			Debug.Assert(damage = 19354083);

			string UserPaddleControl(Game g)
			{
				return Console.ReadLine();
			}

		}

		private static void Puzzle2()
		{
			var solution = @"
				not a t
				not t t
				and b t
				and c t
				not d j
				or j t
				not t j
				run
			";

			while (true)
			{
				var game = new Game()
					.WithController(UserPaddleControl)
					.Run();
			}


			//var shortestPath = 0;// ShortestPath(maze);
			//Console.WriteLine($"Day 20 Puzzle 1: {shortestPath}");
			//			Debug.Assert(damage = 19354083);

			string UserPaddleControl(Game g)
			{
				return Console.ReadLine();
			}

			//			var allowed = new string('?', 9);
			// Rules:
			//   ABCDEFGHI
			//        .
			//   xxxxx1000
			//  ???---J--->
			//            
			//   aaa1xxxxx, a is not all 1s
			//  J???>?????         
			//            
			//    .   .
			//   ?1xx01000
			//  - J   J
			//
			//    .   .
			//   ?1x0?1000
			//  -J    J
			//
			//   0xxxxxxxx
			//  J
			//
			// In summary:
			//
			// Jump if A=0 or (D=1 AND ABC!=111 AND EFGHI!=01000)
			// Jump if ABC!=111 AND D=1 AND EFGHI!=0?000
			// Jump if ABC!=111 AND D=1 AND EGH!=000 AND GHI!=000


			// Jump if ABC!=1111 AND E=1 AND EFGHI!=0?000
			// Jump if ABC!=111 AND E=1 AND FHI!=000 AND GHI!=000


			//  ABCDEFGHI
			// J0???1????
			// J?0??10???
			// J??0?1?0??
			// J???01??0?
			// 
			// J0??11????
			// J?0?110???
			// J??011?0??
			// J???11??0?

			// J0????????
			// J?0?10????
			// J??01?0???
			// J???1?00??
			//  ABCDEFGHI

			// J0???????? ??
			// J?0?10????
			// J??01?0???
			// J???1?00??
			//  ABCDEFGHI


			// !B AND !F  ==  !(B OR F)

			// J if E=1 AND (A=0 OR BF=00 || CG=00 || DH==0)

			// J if E AND (!A OR (!B && !F) OR (!C && !G) OR (!D && !H))

			// J if E AND (!A OR !(B OR F) OR !(C OR G) OR !(D OR H))

			// J if E AND !(A AND !(!B && !F) AND !(!C && !G) AND !(!D && !H))
			// J if E AND !(A AND (B OR F) AND (C OR G) AND (D OR H))
			// J if E AND !(A AND (B OR F) AND (C OR G) AND (D OR H))

			// NOW
			// J if D AND E AND (!A OR (!B AND !F) OR (!C AND !G) OR !H)
			// J IF D AND (!A OR (!B AND !E) OR (!C AND !F) OR !G 


#if false

(B OR F) AND (C OR G) AND (D OR H)
			= B AND ((C OR G) AND (D OR H)) OR F AND ((C OR G) AND (D OR H)) 
			= B AND (!(!C AND !G) AND !(!D AND !H)) OR F AND (!(!C AND !G) AND !(!D AND !H))
			= B AND !((!C AND !G) OR (!D AND !H)) OR F AND !((!C AND !G) OR (!D AND !H))

			var solution = @"


or b t
or e t
or h t
not t j    ---
not c t
not t t
or f t
not t t
and t j    ---
not g t
or t j   ----
not a t
or t j
and d j
run







//X\Y 0 1
//0   1 0
//1   0 0

//!X && !Y => t=1

//not x t
//not t t    // t = 0
//or y t     // t = 0
//not t t    // t = 1

//!(!(!X) OR Y) = (!X AND !Y)

//not a j
//not b t




not c t
not t t
or g t
and 

not t j    // t = 1
not c t
not t t    // t = 0
or g t     // t = 0
not t j    // t = 1





				not a t
				not t t
				and b t
				and c t  // 111 = >t=1
				not t t  // 111 = >t=0
                and d t
                not e j // j=e
				or g j
				or h j
				and j t
                not g j // j=e
				or h j
				or i j
				and t j
				run
			";

#endif
		}

		internal class Game
		{
			private readonly Engine _engine;
			private Func<Game, string> _controller;

			private Queue<char> _input = new Queue<char>();

			public Game()
			{
				_engine = new Engine()
					.WithMemoryFromFile("Day21/input.txt")
					.OnOutput(engine =>
					{
						var result = engine.Output.TakeAll().ToList();
						if (result.Count() == 1 && result.First() > 255)
						{
							Console.WriteLine("######\n###### " + result.First());
						}
						else
						{
							foreach (var ch in result)
							{
								Console.Write((char)ch);
							}
						}
					})
					.OnInput(engine =>
					{
						if (_input.Count() == 0)
						{
							var instruction = _controller(this).Trim().ToUpper() + '\x0a';
							foreach (var ch in instruction.ToArray())
							{
								_input.Enqueue(ch);
							}
						}
						engine.WithInput(_input.Dequeue());
					});
			}

			public Game WithController(Func<Game, string> controller)
			{
				_controller = controller;
				return this;
			}

			public Game Run()
			{
				_engine.Execute();
				return this;
			}
		}


	}

}

