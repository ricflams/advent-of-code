using System;

namespace AdventOfCode.Helpers.Arrays
{
    public static class Extension
    {
        public static T[] Move<T>(this T[] a, int from, int to)
        {
            var v = a[from];
            if (from < to)
            {
                Array.Copy(a, from+1, a, from, to-from);
            }
            else if (from > to)
            {
                Array.Copy(a, to, a, to+1, from-to);
            }
            a[to] = v;
            return a;
        }
    }
}