using System.Linq;

namespace AdventOfCode.Y2017.Common
{
    public static class KnotHash
    {
   		public static byte[] Hash(string message)
		{
			const int N = 256;
			var lengths = message
					.Select(c => (byte)c)
					.Concat(new byte[] { 17, 31, 73, 47, 23 })
					.ToArray();

			var pos = 0;
			var skip = 0;
			var list = Enumerable.Range(0, N).Select(x => (byte)x).ToArray();
			for (var round = 0; round < 64; round++)
			{
				foreach (var len in lengths)
				{
					// Reverse len
					for (var i = 0; i < len/2; i++)
					{
						var a = (pos+i) % N;
						var b = (pos+len-1-i) % N;
						(list[a], list[b]) = (list[b], list[a]);
					}
					pos += len + skip++;
				}
			}

			var densehash = new byte[16];
			for (var i = 0; i < 16; i++)
			{
				var block = i*16;
				densehash[i] = list[block];
				for (var j = 1; j < 16; j++)
				{
					densehash[i] ^= list[block+j];
				}
			}

			return densehash;
		}
    }
}