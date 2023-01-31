using System;

namespace AdventOfCode.Helpers
{
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	public record Point3D(long X, long Y, long Z)
	{
		public static readonly Point3D Origin = new Point3D(0, 0, 0);

		public long ManhattanDistanceTo(Point3D o) => Math.Abs(X - o.X) + Math.Abs(Y - o.Y) + Math.Abs(Z - o.Z);

		public override string ToString() => $"<{X},{Y},{Z}>";

		public static Point3D operator +(Point3D a, Point3D b) => new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		public static Point3D operator -(Point3D a, Point3D b) => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		public static Point3D operator *(Point3D p, double f) => new Point3D((long)Math.Round(p.X * f), (long)Math.Round(p.Y * f), (long)Math.Round(p.Z * f));

		public long Dot(Point3D p) => X * p.X + Y * p.Y + Z * p.Z;
		public Point3D Cross(Point3D p) => new Point3D(Y * p.Z - Z * p.Y, Z * p.X - X * p.Z, X * p.Y - Y * p.X);

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
	}
}
