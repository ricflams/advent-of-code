using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace AdventOfCode.Y2024.Day09.Wip
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Disk Fragmenter";
		public override int Year => 2024;
		public override int Day => 9;

		public override void Run()
		{
			Run("test1").Part1(1928).Part2(2858);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(6415184586041).Part2(6436819084274);
			//Run("extra").Part1(0).Part2(0);
		}


		[DebuggerDisplay("{ToString()}")]
		class Block
		{
			public int? Id;
			public int Length;
			public bool IsFree => Id == null;
			public override string ToString()
			{
				return Id.HasValue ? $"{Length} '{Id}'" : $"{Length} '.'";
			}
			public string Format => new string(Id.HasValue ? Id.ToString()[0] : '.', Length);
		}

		protected override long Part1(string[] input)
		{
			var raw = input[0].ToCharArray().Select(ch => ch - '0').ToArray();

			var blocks = new LinkedList<Block>();
			for (var i = 0; i < raw.Length; i++)
			{
				if (i % 2 == 0)
					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = i / 2, Length = raw[i] }));
				else
					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = null, Length = raw[i] }));
			}

			// // compact
			// var freeblock = blocks.First(b => b.Id == null);
			// foreach (var b in blocks.Reverse())
			// {
			// 	blocks.Remove(b);
			// 	while (b.Length > 0)
			// 	{
			// 		if (b.Length > freeblock.Length)
			// 		{
			// 			// fill free block entirely
			// 			freeblock.Id = 	b.Id;
			// 			b.Length -= freeblock.Length;
			// 			freeblock = blocks.First(b => b.Id == null); // a bit brute
			// 		}
			// 		else
			// 		{
			// 			// split free block
			// 			freeblock.Id = 	b.Id;
			// 			var freeblockpos = blocks.Find(freeblock);
			// 			freeblock = new Block { Id = null, Length = freeblock.Length - b.Length };
			// 			blocks.AddAfter(freeblockpos, freeblock);
			// 			b.Length = 0;
			// 		}
			// 	}
			// }



			// compact
			//var freeblock = blocks.First(b => b.Id == null);
			while (true)
			{
				while (blocks.Last()?.Id == null)
					blocks.RemoveLast();
				var freeblocknode = blocks.FirstNodeOrDefault(b => !b.Id.HasValue);
				if (freeblocknode == null)
					break;
				var freeblock = freeblocknode.Value;
				var last = blocks.LastOrDefault(b => b.Id.HasValue);
				if (last.Length <= freeblock.Length)
				{
					if (last.Length < freeblock.Length)
					{
						var newfreeblock = new Block { Id = null, Length = freeblock.Length - last.Length };
						blocks.AddAfter(blocks.Find(freeblock), newfreeblock);
					}
					freeblock.Id = last.Id;
					freeblock.Length = last.Length;
					blocks.Remove(last);
				}
				else
				{
					freeblock.Id = last.Id;
					last.Length -= freeblock.Length;
				}
			}

			while (true)
			{
				var lasts = blocks.TakeLast(2).ToArray();
				if (lasts[0].Id == lasts[1].Id)
				{
					var len2 = lasts[0].Length + lasts[1].Length;
					blocks.RemoveLast();
					blocks.Last().Length = len2;
					continue;
				}
				break;
			}


			var checksum = 0L;
			var pos = 0L;
			foreach (var b in blocks)
			{
				for (var i = 0; i < b.Length; i++)
				{
					checksum += pos++ * b.Id.Value;
				}
			}

			return checksum;
		}

		protected override long Part2(string[] input)
		{
			var raw = input[0].ToCharArray().Select(ch => ch - '0').ToArray();

			var blocks = new LinkedList<Block>();
			for (var i = 0; i < raw.Length; i++)
			{
				if (i % 2 == 0)
					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = i / 2, Length = raw[i] }));
				else
					blocks.AddLast(new LinkedListNode<Block>(new Block { Id = null, Length = raw[i] }));
			}

			//			var id = blocks.Count / 2 + 1;
			var block = blocks.Last;
			var free = blocks.First;

			// 			foreach (var b in blocks)
			// 				Console.Write(b.Format);
			// 			Console.WriteLine();
			// ;			

			for (var id = blocks.Count / 2; id >= 0; id--)
			{
				;
				// Find next block to move and first free spot
				while (block.Value.Id != id)
				{
					block = block.Previous;
					if (free == block)
						break;
				}
				while (free.Value.Id != null)
				{
					free = free.Next;
					if (free == block)
						break;
				}

				// Find first spot with enough space, if any
				var f = free;
				while (f != null && f != block && !(f.Value.IsFree && block.Value.Length <= f.Value.Length))
				{
					f = f.Next;
				}
				if (f == null || f == block) // no free blocks
					continue;

				var prev = block.Previous;
				blocks.Remove(block);
				blocks.AddBefore(f, block);

				var vacancy = f.Value;

				if (block.Value.Length < f.Value.Length)
				{
					f.Value.Length -= block.Value.Length;
				}
				else
				{
					blocks.Remove(f);
					if (f.Previous?.Value.Id == 0)
					{
						f.Value.Length += f.Previous.Value.Length;
						blocks.Remove(f.Previous);
					}
					if (f.Next?.Value.Id == 0)
					{
						f.Value.Length += f.Next.Value.Length;
						blocks.Remove(f.Next);
					}
				}

				// foreach (var b in blocks)
				// 	Console.Write(b.Format);
				// Console.WriteLine();

				block = prev;
				;
			}

			while (true)
			{
				var lasts = blocks.TakeLast(2).ToArray();
				if (lasts[0].Id == lasts[1].Id)
				{
					var len2 = lasts[0].Length + lasts[1].Length;
					blocks.RemoveLast();
					blocks.Last().Length = len2;
					continue;
				}
				break;
			}


			var checksum = 0L;
			var pos = 0L;
			foreach (var b in blocks)
			{
				for (var i = 0; i < b.Length; i++)
				{
					checksum += pos++ * (b.Id ?? 0);
				}
			}

			return checksum;
		}
	}


	public static class Extension
	{
		public static LinkedListNode<T> FirstNodeOrDefault<T>(this LinkedList<T> list, Func<T, bool> predicate)
		{
			for (var n = list.First; n != null; n = n.Next)
			{
				if (predicate(n.Value))
					return n;
			}
			return null;
		}

		public static LinkedListNode<T> FirstNode<T>(this LinkedList<T> list, Func<T, bool> predicate)
		{
			return FirstNodeOrDefault(list, predicate) ?? throw new Exception();
		}

		// public static IEnumerable<LinkedListNode<T>> TakeNodeWhile<T>(this LinkedList<T> list, Func<T, bool> predicate)
		// {
		// 	for (var n = list.First; n != null; n = n.Next)
		// 	{
		// 		if (predicate(n.Value))
		// 			yield break;
		// 		yield return n;
		// 	}
		// }		
	}
}
