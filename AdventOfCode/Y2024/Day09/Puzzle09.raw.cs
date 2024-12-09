// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.IO;
// using System.Text;
// using AdventOfCode.Helpers;
// using AdventOfCode.Helpers.Puzzles;
// using AdventOfCode.Helpers.String;
// using System.Diagnostics.CodeAnalysis;
// using System.Threading;

// namespace AdventOfCode.Y2024.Day09.Raw
// {
// 	internal class Puzzle : Puzzle<long, long>
// 	{
// 		public static Puzzle Instance = new();
// 		public override string Name => "Disk Fragmenter";
// 		public override int Year => 2024;
// 		public override int Day => 9;

// 		public override void Run()
// 		{
// 			Run("test1").Part1(1928).Part2(2858);
// 			//Run("test2").Part1(0).Part2(0);
// 			Run("input").Part1(6415184586041).Part2(6436819084274);
// 			//Run("extra").Part1(0).Part2(0);
// 		}


// 		[DebuggerDisplay("{ToString()}")]
// 		class Block
// 		{
// 			public int? Id;
// 			public int Length;
// 			public override string ToString()
// 			{
// 				return Id.HasValue ? $"{Length} '{Id}'" : $"{Length} '.'";
// 			}
// 			public string Format => new string(Id.HasValue ? Id.ToString()[0] : '.', Length);
// 		}

// 		protected override long Part1(string[] input)
// 		{
// 			var raw = input[0].ToCharArray().Select(ch => ch - '0').ToArray();
// 			var len = raw.Sum();
// 			Console.WriteLine(len);

// 			var blocks = new LinkedList<Block>();
// 			for (var i = 0; i < raw.Length; i++)
// 			{
// 				if (i % 2 == 0)
// 					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = i/2, Length = raw[i] }));
// 				else
// 					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = null, Length = raw[i] }));
// 			}

// 			// // compact
// 			// var freeblock = blocks.First(b => b.Id == null);
// 			// foreach (var b in blocks.Reverse())
// 			// {
// 			// 	blocks.Remove(b);
// 			// 	while (b.Length > 0)
// 			// 	{
// 			// 		if (b.Length > freeblock.Length)
// 			// 		{
// 			// 			// fill free block entirely
// 			// 			freeblock.Id = 	b.Id;
// 			// 			b.Length -= freeblock.Length;
// 			// 			freeblock = blocks.First(b => b.Id == null); // a bit brute
// 			// 		}
// 			// 		else
// 			// 		{
// 			// 			// split free block
// 			// 			freeblock.Id = 	b.Id;
// 			// 			var freeblockpos = blocks.Find(freeblock);
// 			// 			freeblock = new Block { Id = null, Length = freeblock.Length - b.Length };
// 			// 			blocks.AddAfter(freeblockpos, freeblock);
// 			// 			b.Length = 0;
// 			// 		}
// 			// 	}
// 			// }



// 			// compact
// 			//var freeblock = blocks.First(b => b.Id == null);
// 			while (true)
// 			{
// 				while (blocks.Last()?.Id == null)
// 					blocks.RemoveLast();
// 				var freeblocknode = blocks.FirstOrDefault(b => !b.Id.HasValue);
// 				if (freeblocknode == null)
// 					break;
// 				var freeblock = freeblocknode.Value;
// 				var last = blocks.LastOrDefault(b => b.Id.HasValue);
// 				if (last.Length <= freeblock.Length)
// 				{
// 					if (last.Length < freeblock.Length)
// 					{
// 						var newfreeblock = new Block { Id = null, Length = freeblock.Length - last.Length };
// 						blocks.AddAfter(blocks.Find(freeblock), newfreeblock);
// 					}
// 					freeblock.Id = last.Id;
// 					freeblock.Length = last.Length;
// 					blocks.Remove(last);
// 				}
// 				else
// 				{
// 					freeblock.Id = last.Id;
// 					last.Length -= freeblock.Length;
// 				}
// 			}

// 			while (true)
// 			{
// 				var lasts = blocks.TakeLast(2).ToArray();
// 				if (lasts[0].Id == lasts[1].Id)
// 				{
// 					var len2 = lasts[0].Length + lasts[1].Length;
// 					blocks.RemoveLast();
// 					blocks.Last().Length = len2;
// 					continue;
// 				}
// 				break;
// 			}


