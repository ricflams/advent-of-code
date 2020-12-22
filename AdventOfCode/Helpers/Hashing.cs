using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode.Helpers
{
	public static class Hashing
	{
		public static readonly MD5 _md5 = MD5.Create();

		public static ulong LongHash(string s) => LongHash(Encoding.Default.GetBytes(s));
		public static ulong LongHash(int[] ia) => LongHash(ia.SelectMany(BitConverter.GetBytes).ToArray());
		public static ulong LongHash(byte[] data)
		{
			return _md5.ComputeHash(data).Select(b => (ulong)b).Aggregate((s, v) => 3074457345618258799ul * s + v);
		}

		public static uint Hash(string s) => Hash(Encoding.Default.GetBytes(s));
		public static uint Hash(int[] ia) => Hash(ia.SelectMany(BitConverter.GetBytes).ToArray());
		public static uint Hash(byte[] data)
		{
			var longhash = LongHash(data);
			var hash = (uint)(longhash >> 32 ^ longhash & 0xffffffff);
			return hash;
		}

		public static ulong KnuthHash(string s) => Hash(Encoding.Default.GetBytes(s));
		public static ulong KnuthHash(int[] ia) => Hash(ia.SelectMany(BitConverter.GetBytes).ToArray());
		public static ulong KnuthHash(IEnumerable<byte> bytes)
		{
			var hashedValue = 3074457345618258791UL;
			foreach (var b in bytes)
			{
				hashedValue += b;
				hashedValue *= 3074457345618258799UL;
			}
			return hashedValue;
		}

		public static uint JenkinsHash(string s) => Hash(Encoding.Default.GetBytes(s));
		public static uint JenkinsHash(int[] ia) => Hash(ia.SelectMany(BitConverter.GetBytes).ToArray());
		public static uint JenkinsHash(IEnumerable<byte> bytes)
		{
			var hash = 0U;
			foreach (var b in bytes)
			{
				hash += b;
				hash += hash << 10;
				hash ^= hash >> 6;
			}
			hash += hash << 3;
			hash ^= hash >> 11;
			hash += hash << 15;
			return hash;
		}
	}
}
