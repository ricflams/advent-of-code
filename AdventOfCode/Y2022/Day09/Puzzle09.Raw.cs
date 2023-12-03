using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day09.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 9";
		public override int Year => 2022;
		public override int Day => 9;

		public override void Run()
		{
			Run("test1").Part1(13).Part2(1);
			Run("test2").Part2(36);
			Run("input").Part1(5981).Part2(2352);
		}

		protected override long Part1(string[] input)
		{
			var map = new CharMap();
			var head = Point.Origin;
			var tail = Point.Origin;

			// L 3
			// U 5
			// R 2
			// U 13			
			map[tail] = '#';
			foreach (var s in input)
			{
				var (dir0, n) = s.RxMatch("%c %d").Get<char, int>();
				for (var i = 0; i < n; i++)
				{
					var dir = dir0;
					head = Move(dir, head);

					tail = Pull3(dir, head, tail);
					//Pull(ref dir, head, ref tail);
			//		Console.WriteLine(tail);
					map[tail] = '#';
				}
				// Console.WriteLine();
				// map.ConsoleWrite();
			}

			// 6321 not right
			// 1137 not right

			return map.Count('#');
		}

		private Point Move(char dir, Point p)
		{
			return dir switch
			{
				'R' => p.Right,
				'L' => p.Left,
				'U' => p.Up,
				'D' => p.Down,
				_ => throw new Exception()
			};
		}

		private Point Pull3(char dir, Point head, Point tail)
		{
			// var len = 2;
			var dx = head.X - tail.X;
			var dy = head.Y - tail.Y;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			if (adx == 2 && ady == 0)
			{
				return Point.From(tail.X + (dx > 0 ? 1 : -1), tail.Y);
			}
			if (ady == 2 && adx == 0)
			{
				return Point.From(tail.X, tail.Y + (dy > 0 ? 1 : -1));
			}
			if (adx <= 1 && ady <= 1)
				return tail;
			
			if (dx > 0 && dy > 0)
				return tail.DiagonalDownRight;
			if (dx > 0 && dy < 0)
				return tail.DiagonalUpRight;
			if (dx < 0 && dy < 0)
				return tail.DiagonalUpLeft;
			if (dx < 0 && dy > 0)
				return tail.DiagonalDownLeft;

			return tail;
		}

		// private void Pull(ref char dir, Point head, ref Point tail)
		// {
		// 	// var len = 2;
		// 	var dx = head.X - tail.X;
		// 	var dy = head.Y - tail.Y;

		// 	var adx = Math.Abs(dx);
		// 	var ady = Math.Abs(dy);
			
		// 	if (adx < 2 && ady < 2)
		// 		return;
			
		// 	var (x, y) = (tail.X, tail.Y);

		// 	switch (dir)
		// 	{
		// 		case 'U':
		// 			if (ady > 1)
		// 			{
		// 				x = head.X;
		// 				y = head.Y + 1;
		// 			}
		// 			else if (adx > 1)
		// 			{
		// 				y = head.Y;
		// 				x += dx > 0 ? 1 : -1;
		// 			}
		// 			break;
		// 		case 'D':
		// 			if (ady > 1)
		// 			{
		// 				x = head.X;
		// 				y = head.Y - 1;
		// 			}
		// 			else if (adx > 1)
		// 			{
		// 				y = head.Y;
		// 				x += dx > 0 ? 1 : -1;
		// 			}

		// 			// if (dy > 1)
		// 			// {
		// 			// 	y++;
		// 			// 	if (adx >= 1)
		// 			// 	{
		// 			// 		x += dx > 0 ? 1 : -1;
		// 			// 		//dir = dx > 0 ? 'R' : 'L';
		// 			// 	}					
		// 			// }
		// 			break;
		// 		case 'L':
		// 			if (adx > 1)
		// 			{
		// 				x = head.X + 1;
		// 				y = head.Y;
		// 			}
		// 			else if (ady > 1)
		// 			{
		// 				x = head.X;
		// 				y += dy > 0 ? 1 : -1;
		// 			}
		// 			// if (dx < -1)
		// 			// {
		// 			// 	x--;
		// 			// 	if (ady >= 1)
		// 			// 	{
		// 			// 		y += dy > 0 ? 1 : -1;
		// 			// 		//dir = dy > 0 ? 'D' : 'U';
		// 			// 	}
		// 			// }
		// 			break;
		// 		case 'R':
		// 			if (adx > 1)
		// 			{
		// 				x = head.X - 1;
		// 				y = head.Y;
		// 			}
		// 			else if (ady > 1)
		// 			{
		// 				x = head.X;
		// 				y += dy > 0 ? 1 : -1;
		// 			}
		// 			// if (dx > 1)
		// 			// {
		// 			// 	x++;
		// 			// 	if (ady >= 1)
		// 			// 	{
		// 			// 		y += dy > 0 ? 1 : -1;
		// 			// 		//dir = dy > 0 ? 'D' : 'U';
		// 			// 	}
		// 			// }
		// 			break;
		// 	}
		// 	tail = Point.From(x, y);

		// }


		// private bool OldPull(ref char dir, Point head, ref Point tail)
		// {
		// 	var len = 2;
		// 	var dx = Math.Abs(head.X - tail.X);
		// 	var dy = Math.Abs(head.Y - tail.Y);
			
		// 	if (dx == 0)
		// 	{
		// 		if (dy >= len)
		// 		{
		// 			tail = dir switch
		// 			{
		// 				'R' => tail.Right,
		// 				'L' => tail.Left,
		// 				'U' => tail.Up,
		// 				'D' => tail.Down,
		// 				_ => throw new Exception()
		// 			};
		// 			return true;
		// 		}
		// 	}
		// 	else if (dy == 0)
		// 	{
		// 		if (dx >= len)
		// 		{
		// 			tail = dir switch
		// 			{
		// 				'R' => tail.Right,
		// 				'L' => tail.Left,
		// 				'U' => tail.Up,
		// 				'D' => tail.Down,
		// 				_ => throw new Exception()
		// 			};
		// 			return true;
		// 		}
		// 	}
		// 	else if (dx > 1)
		// 	{
		// 		tail = dir switch
		// 		{
		// 			'R' => tail.Right,
		// 			'L' => tail.Left,
		// 			'U' => tail.Up,
		// 			'D' => tail.Down,
		// 			_ => throw new Exception()
		// 		};
		// 		if (tail.Y > head.Y)
		// 		{
		// 			tail = Point.From(tail.X, head.Y);
		// 			dir = 'U';
		// 		}
		// 		else if (tail.Y < head.Y)
		// 		{
		// 			tail = Point.From(tail.X, head.Y);
		// 			dir = 'D';
		// 		}
		// 		return true;
		// 	}
		// 	else if (dy > 1)
		// 	{
		// 		tail = dir switch
		// 		{
		// 			'R' => tail.Right,
		// 			'L' => tail.Left,
		// 			'U' => tail.Up,
		// 			'D' => tail.Down,
		// 			_ => throw new Exception()
		// 		};
		// 		if (tail.X > head.X)
		// 		{
		// 			tail = Point.From(head.X, tail.Y);
		// 			dir = 'L';
		// 		}
		// 		else if (tail.X < head.X)
		// 		{
		// 			tail = Point.From(head.X, tail.Y);
		// 			dir = 'R';
		// 		}
		// 		return true;
		// 	}
		// 	return false;
		// }

		protected override long Part2(string[] input)
		{
			var map = new CharMap();
			var len = 10;
			var rope = new Point[len];
			for (var i = 0; i < len; i++)
				rope[i] = Point.Origin;
			// var head = Point.Origin;
			// var tail = Point.Origin;

			// L 3
			// U 5
			// R 2
			// U 13			
			map[Point.Origin] = '#';

			foreach (var s in input)
			{
				var (dir0, n) = s.RxMatch("%c %d").Get<char, int>();
				for (var i = 0; i < n; i++)
				{
					var dir = dir0;
					rope[0] = Move(dir, rope[0]);
					for (var j = 0; j < len-1; j++)
					{
						var headi = rope[j];
						var taili = rope[j+1];
						taili = Pull3(dir, headi, taili);
						if (rope[j+1] == taili)
							break;
						rope[j+1] = taili;
					}
					var tail = rope[len-1];
					map[tail] = '#';


					// foreach (var p in rope)
					// {
					// 	if (map[p] != '#')
					// 		map[p] = '.';
					// }

					// Console.WriteLine();
					// Console.WriteLine(s);
					// foreach (var ss in map.Render((p, ch) =>
					// {
					// 	for (var i = 0; i < len; i++)
					// 	{
					// 		if (rope[i] == p)
					// 			return i == 0 ? 'H' : (char)('0' + i);
					// 	}
					// 	if (p == Point.Origin)
					// 		return 's';
					// 	return '.';
					// }))
					// {
					// 	Console.WriteLine(ss);
					// }

				}

				// foreach (var p in rope)
				// {
				// 	if (map[p] != '#')
				// 		map[p] = '.';
				// }

				// Console.WriteLine();
				// Console.WriteLine(s);
				// foreach (var ss in map.Render((p, ch) =>
				// {
				// 	for (var i = 0; i < len; i++)
				// 	{
				// 		if (rope[i] == p)
				// 			return i == 0 ? 'H' : (char)('0' + i);
				// 	}
				// 	if (p == Point.Origin)
				// 		return 's';
				// 	return '.';
				// }))
				// {
				// 	Console.WriteLine(ss);
				// }
			}

			// 4413 not right

			// 6321 not right

			return map.Count('#');
		}



	}
}
