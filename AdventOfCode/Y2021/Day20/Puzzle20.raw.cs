using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day20.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 20";
		public override int Year => 2021;
		public override int Day => 20;

		public void Run()
		{
			Run("test1").Part1(35).Part2(3351);

			//Run("test2").Part1(0).Part2(0);
			//// 5607 too high
			//// 5328 too low
			//// 5432 not right

			Run("input").Part1(5229).Part2(17009);
			// 5402 too high
		}

		protected override long Part1(string[] input)
		{
			var algo = input[0];//.Select(x => x == '#').ToArray();

			var image = CharMatrix.FromArray(input.Skip(2).ToArray());

			//Console.WriteLine();
			//image.ConsoleWrite();

			for (var i = 0; i < 2; i++)
			{

				//Console.WriteLine("expanded");
				//image.ConsoleWrite();
				//Console.WriteLine();

				var dotorg = algo[0] == '.' ? '.' : i % 2 == 0 ? '.' : '#';
				var dotexp = algo[0] == '.' ? '.' : i % 2 == 1 ? '.' : '#';
				image = image.ExpandBy(2, dotorg);

				var (w, h) = image.Dim();

				//	var image2 = CharMatrix.Create(w, h, '.');
				var image2 = CharMatrix.Create(w, h, dotexp);

				for (var x = 1; x < w - 1; x++)
				{
					for (var y = 1; y < h - 1; y++)
					{
						var idx = 0;
						if (image[x + 1, y + 1] == '#') idx += 1;
						if (image[x, y + 1] == '#') idx += 2;
						if (image[x - 1, y + 1] == '#') idx += 4;
						if (image[x + 1, y] == '#') idx += 8;
						if (image[x, y] == '#') idx += 16;
						if (image[x - 1, y] == '#') idx += 32;
						if (image[x + 1, y - 1] == '#') idx += 64;
						if (image[x, y - 1] == '#') idx += 128;
						if (image[x - 1, y - 1] == '#') idx += 256;
						var pixel = algo[idx];
						image2[x, y] = pixel;
					}
				}

				image = image2;
				//Console.WriteLine();
				//image.ConsoleWrite();
			}

			//image.ConsoleWrite();
		//	var (w2, h2) = image.Dim();
		//	image = image.CopyPart(1, 1, w2 - 2, h2 - 2);

			var n = image.CountChar('#');
			return n;


			//var algo = input[0];//.Select(x => x == '#').ToArray();

			//var image = CharMap.FromArray(input.Skip(2).ToArray());
			//var (w, h) = image.Size();

			//Console.WriteLine();
			//image.ConsoleWrite();

			//var expand = 0;
			//for (var i = 0; i < 1; i++)
			//{
			//	//Console.WriteLine("expanded");
			//	//image.ConsoleWrite();
			//	//Console.WriteLine();

			//	expand += 1;
			//	var image2 = new CharMap('.');
			//	for (var x = -expand; x < w+ expand; x++)
			//	{
			//		for (var y = -expand; y < h+ expand; y++)
			//		{
			//			var idx = 0;
			//			if (image[x + 1][y + 1] == '#') idx += 1;
			//			if (image[x][y + 1] == '#') idx += 2;
			//			if (image[x - 1][y + 1] == '#') idx += 4;
			//			if (image[x + 1][y] == '#') idx += 8;
			//			if (image[x][y] == '#') idx += 16;
			//			if (image[x - 1][y] == '#') idx += 32;
			//			if (image[x + 1][y - 1] == '#') idx += 64;
			//			if (image[x][y - 1] == '#') idx += 128;
			//			if (image[x - 1][y - 1] == '#') idx += 256;
			//			var pixel = algo[idx];
			//			image2[x][y] = pixel;
			//		}
			//	}
			//	image = image2;
			//	Console.WriteLine();
			//	image.ConsoleWrite();
			//}

			//var n = image.Count('#');
			//return n;

		}

		protected override long Part2(string[] input)
		{
			return LitPixelsAfterEnhancement(input, 50);
		}

		private int LitPixelsAfterEnhancement(string[] input, int n)
		{
			var mapping = input[0].ToCharArray();
			var image = CharMatrix.FromArray(input.Skip(2).ToArray());
			return LitPixelsAfterEnhancement(mapping, image, n);
		}

		private int LitPixelsAfterEnhancement(char[] mapping, char[,] image, int n)
		{
			//Console.WriteLine();
			//image.ConsoleWrite();

			for (var i = 0; i < n; i++)
			{

				//Console.WriteLine("expanded");
				//image.ConsoleWrite();
				//Console.WriteLine();

				var dotorg = mapping[0] == '.' ? '.' : i % 2 == 0 ? '.' : '#';
				var dotexp = mapping[0] == '.' ? '.' : i % 2 == 1 ? '.' : '#';
				image = image.ExpandBy(2, dotorg);

				var (w, h) = image.Dim();

				//	var image2 = CharMatrix.Create(w, h, '.');
				var image2 = CharMatrix.Create(w, h, dotexp);

				for (var x = 1; x < w - 1; x++)
				{
					for (var y = 1; y < h - 1; y++)
					{
						var idx = 0;
						if (image[x + 1, y + 1] == '#') idx += 1;
						if (image[x, y + 1] == '#') idx += 2;
						if (image[x - 1, y + 1] == '#') idx += 4;
						if (image[x + 1, y] == '#') idx += 8;
						if (image[x, y] == '#') idx += 16;
						if (image[x - 1, y] == '#') idx += 32;
						if (image[x + 1, y - 1] == '#') idx += 64;
						if (image[x, y - 1] == '#') idx += 128;
						if (image[x - 1, y - 1] == '#') idx += 256;
						var pixel = mapping[idx];
						image2[x, y] = pixel;
					}
				}

				image = image2;
				//Console.WriteLine();
				//image.ConsoleWrite();
			}

			//image.ConsoleWrite();
			//	var (w2, h2) = image.Dim();
			//	image = image.CopyPart(1, 1, w2 - 2, h2 - 2);

			return image.CountChar('#');
		}


		//for (var x = )
		//image = image.Transform((ch, adjacents) =>
		//{
		//	var n = 0;
		//	foreach (var c in adjacents)
		//	{
		//		if (c == '|' && ++n >= 3)
		//			return '|';
		//	}
		//	return ch;
		//});

		//var image2 =  CharMatrix();
		//foreach (var p in image.AllPoints(ch => ch == '#'))
		//{
		//	var idx = 0;
		//	if (image[p.DiagonalDownRight] == '#') idx += 1;
		//	if (image[p.Down] == '#') idx += 2;
		//	if (image[p.DiagonalDownLeft] == '#') idx += 4;
		//	if (image[p.Right] == '#') idx += 8;
		//	if (image[p] == '#') idx += 16;
		//	if (image[p.Left] == '#') idx += 32;
		//	if (image[p.DiagonalUpRight] == '#') idx += 64;
		//	if (image[p.Up] == '#') idx += 128;
		//	if (image[p.DiagonalUpLeft] == '#') idx += 256;
		//	image2[p] = algo[idx];

		//}
		//image = image.ex
		//image = image.Transform((p, ch) =>
		//{
		//	var idx = 0;
		//	if (image[p.DiagonalDownRight] == '#') idx += 1;
		//	if (image[p.Down] == '#') idx += 2;
		//	if (image[p.DiagonalDownLeft] == '#') idx += 4;
		//	if (image[p.Right] == '#') idx += 8;
		//	if (image[p] == '#') idx += 16;
		//	if (image[p.Left] == '#') idx += 32;
		//	if (image[p.DiagonalUpRight] == '#') idx += 64;
		//	if (image[p.Up] == '#') idx += 128;
		//	if (image[p.DiagonalUpLeft] == '#') idx += 256;
		//	return algo[idx];
		//});


	}
}
