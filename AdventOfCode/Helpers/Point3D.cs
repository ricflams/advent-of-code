using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	public record Point3D(int X, int Y, int Z)
	{
		public static readonly Point3D Origin = new(0, 0, 0);

		static public Point3D Parse(string s)
		{
			var v = s.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
			return new Point3D(v[0], v[1], v[2]);
		}

		public int ManhattanDistanceTo(Point3D o) => Math.Abs(X - o.X) + Math.Abs(Y - o.Y) + Math.Abs(Z - o.Z);

		public override string ToString() => $"<{X},{Y},{Z}>";
		public override int GetHashCode() => HashCodeValue;
		public int HashCodeValue = HashCode.Combine(X, Y, Z);

		public static Point3D operator +(Point3D a, Point3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		public static Point3D operator -(Point3D a, Point3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		public static Point3D operator *(Point3D p, double f) => new((int)Math.Round(p.X * f), (int)Math.Round(p.Y * f), (int)Math.Round(p.Z * f));

		public long Dot(Point3D p) => X * p.X + Y * p.Y + Z * p.Z;
		public Point3D Cross(Point3D p) => new(Y * p.Z - Z * p.Y, Z * p.X - X * p.Z, X * p.Y - Y * p.X);

		public static Point3D IntersectPoint(Point3D lineVector, Point3D linePoint, Point3D planeNormal, Point3D planePoint)
		{
			var diff = linePoint - planePoint;
			var prod1 = diff.Dot(planeNormal);
			var prod2 = lineVector.Dot(planeNormal);
			if (prod2 == 0)
				return null;
			var prod3 = (double)prod1 / prod2;
			return linePoint - lineVector * prod3;
		}

		public IEnumerable<Point3D> Rotate()
		{
			foreach (var p in new Point3D(Y, Z, X).RotateXy()) yield return p;
			foreach (var p in new Point3D(-Y, Z, -X).RotateXy()) yield return p;
			foreach (var p in new Point3D(Z, X, Y).RotateXy()) yield return p;
			foreach (var p in new Point3D(-Z, X, -Y).RotateXy()) yield return p;
			foreach (var p in new Point3D(X, Y, Z).RotateXy()) yield return p;
			foreach (var p in new Point3D(-X, Y, -Z).RotateXy()) yield return p;
		}
		private IEnumerable<Point3D> RotateXy()
		{
			yield return new Point3D(X, Y, Z);
			yield return new Point3D(-Y, X, Z);
			yield return new Point3D(-X, -Y, Z);
			yield return new Point3D(Y, -X, Z);
		}

		public IEnumerable<Point3D> Neighbors => Displacements.Select(d => this + d);
		private static readonly Point3D[] Displacements = new[] { new Point3D(-1, 0, 0), new Point3D(1, 0, 0), new Point3D(0, -1, 0), new Point3D(0, 1, 0), new Point3D(0, 0, -1), new Point3D(0, 0, 1) };
	}
}
