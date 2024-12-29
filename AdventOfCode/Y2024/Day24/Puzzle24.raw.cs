using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.ComponentModel;
using Microsoft.VisualBasic;
using MathNet.Numerics.RootFinding;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Y2024.Day24.Raw
{
	internal class Puzzle : Puzzle<long, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 24;

		public override void Run()
		{
			//Run("test1").Part1(4);
			//Run("test2").Part1(2024);
			Run("input").Part1(58639252480880).Part2("bkr,mqh,rnq,tfb,vvr,z08,z28,z39");

			// not ctv,kgn,kwv,mqh,tfb,vvr,z28,z39
			// not ctv,jds,kgn,mqh,tfb,vkh,z28,z39
			// bkr,mqh,rnq,tfb,vvr,z08,z28,z39


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

			var pending = new SafeDictionary<(string, string, string), List<string>>(() => []);
			foreach (var line in parts[1])
			{
				var (a, op, b, c) = line.RxMatch("%s %s %s -> %s").Get<string, string, string, string>();
				pending[(a, op, b)].Add(c);
			}

			var manyout = pending.Where(x => x.Value.Count > 1).ToArray();
			;

			// Console.WriteLine("digraph {");
			// foreach (var pend in pending)
			// {
			// 	Console.WriteLine($"  \"{pend.Key.Item1}\" -> \"{pend.Key}\"");
			// 	Console.WriteLine($"  \"{pend.Key.Item3}\" -> \"{pend.Key}\"");
			// 	foreach (var o in pend.Value)
			// 	{
			// 		Console.WriteLine($"  \"{pend.Key}\" -> \"{o}\"");
			// 	}
			// }
			// Console.WriteLine("}");

			while (pending.Any())
			{
				var next = pending.First(x => values.ContainsKey(x.Key.Item1) && values.ContainsKey(x.Key.Item3));
				var v = next.Key;
				//Console.Write($"{next.Key} -> {next.Value}: ");
				//Console.Write($"{values[v.Item1]} {v.Item2} {values[v.Item3]}");
				var value = v.Item2 switch
				{
					"AND" => values[v.Item1] && values[v.Item3],
					"OR" => values[v.Item1] || values[v.Item3],
					"XOR" => values[v.Item1] ^ values[v.Item3],
					_ => throw new Exception()
				};
				//Console.WriteLine($"  => {value}");
				foreach (var dest in next.Value)
					values[dest] = value;
				pending.Remove(next.Key);
			}

			var sum = 0UL;
			foreach (var w in values.Where(x => x.Key.StartsWith("z") && x.Value))
			{
				var bit = int.Parse(w.Key[1..]);
				sum += 1UL << bit;
			}

			return (long)sum;
		}

		private class Gate
		{
			public static Gate Parse(string s)
			{
				var (a, op, b, o) = s.RxMatch("%s %s %s -> %s").Get<string, string, string, string>();
				if (a.CompareTo(b) > 0)
					(a, b) = (b, a);
				return new Gate
				{
					A = a,
					B = b,
					Op = op switch
					{
						"AND" => Operand.And,
						"OR" => Operand.Or,
						"XOR" => Operand.Xor,
						_ => throw new Exception()
					},
					Out = o,
					IsEntryGate = a.StartsWith('x')
				};
			}
			public string A;
			public string B;
			public string Out;
			public Operand Op;
			public enum Operand { And, Or, Xor }
			public bool HasInput(string x) => A == x || B == x;
			public bool HasInput(string a, string b) => A == a && B == b || A == b && B == a;
			public bool IsEntryGate { get; init; }
			public override string ToString()
			{
				return $"{A} {Op} {B} => {Out}";
			}
			public override bool Equals(object obj)
			{
				var g = obj as Gate;
				return g is { } && A == g.A && B == g.B && Op == g.Op && Out == g.Out;
			}
			public override int GetHashCode()
			{
				return HashCode.Combine(A, B, Op, Out);
			}
		}


		protected override string Part2(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();

			var values = new Dictionary<string, bool>();
			foreach (var line in parts[0])
			{
				var (name, val) = line.RxMatch("%s: %d").Get<string, int>();
				values[name] = val == 1;
			}

			var gates = new HashSet<Gate>(parts[1].Select(Gate.Parse));

			var width = values.Count / 2;

			var zXorGates = gates.Where(g => g.Op == Gate.Operand.Xor && !g.IsEntryGate).ToArray();
			var zXorcAndPairs = zXorGates.Select(z => (Xor: z, And: gates.Single(gor => gor.HasInput(z.A, z.B) && gor.Op == Gate.Operand.And))).OrderBy(x => x.Xor.A).ToList();

			var carry = gates.Single(g => g.A == "x00" && g.B == "y00" && g.Op == Gate.Operand.And).Out;

			var validatedGates = new HashSet<Gate>(gates.Where(g => g.A[0] == 'x' && g.B[0] == 'y'));

			// TestAdders(1, []);

			// bool TestSwapAdders(int start, string carry, HashSet<string> swaps, string a, string b)
			// {

			// }

			// bool TestAdders(int start, string carry, HashSet<(string, string)> swaps)
			// {
			// 	if (swaps.Count == 4)
			// 	{
			// 		Console.Write(string.Join(',', swaps.OrderBy(x => x)));
			// 	}
			// 	if (swaps.Count > 4)
			// 		return false;

			var swaps = new List<string>();

			for (var i = 1; i < width; i++)
			{
				if (swaps.Count > 8)
					return "something's wrong after " + i;

				var id = i.ToString("D2");
				var (xwire, ywire, zwire) = ("x" + id, "y" + id, "z" + id);
				var xyXor = gates.Single(g => g.HasInput(xwire, ywire) && g.Op == Gate.Operand.Xor); // certain
				var xyAnd = gates.Single(g => g.HasInput(xwire, ywire) && g.Op == Gate.Operand.And); // certain

				// There's just one zXor-gate that receives input from the xyXor-gate
				// But the carry or xyXorOut (or both!) may be lying
				var zXor = zXorGates.SingleOrDefault(g => g.HasInput(carry, xyXor.Out));
				if (zXor == null)
				{
					var zOrMostLikely = zXorGates.SingleOrDefault(g => g.Out == zwire);
					if (zOrMostLikely.HasInput(carry))
					{
						// swap xyXor
						var xyXorOutDesired = zOrMostLikely.A == carry ? zOrMostLikely.B : zOrMostLikely.A;
						SwapOuts(xyXor.Out, xyXorOutDesired);
					}
					else if (zOrMostLikely.HasInput(xyXor.Out))
					{
						// swap carry
						var carryDesired = zOrMostLikely.A == xyXor.Out ? zOrMostLikely.B : zOrMostLikely.A;
						SwapOuts(carry, carryDesired);
						carry = carryDesired;
					}
					else throw new Exception("both are bad!");

					zXor = zXorGates.SingleOrDefault(g => g.HasInput(carry, xyXor.Out));
					Debug.Assert(zXor != null);
				}

				if (zXor.Out != zwire)
				{
					// swap zXor output
					SwapOuts(zXor.Out, zwire);

					zXorGates = gates.Where(g => g.Op == Gate.Operand.Xor && !g.IsEntryGate).ToArray();
					zXorcAndPairs = zXorGates.Select(z => (Xor: z, And: gates.Single(gor => gor.HasInput(z.A, z.B) && gor.Op == Gate.Operand.And))).OrderBy(x => x.Xor.A).ToList();

				}


				var cAnd = zXorcAndPairs.Single(x => x.Xor.HasInput(carry)).And;

				var cOr = gates.SingleOrDefault(g => g.HasInput(cAnd.Out, xyAnd.Out) && g.Op == Gate.Operand.Or);
				if (cOr == null)
				{
					// cAnd and/or xyAnd.Out is swapped
					var cOrCandidates = gates.Where(g => g.HasInput(cAnd.Out) || g.HasInput(xyAnd.Out) && g.Op == Gate.Operand.Or).ToArray();
					cOr = cOrCandidates.Single(); // dodgy

					if (cOr.HasInput(xyAnd.Out))
					{
						// swap cAnd.Out
						var cAndOutDesired = cOr.A == xyAnd.Out ? cOr.B : cOr.A;
						SwapOuts(cAnd.Out, cAndOutDesired);
					}
					else if (cOr.HasInput(cAnd.Out))
					{
						// swap xyAnd.Out
						var xyAndOutDesired = cOr.A == cAnd.Out ? cOr.B : cOr.A;
						SwapOuts(xyAnd.Out, xyAndOutDesired);
					}
					else throw new Exception("both are bad 2!");

					cOr = gates.SingleOrDefault(g => g.HasInput(cAnd.Out, xyAnd.Out) && g.Op == Gate.Operand.Or);
					Debug.Assert(cOr != null);
				}

				var xyAndDesiredOut = cOr.A == cAnd.Out ? cOr.B : cOr.A;
				if (xyAnd.Out != xyAndDesiredOut)
				{
					SwapOuts(xyAnd.Out, xyAndDesiredOut);
				}

				carry = cOr.Out; // maybe not correct
			}

			var result = string.Join(',', swaps.OrderBy(x => x));
			return result;
			// }

			void SwapOuts(string a, string b)
			{
				var ga = gates.Single(g => g.Out == a);
				var gb = gates.Single(g => g.Out == b);
				(ga.Out, gb.Out) = (gb.Out, ga.Out);
				swaps.Add(ga.Out);
				swaps.Add(gb.Out);
				Debug.Assert(swaps.Count == swaps.Distinct().Count());
			}

			// ;


			// var goodwires = new HashSet<string>();
			// var badvalues = new HashSet<string>();
			// var badpairs = new HashSet<(string, string)>();


			// for (var i = 0; i < width; i++)
			// {
			// 	if (TestAddBitOperations(gates, i, out var vals))
			// 	{
			// 		foreach (var v in vals)
			// 			goodwires.Add(v);
			// 	}
			// 	else
			// 	{
			// 		// badpairs = new HashSet<(string, string)>();
			// 		// foreach (var v in vals)
			// 		// 	badvalues.Add(v);
			// 		var baddies = vals
			// 			.Except(goodwires)
			// 			//.Where(x => !swapped.Any(swap => swap.Item1 == x || swap.Item2 == x))
			// 			.ToArray();
			// 		;
			// 		badvalues = new HashSet<string>(badvalues.Concat(baddies));

			// 		// for (var j = 0; j < baddies0.Length; j++)
			// 		// {
			// 		// 	for (var k = j + 1; k < baddies0.Length; k++)
			// 		// 	{
			// 		// 		var (a, b) = (baddies0[j], baddies0[k]);
			// 		// 		var ga = gates.Single(g => g.Out == a);
			// 		// 		var gb = gates.Single(g => g.Out == b);
			// 		// 		(ga.Out, gb.Out) = (b, a);
			// 		// 		var ok = TestAddBitOperations(gates, i, out var _);
			// 		// 		if (ok)
			// 		// 		{
			// 		// 			var badpairs2 = new HashSet<(string, string)>(swapped)
			// 		// 			{
			// 		// 				(a, b)
			// 		// 			};
			// 		// 			var goodwires2 = new HashSet<string>(goodwires);//.Except([a]).Except([b]));
			// 		// 			TestAddBitOperationsFromWidth(gates, i + 1, width, goodwires2, badpairs2);
			// 		// 		}
			// 		// 		(ga.Out, gb.Out) = (a, b);
			// 		// 	}
			// 		// }
			// 		//Console.WriteLine($"bit {i}: {string.Join(' ', badpairs)}");
			// 	}
			// }

			// var baddies0 = badvalues.ToArray();

			// var gatesByOut = gates.ToDictionary(x => x.Out, x => x);
			// // var swapcount = 0;
			// // foreach (var swaps in MathHelper.Combinations(baddies0, 8))
			// // {
			// // 	//var (a0, a1, a2, a3, a4, a5, a6, a7) = (c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7] );
			// // 	if (swapcount++ % 1000 == 0)
			// // 		Console.Write('.');
			// // 	var gs = swaps.Select(s => gatesByOut[s]).ToArray();
			// // 	SwapTwo(ref gs[0].Out, ref gs[1].Out);
			// // 	SwapTwo(ref gs[2].Out, ref gs[3].Out);
			// // 	SwapTwo(ref gs[4].Out, ref gs[5].Out);
			// // 	SwapTwo(ref gs[6].Out, ref gs[7].Out);
			// // 	if (CanAddAll(gates, width))
			// // 	{
			// // 		Console.Write(string.Join(',', swaps));
			// // 	}
			// // 	SwapTwo(ref gs[0].Out, ref gs[1].Out);
			// // 	SwapTwo(ref gs[2].Out, ref gs[3].Out);
			// // 	SwapTwo(ref gs[4].Out, ref gs[5].Out);
			// // 	SwapTwo(ref gs[6].Out, ref gs[7].Out);

			// // 	void SwapTwo(ref string a, ref string b) { (a, b) = (b, a); };
			// // }

			// var faultsAt = new Dictionary<string, int>();
			// int GetFaultsAt(HashSet<string> swaps)
			// {
			// 	var key = string.Join(',', swaps.OrderBy(x => x));
			// 	if (!faultsAt.TryGetValue(key, out var n))
			// 	{
			// 		n = faultsAt[key] = Faults(gates, width);
			// 	}
			// 	return n;
			// }

			// baddies0 = gates.Select(g => g.Out).ToArray();



			// Swap([]);

			// void Swap(HashSet<string> swaps)
			// {
			// 	for (var i = 0; i < baddies0.Length; i++)
			// 	{
			// 		for (var j = 0; j < baddies0.Length; j++)
			// 		{
			// 			var (a, b) = (baddies0[i], baddies0[j]);
			// 			if (a == b)
			// 				continue;
			// 			if (swaps.Contains(a) || swaps.Contains(b))
			// 				continue;
			// 			if (!swaps.Any())
			// 				Console.WriteLine($"Test {a} and {b}");
			// 			var faults0 = GetFaultsAt(swaps);
			// 			var ga = gatesByOut[a];
			// 			var gb = gatesByOut[b];
			// 			(ga.Out, gb.Out) = (b, a);
			// 			var swaps2 = new HashSet<string>(swaps.Append(a).Append(b));
			// 			var faults1 = GetFaultsAt(swaps2);
			// 			if (faults1 < faults0)
			// 			{
			// 				if (swaps2.Count == 8)
			// 				{
			// 					if (Faults(gates, width) == 0)
			// 						Console.WriteLine(string.Join(',', swaps2.OrderBy(x => x)));
			// 					if (Faults(gates, width) == 1)
			// 						Console.WriteLine("nearly: " + string.Join(',', swaps2.OrderBy(x => x)));
			// 				}
			// 				else
			// 				{
			// 					Swap(swaps2);
			// 				}
			// 			}
			// 			(ga.Out, gb.Out) = (a, b);
			// 		}

			// 	}
			// }


			// TestAddBitOperationsFromWidth(gates, 0, width, [], []);



			// //var baddies = badvalues.Except(goodvalues).ToArray();
			// //;
			// //var bads = badpairs.SelectMany(p => new string[] { p.Item1, p.Item2 }).Distinct().OrderBy(x => x).ToArray();

			// static bool CanAddAll(HashSet<Gate> gates, int width)
			// {
			// 	for (var i = 0; i < width; i++)
			// 		if (!TestAddBitOperations(gates, i, out var _))
			// 		{
			// 			//Console.WriteLine($"failed at {i}");
			// 			return false;
			// 		}
			// 	//Console.WriteLine("bingo");
			// 	return true;
			// }

			// static int Faults(HashSet<Gate> gates, int width)
			// {
			// 	var faults = 0;
			// 	for (var i = 0; i < width; i++)
			// 		if (!TestAddBitOperations(gates, i, out var _))
			// 			faults++;
			// 	return faults;
			// }

			// static void TestAddBitOperationsFromWidth(HashSet<Gate> gates, int bit, int width, HashSet<string> goodwires, HashSet<(string, string)> swapped)
			// {
			// 	if (swapped.Count == 3)
			// 		;

			// 	if (swapped.Count == 4)
			// 	{
			// 		var bads = swapped.SelectMany(p => new string[] { p.Item1, p.Item2 }).Distinct().OrderBy(x => x).ToArray();
			// 		Console.Write($"{string.Join(',', bads)}    {string.Join(' ', swapped)} ... ");
			// 		for (var i = 0; i < width; i++)
			// 			if (!TestAddBitOperations(gates, i, out var _))
			// 			{
			// 				Console.WriteLine($"failed at {i}");
			// 				return;
			// 			}
			// 		Console.WriteLine("bingo");
			// 		return;
			// 	}
			// 	for (var i = 0; i < width; i++)
			// 	{
			// 		if (i == width - 1)
			// 			;
			// 		if (TestAddBitOperations(gates, i, out var vals))
			// 		{
			// 			foreach (var v in vals)
			// 				goodwires.Add(v);
			// 		}
			// 		else
			// 		{
			// 			// badpairs = new HashSet<(string, string)>();
			// 			// foreach (var v in vals)
			// 			// 	badvalues.Add(v);
			// 			var baddies0 = vals
			// 				.Except(goodwires)
			// 				.Where(x => !swapped.Any(swap => swap.Item1 == x || swap.Item2 == x))
			// 				.ToArray();
			// 			;
			// 			for (var j = 0; j < baddies0.Length; j++)
			// 			{
			// 				for (var k = j + 1; k < baddies0.Length; k++)
			// 				{
			// 					var (a, b) = (baddies0[j], baddies0[k]);
			// 					var ga = gates.Single(g => g.Out == a);
			// 					var gb = gates.Single(g => g.Out == b);
			// 					(ga.Out, gb.Out) = (b, a);
			// 					var ok = TestAddBitOperations(gates, i, out var _);
			// 					if (ok)
			// 					{
			// 						var badpairs2 = new HashSet<(string, string)>(swapped)
			// 						{
			// 							(a, b)
			// 						};
			// 						var goodwires2 = new HashSet<string>(goodwires);//.Except([a]).Except([b]));
			// 						TestAddBitOperationsFromWidth(gates, i + 1, width, goodwires2, badpairs2);
			// 					}
			// 					(ga.Out, gb.Out) = (a, b);
			// 				}
			// 			}
			// 			//Console.WriteLine($"bit {i}: {string.Join(' ', badpairs)}");
			// 		}
			// 	}
			// }

			// static bool TestAddBitOperations(HashSet<Gate> gates, int bit, out HashSet<string> vals)
			// {
			// 	vals = [];
			// 	return
			// 		TestAddBit(bit, 1, 0, vals) &&
			// 		TestAddBit(bit, 0, 1, vals) &&
			// 		TestAddBit(bit, 1, 1, vals);

			// 	bool TestAddBit(int bit, int x, int y, HashSet<string> vals)
			// 	{
			// 		var values = new Dictionary<string, int>();
			// 		try
			// 		{
			// 			var carry = x + y == 2 ? 1 : 0;
			// 			for (var i = 0; i <= bit + carry; i++)
			// 			{
			// 				values[$"x{i:D2}"] = 0;
			// 				values[$"y{i:D2}"] = 0;
			// 			}
			// 			values[$"x{bit:D2}"] = x;
			// 			values[$"y{bit:D2}"] = y;
			// 			var completed = Run(gates, values, bit, 1 + carry);
			// 			if (!completed)
			// 				return false;
			// 			var outputs = values.Where(x => x.Key.StartsWith('z')).OrderBy(x => x.Key).Select(x => x.Value).ToArray();
			// 			if (outputs.Take(bit).Any(z => z != 0))
			// 				return false;
			// 			if (outputs[bit] != (x + y) % 2)
			// 				return false;
			// 			if (carry == 1)
			// 				if (outputs[bit + 1] != 1)
			// 					return false;
			// 			return true;
			// 		}
			// 		finally
			// 		{
			// 			foreach (var v in values.Keys.Where(x => x[0] is not 'x' and not 'y'))
			// 				vals.Add(v);
			// 		}
			// 	}
			// }

			// static bool Run(HashSet<Gate> gates, Dictionary<string, int> values, int bit, int outwidth)
			// {
			// 	var outbits = Enumerable.Range(0, bit + outwidth).Select(i => $"z{i:D2}").ToArray();
			// 	while (true)
			// 	{
			// 		var gate = gates.FirstOrDefault(g =>
			// 			 values.Keys.Contains(g.A) &&
			// 			 values.Keys.Contains(g.B) &&
			// 			 !values.Keys.Contains(g.Out));
			// 		if (gate == null)
			// 			return false;
			// 		values[gate.Out] = gate.Op switch
			// 		{
			// 			Gate.Operand.And => values[gate.A] & values[gate.B],
			// 			Gate.Operand.Or => values[gate.A] | values[gate.B],
			// 			Gate.Operand.Xor => values[gate.A] ^ values[gate.B],
			// 			_ => throw new Exception()
			// 		};
			// 		if (outbits.All(values.ContainsKey))
			// 			return true;
			// 	}
			// }

			// var pending = new SafeDictionary<(string, string, string), List<string>>(() => []);
			// foreach (var line in parts[1])
			// {
			// 	var (a, op, b, c) = line.RxMatch("%s %s %s -> %s").Get<string, string, string, string>();
			// 	pending[(a, op, b)].Add(c);
			// }

			// var allz = pending.Where(x => x.Value.SingleOrDefault()?.StartsWith("z") ?? false).ToArray();
			// var trails = allz.Select(z =>
			// {
			// 	return $"Trail {z.Value.Single()}: " + FindTrail(z.Key.Item1, z.Key.Item3, 0);
			// });

			// string FindTrail(string a, string b, int level)
			// {
			// 	var pa = values.ContainsKey(a) ? a : FindTrailFromOutput(a, level - 1);
			// 	var pb = values.ContainsKey(b) ? b : FindTrailFromOutput(b, level - 1);
			// 	return $"{level:D2}-{pa}-{pb}";
			// }
			// string FindTrailFromOutput(string o, int level)
			// {
			// 	var p = pending.Where(x => x.Value.Contains(o)).Single();
			// 	return FindTrail(p.Key.Item1, p.Key.Item3, level);
			// }

			// // foreach (var t in trails.OrderBy(x => x))
			// // {
			// // 	Console.WriteLine(t);
			// // }

			// // var startXy = gates.Values.Where(g => g.IsXy).ToArray();
			// // foreach (var g in startXy.OrderBy(g => g.A).ThenBy(g => g.B).ThenBy(g => g.Op))
			// // {
			// // 	var involved = FindInvolved(g);
			// // 	Console.WriteLine($"{involved.Length}  {g}        {involved}");
			// // }

			// // string FindInvolved(Gate gate)
			// // {
			// // 	var s = gate.Out + "," + string.Join(',', gates.Values.Where(g => g.HasInput(gate.Out)).Select(FindInvolved));
			// // 	return s;
			// // }

			// // foreach (var g in gates.Values.OrderBy(x => x.A).ThenBy(x => x.B).ThenBy(x => x.Op))
			// // {
			// // 	if (g.A[0] == 'x' && g.B[0] == 'y')
			// // 	{
			// // 		if (g.A[1..] == g.B[1..])
			// // 		{
			// // 			PrintGate(g, "", 6);
			// // 		}
			// // 	}
			// // }

			// // void PrintGate(Gate gate, string indent, int level)
			// // {
			// // 	if (level == 0)
			// // 		return;
			// // 	Console.WriteLine($"{indent}{gate}{(gate.Out.StartsWith('z') ? "       ##################" : "")}");
			// // 	foreach (var wa in gates.Where(x => x.Value.A == gate.Out || x.Value.B == gate.Out))
			// // 	{
			// // 		PrintGate(wa.Value, indent + "   ", level - 1);
			// // 	}
			// // }

			// // foreach (var p in pending)
			// // {
			// // 	var v = p.Key;
			// // 	var (a, b) = (v.Item1[0], v.Item3[0]);
			// // 	if (a == 'x' && b == 'y' || a == 'y' && b == 'x')
			// // 	{
			// // 		if (v.Item1[1..] == v.Item3[1..])
			// // 		{
			// // 			Console.WriteLine($"{v.Item1} {v.Item2} {v.Item3} => {p.Value.Single()}");
			// // 		}
			// // 	}
			// // }

			// return null;

			// var allo = pending.SelectMany(p => p.Value).Distinct().ToArray();
			// //		while (true)
			// {


			// 	while (pending.Any())
			// 	{
			// 		var next = pending.First(x => values.ContainsKey(x.Key.Item1) && values.ContainsKey(x.Key.Item3));
			// 		var v = next.Key;
			// 		//Console.Write($"{next.Key} -> {next.Value}: ");
			// 		//Console.Write($"{values[v.Item1]} {v.Item2} {values[v.Item3]}");
			// 		var value = v.Item2 switch
			// 		{
			// 			"AND" => values[v.Item1] && values[v.Item3],
			// 			"OR" => values[v.Item1] || values[v.Item3],
			// 			"XOR" => values[v.Item1] ^ values[v.Item3],
			// 			_ => throw new Exception()
			// 		};
			// 		//Console.WriteLine($"  => {value}");
			// 		foreach (var dest in next.Value)
			// 			values[dest] = value;
			// 		pending.Remove(next.Key);
			// 	}

			// 	var sumz = 0UL;
			// 	foreach (var w in values.Where(x => x.Key.StartsWith("z") && x.Value))
			// 	{
			// 		var bit = int.Parse(w.Key[1..]);
			// 		sumz += 1UL << bit;
			// 	}
			// 	var sumx = 0UL;
			// 	foreach (var w in values.Where(x => x.Key.StartsWith("x") && x.Value))
			// 	{
			// 		var bit = int.Parse(w.Key[1..]);
			// 		sumx += 1UL << bit;
			// 	}
			// 	var sumy = 0UL;
			// 	foreach (var w in values.Where(x => x.Key.StartsWith("y") && x.Value))
			// 	{
			// 		var bit = int.Parse(w.Key[1..]);
			// 		sumy += 1UL << bit;
			// 	}

			// 	if (sumx + sumy == sumz)
			// 	{
			// 		var swaps = 0;
			// 	}

			// }


			// return null;
		}
	}
}
