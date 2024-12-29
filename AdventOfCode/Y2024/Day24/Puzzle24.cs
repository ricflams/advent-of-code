using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day24
{
	internal class Puzzle : Puzzle<long, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "Crossed Wires";
		public override int Year => 2024;
		public override int Day => 24;

		public override void Run()
		{
			Run("test1").Part1(4);
			Run("test2").Part1(2024);
			Run("input").Part1(58639252480880).Part2("bkr,mqh,rnq,tfb,vvr,z08,z28,z39");
			Run("extra").Part1(49574189473968).Part2("ckb,kbs,ksv,nbd,tqq,z06,z20,z39");
		}

		protected override long Part1(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();

			var values = new Dictionary<string, bool>();
			foreach (var line in parts[0])
			{
				var (name, val) = line.RxMatch("%s: %d").Get<string, int>();
				values[name] = val == 1;
			}

			var pending = new SafeDictionary<(string A, string Op, string B), List<string>>(() => []);
			foreach (var line in parts[1])
			{
				var (a, op, b, o) = line.RxMatch("%s %s %s -> %s").Get<string, string, string, string>();
				pending[(a, op, b)].Add(o);
			}

			while (pending.Any())
			{
				var next = pending.First(x => values.ContainsKey(x.Key.A) && values.ContainsKey(x.Key.B));
				var (a, op, b) = next.Key;
				var bit = op switch
				{
					"AND" => values[a] && values[b],
					"OR" => values[a] || values[b],
					"XOR" => values[a] ^ values[b],
					_ => throw new Exception("Unknown operator")
				};
				foreach (var dest in next.Value)
					values[dest] = bit;
				pending.Remove(next.Key);
			}

			var sum = 0L;
			foreach (var w in values.Where(x => x.Key.StartsWith('z') && x.Value))
			{
				// Keys are named zNNN so skip 'z' to get the NNN-value, as Key[1..]
				var bit = int.Parse(w.Key[1..]);
				sum += 1L << bit;
			}

			return sum;
		}

		protected override string Part2(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();

			var bits = parts[0].Length / 2;
			var gates = parts[1].Select(Gate.Parse).ToArray();

			var swaps = new List<string>();
			void Swap(string a, string b)
			{
				var ga = gates.Single(g => g.Out == a);
				var gb = gates.Single(g => g.Out == b);
				(ga.Out, gb.Out) = (gb.Out, ga.Out);
				swaps.Add(a);
				swaps.Add(b);
				Debug.Assert(swaps.Count == swaps.Distinct().Count());
			}

			// Full-adder circuit looks like this:
			//
			//                                  | Carry in                                  
			//                                  |                                           
			//                                  |                                           
			//                                  |                  +-------+                
			//                +--------+        |------------------|       |                
			//   Xn  ---|-----|        |        |                  | Z XOR |---------- Zn   
			//          |     | XY XOR |-----|--|------------------|       |                
			//          | |---|        |     |  |                  +-------+                
			//          | |   +--------+     |  |   +-------+                               
			//          | |                  |  ----|       |                               
			//          | |                  |      | C AND |--|                            
			//          | |                  |------|       |  |                            
			//          | |                         +-------+  |   +-------+                
			//          | |   +--------+                       ----|       |                
			//          |-|---|        |                           | C OR  |-----|          
			//            |   | XY AND-----------------------------|       |     |          
			//   Yn  -----|---|        |                           +-------+     |          
			//                +--------+                                         |          
			//                                                                   | Carry out
			//
			// The gates have been given these shortcut names:
			//
			//   carry
			//   xyXor: XY XOR, the XOR of the X,Y input
			//   xyAnd: XY AND, the AND of the X,Y input
			//   zXor:   Z XOR, the XOR producing the Z output bit
			//   cAND:   C AND, the AND involved in the carry
			//   cOr:    C OR,  the OR (the only OR) involved in the carry

			var carry = gates.Single(g => g.HasInput("x00", "y00") && g.IsAnd).Out; // is maybe swapped
			for (var i = 1; i < bits; i++)
			{
				var id = i.ToString("D2");
				var (xwire, ywire, zwire) = ("x" + id, "y" + id, "z" + id);

				// The two x,y input gates XOR,AND are both certain
				var xyXor = gates.Single(g => g.HasInput(xwire, ywire) && g.IsXor);
				var xyAnd = gates.Single(g => g.HasInput(xwire, ywire) && g.IsAnd);

				// There's just one z XOR-gate that receives input from the xy XOR-gate
				// The carry or xy XOR's out may be swapped but let's assume that they're
				// not BOTH swapped with two other wires: if there's a gate receiving
				// both these inputs then assume it's the right one; else look for an
				// XOR gate producing the desired z-bit and assume that it will have
				// one of these inputs (ie that not both are swapped) and the swap the
				// other gate's output with this XOR-gates desired input.
				var zXor = gates.SingleOrDefault(g => g.HasInput(carry, xyXor.Out) && g.IsXor);
				if (zXor == null)
				{
					zXor = gates.SingleOrDefault(g => g.Out == zwire);
					if (zXor.HasInput(carry))
						Swap(xyXor.Out, zXor.OtherInput(carry));
					else if (zXor.HasInput(xyXor.Out))
					{
						// swap carry
						var actual = zXor.OtherInput(xyXor.Out);
						Swap(carry, actual);
						carry = actual;
					}
					else throw new Exception("both input are swapped");
				}

				// If the Z XOR is producint the wrong output bit then swap its output
				// with the XOR-gate that does produce the right bit.
				if (zXor.Out != zwire)
				{
					Swap(zXor.Out, zwire);
				}

				// We know the carry and the XY XOR's out are certain now.
				// Find the cAnd AND-gate that has these two inputs.
				var cAnd = gates.Single(g => g.HasInput(carry, xyXor.Out) && g.IsAnd);

				// Now find the cOr OR-gate that has cAnd and yxAnd as inputs.
				// Both may be swapped, but let's assume they're not both swapped. Unlike for the
				// Z XOR-gate, where we could look for an XOR with output z, we don't have that
				// certainty here. Instead we look for an OR-gate (there's only one in each part)
				// that has at least one of these as inputs and we assume there's just one and it's
				// the right one.
				var cOr = gates.SingleOrDefault(g => g.HasInput(cAnd.Out, xyAnd.Out) && g.IsOr);
				if (cOr == null)
				{
					var cOrCandidates = gates.Where(g => g.HasInput(cAnd.Out) || g.HasInput(xyAnd.Out) && g.IsOr).ToArray();
					if (cOrCandidates.Length != 1)
						throw new Exception("Not exactly one candidate for the carry OR-gate");
					cOr = cOrCandidates.Single();
					if (cOr.HasInput(xyAnd.Out))
						Swap(cAnd.Out, cOr.OtherInput(xyAnd.Out));
					else
						Swap(xyAnd.Out, cOr.OtherInput(cAnd.Out));
				}

				// The cOr OR-gate is now certain.
				// Make sure that it's input really is the one coming from the xy AND-gate; if not
				// then the AND-gate's output need to be swapped.
				var xyAndActual = cOr.OtherInput(cAnd.Out);
				if (xyAnd.Out != xyAndActual)
				{
					Swap(xyAnd.Out, xyAndActual);
				}

				// The carry's output may be swapped but we'll deal with that in the next round
				carry = cOr.Out;
			}

			var result = string.Join(',', swaps.OrderBy(x => x));
			return result;
		}

		private class Gate
		{
			public static Gate Parse(string s)
			{
				var (a, op, b, o) = s.RxMatch("%s %s %s -> %s").Get<string, string, string, string>();
				return new Gate(a, b)
				{
					Out = o,
					IsAnd = op == "AND",
					IsOr = op == "OR",
					IsXor = op == "XOR"
				};
			}

			private Gate(string a, string b) => (_a, _b) = (a, b);

			private readonly string _a;
			private readonly string _b;

			public string Out { get; set; }
			public bool IsAnd { get; init; }
			public bool IsOr { get; init; }
			public bool IsXor { get; init; }

			public bool HasInput(string x) => _a == x || _b == x;
			public bool HasInput(string a, string b) => _a == a && _b == b || _a == b && _b == a;
			public string OtherInput(string x) => x == _a ? _b : _a;
		}
	}
}
