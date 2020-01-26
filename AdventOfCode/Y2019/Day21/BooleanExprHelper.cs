using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Y2019.Day21
{
    internal static class BooleanExprHelper
    {
		internal static IEnumerable<(string, string)> Compare(string a, string b)
		{
			// Format is eg: ~a + ~e~f~g + ~cd~f + ~b~cd~e + b~cd~efh

			var inputs = a.Where(c => !" ~+()".Contains(c)).Distinct().OrderBy(c => c).ToArray();
			for (var mask = 0; mask < 1<<inputs.Length; mask++)
			{
				var va = a.ToString();
				var vb = b.ToString();
				for (var i = 0; i < inputs.Length; i++)
				{
					var value = (1<<i & mask) != 0 ? '1' : '0';
					va = va.Replace(inputs[i], value);
					vb = vb.Replace(inputs[i], value);
				}
				var ea = Evaluate(va);
				var eb = Evaluate(vb);
				if (ea != eb)
				{
					yield return (va, vb);
				}
			}

			bool Evaluate(string boolExpr)
			{
				// Examples:
				// ~a + ~e~f~g + ~cd~f + ~b~cd~e + b~cd~efh
				// ~a + ~e~f~g + ~cd(~f + ~b~e + b~efh)
				// ~(a (e+f+g) (c + d + (f (e + (b ~(bfh)))))
				var s = boolExpr.Replace(" ", "");
				if (s.Count(c => c == '(') != s.Count(c => c == ')'))
				{
					throw new Exception($"Mismatched parenthesis in {boolExpr}");
				}
				while (s.Length > 1)
				{
					s = s
						.Replace("(0)", "0")
						.Replace("(1)", "1");
					s = s
						.Replace("~0", "1")
						.Replace("~1", "0");
					while (true)
					{
						var o = s;
						s = s
							.Replace("00", "0")
							.Replace("11", "1")
							.Replace("01", "0")
							.Replace("10", "0");
						if (o == s)
							break;
					}

					s = s
						.Replace("0+0)", "0)")
						.Replace("0+1)", "1)")
						.Replace("1+0)", "1)")
						.Replace("1+1)", "1)")
						.Replace("0+0+", "0+")
						.Replace("0+1+", "1+")
						.Replace("1+0+", "1+")
						.Replace("1+1+", "1+")
						;
					if (s.Length == 3)
					{
						s = s
							.Replace("0+0", "0")
							.Replace("0+1", "1")
							.Replace("1+0", "1")
							.Replace("1+1", "1");
					}
				}
				return s == "1";
			}
		}
	}
}
