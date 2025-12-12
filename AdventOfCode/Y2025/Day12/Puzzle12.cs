using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace AdventOfCode.Y2025.Day12
{
    internal class Puzzle : Puzzle<long, long>
    {
        public static Puzzle Instance = new();
        public override string Name => "";
        public override int Year => 2025;
        public override int Day => 12;

        public override void Run()
        {
            Run("test1").Part1(2).Part2(0);
            		Run("input").Part1(0).Part2(0);
            //		Run("extra").Part1(1690).Part2(221371496188107);
        }

        protected override long Part1(string[] input)
        {
            var parts = input.GroupByEmptyLine().ToArray();
            var presents = parts[..^1]
                .Select(Present.Parse)
                .ToArray();
            var regions = parts.Last()
                .Select(Region.Parse)
                .ToArray();

            var fits = regions.Count(r => WillFit(r, presents));

            return fits;
        }

        private static bool WillFit(Region region, Present[] presents)
        {
            var area = new bool[region.Width, region.Height];
            var ps = region.Quantities
                .SelectMany((q, index) => Enumerable.Repeat(presents[index], q))
                .Select(p => (Present: p, SpaceNeeded: -1))
                .ToArray();

            var spaceNeeded = 0;
            for (var i = ps.Length; i-- > 0;)
            {
                spaceNeeded += ps[i].Present.Occupies;
                ps[i].SpaceNeeded = spaceNeeded;
            }
            var totalSpace = region.Width * region.Height;

            return WillFit(0, 0);

            bool WillFit(int i, int occupied)
            {
                if (i == ps.Length)
                {
                    Console.WriteLine("bingo!");
                    return true;
                }

                var (present, spaceNeeded) = ps[i];

                if (occupied + spaceNeeded > totalSpace)
                    return false;

                for (var x = 0; x <= region.Width - 3; x++)
                {
                    for (var y = 0; y <= region.Height - 3; y++)
                    {
                        foreach (var shape in present.Shapes)
                        {
                            //Console.WriteLine();
                            //Console.WriteLine($"i={i} x={x} y={y}");
                            //area.WriteConsole();
                            //shape.Area.WriteConsole();

                            if (IsVacant(x, y, shape))
                            {
                                Set(x, y, shape, true);
                                area.WriteConsole();
                                var willFit = WillFit(i + 1, occupied + shape.Occupies);
                                Set(x, y, shape, false);
                                area.WriteConsole();
                                if (willFit)
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }

            bool IsVacant(int x, int y, Shape shape)
            {
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        if (shape.Area[i, j] && area[x + i, y + j])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            void Set(int x, int y, Shape shape, bool val)
            {
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        if (shape.Area[i, j])
                            area[x + i, y + j] = val;
                    }
                }
            }


        }


        private record Present(Shape[] Shapes, int Occupies)
        {
            public static Present Parse(string[] lines)
            {
                var m = CharMatrix.FromArray(lines[1..]);
                var variants = new HashSet<Shape>();
                for (var i = 0; i < 4; i++)
                {
                    variants.Add(new Shape(m));
                    variants.Add(new Shape(m.FlipH()));
                    variants.Add(new Shape(m.FlipV()));
                    m = m.RotateClockwise(90);
                }
                var v = variants.ToArray();

                return new Present(v, v.First().Occupies);
            }
        }

        public class Shape
        {
            private static readonly int N = 3; // They're all 3x3

            public bool[,] Area { get; init; }
            public int Occupies { get; init; }

            private readonly int _hashCode;

            public Shape(char[,] map)
            {
                Area = new bool[N, N];
                for (var i = 0; i < N; i++)
                {
                    for (var j = 0; j < N; j++)
                    {
                        if (map[i, j] == '#')
                        {
                            Area[i, j] = true;
                            Occupies++;
                            _hashCode++;

                        }
                        _hashCode <<= 1;
                    }
                }
            }

            public override int GetHashCode() => _hashCode;

            public override bool Equals(object obj)
            {
                if (!(obj is Shape s))
                    return false;
                for (var i = 0; i < N; i++)
                {
                    for (var j = 0; j < N; j++)
                    {
                        if (Area[i, j] != s.Area[i, j])
                            return false;
                    }
                }
                return true;
            }
        }

        private record Region(int Width, int Height, int[] Quantities)
        {
            public static Region Parse(string line)
            {
                var parts = line.Split(':').ToArray();
                var (width, height) = parts[0].RxMatch("%dx%d").Get<int, int>();
                var quantities = parts[1].SplitSpace().Select(int.Parse).ToArray();
                return new Region(width, height, quantities);
            }
        }

        protected override long Part2(string[] input)
        {
            return 0;
        }
    }


    public static class Extensions
    {
        public static void WriteConsole(this bool[,] map)
        {
            //var (w, h) = map.Dim();
            //for (var y = 0; y < h; y++)
            //{
            //    for (var x = 0; x < w; x++)
            //    {
            //        Console.Write(map[x, y] ? '#' : '.');
            //    }
            //    Console.WriteLine();
            //}
        }
    }


}
		