// 			var checksum = 0L;
// 			var pos = 0L;
// 			foreach (var b in blocks)
// 			{
// 				for (var i = 0; i < b.Length; i++)
// 				{
// 					checksum += pos++ * b.Id.Value;
// 				}
// 			}

// 			return checksum;
// 		}

// 		protected override long Part2(string[] input)
// 		{
// 			var raw = input[0].ToCharArray().Select(ch => ch - '0').ToArray();
// 			var len = raw.Sum();
// 			Console.WriteLine(len);

// 			var blocks = new LinkedList<Block>();
// 			for (var i = 0; i < raw.Length; i++)
// 			{
// 				if (i % 2 == 0)
// 					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = i/2, Length = raw[i] }));
// 				else
// 					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = null, Length = raw[i] }));
// 			}

// 			// // compact
// 			// var freeblock = blocks.First(b => b.Id == null);
// 			// foreach (var b in blocks.Reverse())
// 			// {
// 			// 	blocks.Remove(b);
// 			// 	while (b.Length > 0)
// 			// 	{
// 			// 		if (b.Length > freeblock.Length)
// 			// 		{
// 			// 			// fill free block entirely
// 			// 			freeblock.Id = 	b.Id;
// 			// 			b.Length -= freeblock.Length;
// 			// 			freeblock = blocks.First(b => b.Id == null); // a bit brute
// 			// 		}
// 			// 		else
// 			// 		{
// 			// 			// split free block
// 			// 			freeblock.Id = 	b.Id;
// 			// 			var freeblockpos = blocks.Find(freeblock);
// 			// 			freeblock = new Block { Id = null, Length = freeblock.Length - b.Length };
// 			// 			blocks.AddAfter(freeblockpos, freeblock);
// 			// 			b.Length = 0;
// 			// 		}
// 			// 	}
// 			// }



// 			// compact
// 			//var freeblock = blocks.First(b => b.Id == null);
// 			var id = blocks.Last(b => b.Id.HasValue).Id;

// 			while (true)
// 			{
				
// 				// foreach (var b in blocks)
// 				// 	Console.Write(b.Format);
// 				// Console.WriteLine();



// 				var blocknode = blocks.FirstOrDefault(b => b.Id == id);
// 				if (blocknode == null)
// 					break;
// 				var block = blocknode.Value;
// 				id--;

// 				var freeblocknode = blocks.TakeWhile(b => b != block).FirstOrDefault(b => !b.Id.HasValue && block.Length <= b.Length);
// 				if (freeblocknode == null)
// 					continue;
// 				var freeblock = freeblocknode.va

// 				//var last = blocks.LastOrDefault(b => b.Id.HasValue);
// 				if (block.Length < freeblock.Length)
// 				{
// 					var newfreeblock = new Block { Id = null, Length = freeblock.Length - block.Length };
// 					blocks.AddAfter(blocks.Find(freeblock), newfreeblock);
// 				}
// 				freeblock.Id = block.Id;
// 				freeblock.Length = block.Length;
// 				block.Id = null;
// 				var next = blocks.Find(block)?.Next;
// 				if (next != null && !next.Value.Id.HasValue)
// 				{
// 					block.Length += next.Value.Length;
// 					blocks.Remove(next);
// 				}
// 				var prev = blocks.Find(block)?.Previous;
// 				if (prev != null && !prev.Value.Id.HasValue)
// 				{
// 					block.Length += prev.Value.Length;
// 					blocks.Remove(prev);
// 				}				
// //				blocks.Remove(block);
// 			}

// 			while (true)
// 			{
// 				var lasts = blocks.TakeLast(2).ToArray();
// 				if (lasts[0].Id == lasts[1].Id)
// 				{
// 					var len2 = lasts[0].Length + lasts[1].Length;
// 					blocks.RemoveLast();
// 					blocks.Last().Length = len2;
// 					continue;
// 				}
// 				break;
// 			}


// 			var checksum = 0L;
// 			var pos = 0L;
// 			foreach (var b in blocks)
// 			{
// 				for (var i = 0; i < b.Length; i++)
// 				{
// 					checksum += pos++ * (b.Id ?? 0);
// 				}
// 			}

// 			return checksum;
// 		}
// 	}
// }
