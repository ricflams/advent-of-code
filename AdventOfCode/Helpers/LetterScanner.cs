using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Helpers
{
	public static class LetterScanner
	{
        // Taken from https://www.reddit.com/r/adventofcode/comments/5h52ro/2016_day_8_solutions/daxv8cr/
        private static readonly Dictionary<uint,char> LetterMap = new Dictionary<uint, char>
        {
          {0x19297A52, 'A'}, {0x392E4A5C, 'B'}, {0x1928424C, 'C'},
          {0x39294A5C, 'D'}, {0x3D0E421E, 'E'}, {0x3D0E4210, 'F'},
          {0x19285A4E, 'G'}, {0x252F4A52, 'H'}, {0x1C42108E, 'I'},
          {0x0C210A4C, 'J'}, {0x254C5292, 'K'}, {0x2108421E, 'L'},
          {0x19294A4C, 'O'}, {0x39297210, 'P'}, {0x39297292, 'R'},
          {0x1D08305C, 'S'}, {0x1C421084, 'T'}, {0x25294A4C, 'U'},
          {0x23151084, 'Y'}, {0x3C22221E, 'Z'}
        };


        // Examples:
        // ███  ████ ████ █  █ ███
        // █  █    █ █    █ █  █  █
        // █  █   █  ███  ██   ███
        // ███   █   █    █ █  █  █
        // █    █    █    █ █  █  █
        // █    ████ ████ █  █ ███
        //
        //    ██ ████ ███  ████ ███  ███  █  █ █  █
        //     █ █    █  █ █    █  █ █  █ █  █ █  █
        //     █ ███  ███  ███  █  █ ███  █  █ ████
        //     █ █    █  █ █    ███  █  █ █  █ █  █
        //  █  █ █    █  █ █    █ █  █  █ █  █ █  █
        //   ██  █    ███  ████ █  █ ███   ██  █  █

        public static string Scan(char [,] matrix)
		{
			return Scan(matrix.ToStringArray());
		}

        public static string Scan(IEnumerable<string> raw)
        {
            var lines = raw.ToArray();

            try
            {
                // Sanity-check height and similar lengths
                if (lines.Length != 6)
                {
                    throw new Exception("Letters must be exactly 6 chars high");
                }
                if (!lines.Skip(1).All(x => x.Length == lines[0].Length))
                {
                    throw new Exception("Letter-lines must be same length");
                }

                // Skip leading spaces, and also trailing spaces if length is too long
                while (lines.All(x => x.First() == ' '))
                {
                    lines = lines.Select(x => x[1..]).ToArray();
                }
                while (lines.All(x => x.Last() == ' ') && lines[0].Length % 5 != 0)
                {
                    lines = lines.Select(x => x[..^1]).ToArray();
                }
                if (lines.Any(x => x.Last() != ' ') && lines[0].Length % 5 == 4)
                {
                    lines = lines.Select(x => x + " ").ToArray();
                }                

                // After trimming spaces, make sure letters are 5 chars wide
                if (lines[0].Length % 5 != 0)
                {
                    throw new Exception($"Width {lines[0].Length} can't be scanned; letters must be 5 chars wide each");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(Scan)} failed: {ex.Message}");
                ConsoleWrite(raw);
                throw;
            }

            // Parse into string
            var n = lines[0].Length / 5;
            var letters = Enumerable.Range(0, n).Select(ParseLetter).ToArray();
            return new string(letters);

            char ParseLetter(int index)
            {
                var id = lines.Aggregate(0U, (sum, line) => sum = (sum << 5) + ReadLineVal(line, index * 5));
                return LetterMap[id];
                static uint ReadLineVal(string s, int x) => Dot(s[x], 16) + Dot(s[x + 1], 8) + Dot(s[x + 2], 4) + Dot(s[x + 3], 2) + Dot(s[x + 4], 1);
                static uint Dot(char ch, uint val) => ch == '#' ? val : 0;
            }
        }

        public static void ConsoleWrite(IEnumerable<string> raw)
        {
            var lines = raw.ToArray();
            var boxlen = lines[0].Length + 2;
            Console.WriteLine(new string('v', boxlen));
            foreach (var line in lines)
            {
                Console.WriteLine($">{line}<");
            }
            Console.WriteLine(new string('^', boxlen));
        }
    }
}
