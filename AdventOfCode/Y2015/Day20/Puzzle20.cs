using System;
using System.Diagnostics;

namespace AdventOfCode.Y2015.Day20
{
	internal class Puzzle20
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = 36000000;

			var target = input / 10;
			var houses = new int[target];
			for (var elf = 1; elf < target; elf++)
			{
				for (var i = elf; i < target; i += elf)
				{
					houses[i] += elf * 10;
				}
			}

			var house = Array.FindIndex(houses, v => v >= input);
			Console.WriteLine($"Day 20 Puzzle 1: {house}");
			Debug.Assert(house == 831600);
		}

		private static void Puzzle2()
		{
			var input = 36000000;

			var target = input / 11;
			var houses = new int[target];
			var deliveries = new int[target];
			for (var elf = 1; elf < target; elf++)
			{
				for (var i = elf; i < target && deliveries[elf]++ < 50; i += elf)
				{
					houses[i] += elf * 11;
				}
			}

			var house = Array.FindIndex(houses, v => v >= input);
			Console.WriteLine($"Day 20 Puzzle 2: {house}");
			Debug.Assert(house == 884520);
		}
	}
}


//static int FindLowestHouseReceivingNPresents(int input)
//{
//	//var target = input / 11;
//	//var start = (int)Math.Pow(2, (input / target - 1) * 2); // lower bound
//	//var delivered = new SafeDictionary<int, int>();
//	var delivered = new int[input];

//	for (var i = 1; ; i++)
//	{
//		if (CalcPresents(i) >= input)
//		{
//			return i;
//		}
//	}

//	int CalcPresents(int n)
//	{
//		var sum = 0;
//		var sqrt = (int)Math.Floor(Math.Sqrt(n));
//		for (var i = 1; i < sqrt; i++)
//		{
//			if (n % i == 0)
//			{
//				if (delivered[i]++ < 50) sum += i;
//				if (delivered[n / i]++ < 50) sum += n / i;
//			}
//		}
//		if (sqrt * sqrt == n)
//		{
//			sum += sqrt;
//		}
//		return sum * 11;
//	}
//}

//static int FindLowestHouseReceivingNPresents(int input)
//{
//	var target = input / 10;
//	var start = (int)Math.Pow(2, (input / target - 1) * 2); // lower bound
//	for (var i = start; ; i++)
//	{
//		if (NumberOfPresents(i) >= input)
//		{
//			return i;
//		}
//	}

//	static int NumberOfPresents(int n)
//	{
//		var sum = 0;
//		var sqrt = (int)Math.Floor(Math.Sqrt(n));
//		for (var i = 1; i < sqrt; i++)
//		{
//			if (n % i == 0)
//			{
//				sum += i + n / i;
//			}
//		}
//		if (sqrt * sqrt == n)
//		{
//			sum += sqrt;
//		}
//		return sum * 10;
//	}
//}




//IEnumerable<int> SumOfDivisors()
//{
//	var sumOfDivisors = new List<int>();
//	sumOfDivisors.Add(0);
//	sumOfDivisors.Add(1);
//	yield return 1;
//	for (var n = 2; ; n++)
//	{
//		var sum = CalcSumOfDivisorsSmart(n);
//		sumOfDivisors.Add(sum);
//		yield return sum;
//	}

//	int CalcSumOfDivisorsSmart(int n)
//	{
//		if (n % 2 == 0)
//		{
//			var reused = n / 2;
//			var sum = n + sumOfDivisors[reused];
//			if (n >= 6)
//			{
//				if (n % 4 != 0)
//					sum += 2;
//				for (var i = Math.Max(3, (int)Math.Ceiling(Math.Sqrt(reused))); i < reused; i++)
//				{
//					if (n % i == 0 && reused % i != 0)
//					{
//						sum += i;
//					}
//				}
//			}

//			return sum;
//		}
//		return CalcSumOfDivisors(n);
//	}
//}