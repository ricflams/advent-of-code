using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day24.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 24";
		public override int Year => 2021;
		public override int Day => 24;

		public void Run()
		{
			//Run("test1").Part1(0).Part2(0);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(53999995829399).Part2(11721151118175);
			// 99999999909999 too high
			// 99999999961374 too high
			// 11111111918819 too low
		}

		internal struct Regs
        {
			public long W;
			public long X;
			public long Y;
			public long Z;
		}

		private static int[] Digits = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };
		private static int[] Digits19 = new int[] { 1,2,3,4,5,6,7,8,9 };


		protected override long Part1(string[] input)
		{
			//var regs = Dictionary<char, long>();

			var N = 14;
			var factors = Enumerable.Range(0, N)
				.Select(i =>
				{
					var part = input.Skip(i * input.Length / N).ToArray();
					return new int[]
					{
						int.Parse(part[4].Split(' ')[2]),
						int.Parse(part[5].Split(' ')[2]),
						int.Parse(part[15].Split(' ')[2])
					};
				})
				.ToArray();

			//var z = 0;
			//for (var i = 0; i < N; i++)
			//{
			//	var f = factors[i];
			//	var dig = Enumerable.Range(1, 9)
			//		.Select(d =>
			//		   {
			//			   //var z = 0;
			//			   var z3 = z % 26 == d + f[1]
			//				   ? z / f[0]
			//				   : z + d + f[2];
			//			   return (d, z3);
			//		   })
			//		.Where(x => x.z3 == 0)
			//		.Select(x => x.d)
			//		.Max();
			//	Console.Write(dig);
			//}
			//Console.WriteLine();

			for (var k = 0; k < 3; k++)
            {
				for (var i = 0; i < N; i++)
                {
					Console.Write($"{factors[i][k],3} ");
				}
				Console.WriteLine();
			}

			for (var i = 0; i < N; i++)
			{
				var f = factors[i];
				for (var z = 0; z < 100; z++)
                {
					for (var d = 1; d < 10; d++)
					{
						var regs = new Regs();

						regs.Z = z;

						regs.W = d;           //inp w
						regs.X = 0;            //mul x 0
						regs.X += regs.Z;   //add x z
						regs.X %= 26;          //mod x 26  ---> Z is always positive, >0
						regs.Z /= f[0];       //div z AAAAA
						regs.X += f[1];           //add x -9
						regs.X = regs.X == regs.W ? 1 : 0;    //eql x w
						regs.X = regs.X == 0 ? 1 : 0;                         //eql x 0
						regs.Y = 0;                                                        //mul y 0
						regs.Y += 25;                                                          //add y 25
						regs.Y *= regs.X;                                                           //mul y x
						regs.Y += 1;                                                           //add y 1
						regs.Z *= regs.Y;                                                          //mul z y
						regs.Y = 0;                                                        //mul y 0
						regs.Y += regs.W;                                                          //add y w
						regs.Y += f[2];                                                        //add y 10
						regs.Y *= regs.X;                                                          //mul y x
						regs.Z += regs.Y;                                                          //add z y

						var z3 = z % 26 == d - f[1]
							? z / f[0]
							: z / f[0] * 26 + d + f[2];

						if (z3 != regs.Z)
						{
							Console.WriteLine($"Mismatch: regs:{regs.Z} z3={z3}");
						}
					}

				}

				//  1   1   1   1   1  26   1  26  26   1  26  26  26  26
				// 15  10  12  10  14 -11  10 -16  -9  11  -8  -8 -10  -9
				// 13  16   2   8  11   6  12   2   2  15   1  10  14  10

			}

			//var lastz = 0;
			//for (var i = N; i-- > 0;)
			//{
			//	var matches = CalcDigit(i, lastz).ToArray().Last().Item1;
			//	//Console.WriteLine($"{digit} {lastz}");
			//	//lastz = lastz2;
			//	Console.WriteLine($"{matches} {lastz}");
			//	lastz = matches;
			//}




			//for (var i = N-1; i-- > 0;)
			//{
			//	var matches = CalcDigit(i, 0).ToArray();


			//	var matches2 = matches.SelectMany(x => CalcDigit(i, x.Item2).ToArray())
			//		.GroupBy(x => x.Item1)
			//		.ToDictionary(x => x.Key, x => x);
			//	//Console.WriteLine($"{digit} {lastz}");
			//	//lastz = lastz2;
			//	//Console.WriteLine($"{matches} {lastz}");
			//	//lastz = matches;
			//}

			//IEnumerable<(int, int)> CalcDigit(int pos, int outz)
   //         {
			//	var f = factors[pos];
			//	var maxdigit = 0;
			//	//var minlastz = 0;
			//	for (var z = 0; z < 20000; z++)
			//	{
			//		for (var d = 1; d < 10; d++)
			//		{
			//			var cond1 = z % 26 + f[1] == d && z / f[0] == outz;
			//			var cond2 = z % 26 + f[1] != d && z / f[0] * 26 + f[2] + d == outz;
			//			if (cond1 || cond2)
			//			{
			//				if (d >= maxdigit)
			//				{
			//					Console.WriteLine($"  at {pos} for {outz}: found d={d} at z={z}");
			//					maxdigit = d;
			//					//minlastz = z;
			//					yield return (d, z);
			//				}
			//				if (d == 9)
			//				{
			//					// as good as it gets
			//				//	yield break;
			//				}
			//			}
			//		}
			//	}
			//}

			//return 0;




            //Console.WriteLine();

            //         var lastZ = new int[] { 0 };
            //         for (var i = N; i-- > 0;)
            //         {
            //             var opts = PossibleZ(i, lastZ).ToArray(); // (maxDigit, chosenZ)
            //	foreach (var x in opts)
            //             {
            //		Console.WriteLine($"Digit {x.Item1} with initial z={x.Item2} will produce zs");
            //	}
            //	Console.WriteLine();

            //	lastZ = opts.Select(x => x.Item2).ToArray();
            //         }

            var seen = new Dictionary<string, bool>();
			var result = "";
            IsMatch(0, 0);
            bool IsMatch(int pos, int z)
            {
                if (pos == N)
                    return z == 0;

                var key = $"{pos}-{z}";

                if (seen.TryGetValue(key, out var ok))
                    return ok;

                if (z > 10_000_000)
                    return false;

                var f = factors[pos];
				//var digit = Digits19.FirstOrDefault(d =>
				var digit = Digits.FirstOrDefault(d =>
				{
                    var z3 = z % 26 + f[1] == d
                        ? z / f[0]
                        : z / f[0] * 26 + d + f[2];
                    return z3 >= 0 && IsMatch(pos + 1, z3);
                });
                ok = digit > 0;
                seen[key] = ok;
				if (ok)
					result = digit.ToString() + result;
                return ok;
            }

			Console.WriteLine($"seen: {seen.Count}");
			return long.Parse(result);



            //{
            //    var z = 0;
            //    for (var i = 0; i < N; i++)
            //    {
            //        var f = factors[i];
            //        var digit = Digits.First(d =>
            //        {
            //            var z3 = z % 26 == d + f[1]
            //                ? z / f[0]
            //                : z / f[0] * 26 + d + f[2];
            //            return z >= 0;
            //        });
            //        Console.Write(digit);
            //        var z4 = z % 26 == digit + f[1]
            //            ? z / f[0]
            //            : z / f[0] * 26 + digit + f[2];
            //        z = z4;
            //    }
            //    Console.WriteLine();
            //}

            //	var z = 0;



            //	for (var d = 1; d < 10; d++)
            //	{

            //		//var regs = new Regs();

            //regs.Z = z;




            //// x = z % 26 - f1
            //// x = (x==d ? 0 : 1)
            //// z = z / f0 * (25*x+1) + (d+f2)*x    <--- f0: 1 or 26
            //var x = z % 26 - f[1] == d ? 0 : 1;
            //var z2 = z / f[0] * (25 * x + 1) + (d + f[2]) * x;

            //var z3 = z % 26 == d + f[1]
            //	? z / f[0]
            //	: z + d + f[2];

            //// last digit d=9, hence z=9

            //// z + d + f[2] == 0, f[2]=10
            ////  z + d + 10 = 0
            ////  z = -(d+10)
            ////  z = -9..-1

            //if (regs.Z != z3)
            //	;
            //Console.Write($"{z2:D2} ");

     //       IEnumerable<(int, int)> PossibleZ(int step, int[] willproduceZ)
     //       {
     //           var f = factors[step];
     //           for (var z = -100; z < 100; z++)
     //           {
					//foreach (var d in Digits.Where(d =>
					//{
					//	var z3 = z % 26 == d + f[1]
					//		? z / f[0]
					//		: z / f[0] * 26 + d + f[2];
					//	return z3 >= 0 && willproduceZ.Any(zz => z3 == zz);
					//}))
     //               {
					//	yield return (d, z);
					//}
     //           }
     //       }


     //       var cache = new Dictionary<string, long?>();

			//var thebigmax = 0L;
			//var themax = FindMax(0, 0, 0);
			//return themax.Value;

			//IEnumerable<long> FindMax(long val, int pos, long z)
			//{
			//	if (pos == N)
   //             {
			//		if (z == 0)
			//			yield return val;
			//		yield break;
   //             }

			//	if (z < 0)
			//		throw new Exception();

			//	var key = $"{pos}-{z}";
			//	if (cache.TryGetValue(key, out var value))
			//		return value;

			//	var f = factors[pos];
			//	var max = (long?)null;
			//	for (var d = 10; d-- > 0;)
			//	{
			//		var z3 = z % 26 == d + f[1]
			//			? z / f[0]
			//			: z + d + f[2];



			//		// x = z % 26 - f1
			//		// x = (x==d ? 0 : 1)
			//		// z = z / f0 * (25*x+1) + (d+f2)*x    <--- f0: 1 or 26
			//		var x = z % 26 - f[1] == d ? 0 : 1;
			//		var z2 = z / f[0] * (25 * x + 1) + (d + f[2]) * x;
			//		var dmax = FindMax(val * 10 + d, pos + 1, z2);
			//		if (dmax != null)
   //                 {
			//			max = dmax;
			//			if (max > thebigmax)
			//				thebigmax = max.Value;
			//			break;
   //                 }
			//	}

			//	cache[key] = max;
			//	return max;
			//}
		}

		//class State
		//{
		//	public int Z;
		//	public int Len;
		//	public long Value;
		//}

		//regs.W = d;           //inp w
		//regs.X = 0;            //mul x 0
		//regs.X += regs.Z;   //add x z
		//regs.X %= 26;          //mod x 26  ---> Z is always positive, >0
		//regs.Z /= f[0];       //div z AAAAA
		//regs.X -= f[1];           //add x BBBBB
		//regs.X = regs.X == regs.W ? 1 : 0;    //eql x w
		//regs.X = regs.X == 0 ? 1 : 0;                         //eql x 0
		//regs.Y = 0;                                                        //mul y 0
		//regs.Y += 25;                                                          //add y 25
		//regs.Y *= regs.X;                                                           //mul y x
		//regs.Y += 1;                                                           //add y 1
		//regs.Z *= regs.Y;                                                          //mul z y
		//regs.Y = 0;                                                        //mul y 0
		//regs.Y += regs.W;                                                          //add y w
		//regs.Y += f[2];                                                        //add y CCCCC
		//regs.Y *= regs.X;                                                          //mul y x
		//regs.Z += regs.Y;                                                          //add z y

		//var regs = new Regs();
		//regs.W = d;           //inp w
		//regs.X = 0;            //mul x 0
		//regs.X += regs.Z;   //add x z
		//regs.X %= 26;          //mod x 26  ---> Z is always positive, >0
		//regs.Z /= f[0];       //div z AAAAA
		//regs.X -= f[1];           //add x -9
		//regs.X = regs.X == regs.W ? 1 : 0;    //eql x w
		//regs.X = regs.X == 0 ? 1 : 0;                         //eql x 0
		//regs.Y = 0;                                                        //mul y 0
		//regs.Y += 25;                                                          //add y 25
		//regs.Y *= regs.X;                                                           //mul y x
		//regs.Y += 1;                                                           //add y 1
		//regs.Z *= regs.Y;                                                          //mul z y
		//regs.Y = 0;                                                        //mul y 0
		//regs.Y += regs.W;                                                          //add y w
		//regs.Y += f[2];                                                        //add y 10
		//regs.Y *= regs.X;                                                          //mul y x
		//regs.Z += regs.Y;                                                          //add z y


		//inp w		  inp w		  inp w		inp w	  inp w		inp w	  inp w		inp w	  inp w		inp w	 inp w	   inp w	 inp w	   inp w
		//mul x 0	  mul x 0	  mul x 0	mul x 0	  mul x 0	mul x 0	  mul x 0	mul x 0	  mul x 0	mul x 0	 mul x 0   mul x 0	 mul x 0   mul x 0
		//add x z	  add x z	  add x z	add x z	  add x z	add x z	  add x z	add x z	  add x z	add x z	 add x z   add x z	 add x z   add x z
		//mod x 26	  mod x 26	  mod x 26	mod x 26  mod x 26	mod x 26  mod x 26	mod x 26  mod x 26	mod x 26 mod x 26  mod x 26	 mod x 26  mod x 26
		//div z 1	  div z 1	  div z 1	div z 1	  div z 1	div z 26  div z 1	div z 26  div z 26	div z 1	 div z 26  div z 26	 div z 26  div z 26   ---
		//add x 15	  add x 10	  add x 12	add x 10  add x 14	add x -11 add x 10	add x -16 add x -9	add x 11 add x -8  add x -8	 add x -10 add x -9   ---
		//eql x w	  eql x w	  eql x w	eql x w	  eql x w	eql x w	  eql x w	eql x w	  eql x w	eql x w	 eql x w   eql x w	 eql x w   eql x w
		//eql x 0	  eql x 0	  eql x 0	eql x 0	  eql x 0	eql x 0	  eql x 0	eql x 0	  eql x 0	eql x 0	 eql x 0   eql x 0	 eql x 0   eql x 0
		//mul y 0	  mul y 0	  mul y 0	mul y 0	  mul y 0	mul y 0	  mul y 0	mul y 0	  mul y 0	mul y 0	 mul y 0   mul y 0	 mul y 0   mul y 0
		//add y 25	  add y 25	  add y 25	add y 25  add y 25	add y 25  add y 25	add y 25  add y 25	add y 25 add y 25  add y 25	 add y 25  add y 25
		//mul y x	  mul y x	  mul y x	mul y x	  mul y x	mul y x	  mul y x	mul y x	  mul y x	mul y x	 mul y x   mul y x	 mul y x   mul y x
		//add y 1	  add y 1	  add y 1	add y 1	  add y 1	add y 1	  add y 1	add y 1	  add y 1	add y 1	 add y 1   add y 1	 add y 1   add y 1
		//mul z y	  mul z y	  mul z y	mul z y	  mul z y	mul z y	  mul z y	mul z y	  mul z y	mul z y	 mul z y   mul z y	 mul z y   mul z y
		//mul y 0	  mul y 0	  mul y 0	mul y 0	  mul y 0	mul y 0	  mul y 0	mul y 0	  mul y 0	mul y 0	 mul y 0   mul y 0	 mul y 0   mul y 0
		//add y w	  add y w	  add y w	add y w	  add y w	add y w	  add y w	add y w	  add y w	add y w	 add y w   add y w	 add y w   add y w
		//add y 13	  add y 16	  add y 2	add y 8	  add y 11	add y 6	  add y 12	add y 2	  add y 2	add y 15 add y 1   add y 10	 add y 14  add y 10  ----
		//mul y x	  mul y x	  mul y x	mul y x	  mul y x	mul y x	  mul y x	mul y x	  mul y x	mul y x	 mul y x   mul y x	 mul y x   mul y x
		//add z y	  add z y	  add z y	add z y	  add z y	add z y	  add z y	add z y	  add z y	add z y	 add z y   add z y	 add z y   add z y

		//inp a - Read an input value and write it to variable a.
		//add a b - Add the value of a to the value of b, then store the result in variable a.
		//mul a b - Multiply the value of a by the value of b, then store the result in variable a.
		//div a b - Divide the value of a by the value of b, truncate the result to an integer, then store the result in variable a. (Here, "truncate" means to round the value toward zero.)
		//mod a b - Divide the value of a by the value of b, then store the remainder in variable a. (This is also called the modulo operation.)
		//eql a b - If the value of a and b are equal, then store the value 1 in variable a. Otherwise, store the value 0 in variable a.


		protected override long Part2(string[] input)
		{


			return 0;
		}

	}
}
