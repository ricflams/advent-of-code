using System;
using System.Linq;

namespace AdventOfCode2019.NBody
{
	internal class Moon
	{
		public Moon(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
			Vx = Vy = Vz = 0;
		}
		public int X;
		public int Y;
		public int Z;
		public int Vx;
		public int Vy;
		public int Vz;

		public static Moon ParseFrom(string s)
		{
			var pos = s.Split(',')
				.Select(x => x.Trim("<=, >xyz".ToArray()))
				.Select(int.Parse)
				.ToArray();
			return new Moon(pos[0], pos[1], pos[2]);
		}

		public void ApplyGravityFrom(Moon otherMoon)
		{
			Vx += X == otherMoon.X ? 0 : X < otherMoon.X ? 1 : -1;
			Vy += Y == otherMoon.Y ? 0 : Y < otherMoon.Y ? 1 : -1;
			Vz += Z == otherMoon.Z ? 0 : Z < otherMoon.Z ? 1 : -1;
		}

		public void ApplyVelocity()
		{
			X += Vx;
			Y += Vy;
			Z += Vz;
		}

		public override string ToString() => $"p=({X},{Y},{Z}) v=({Vx},{Vy},{Vz}) Ep={PotentialEnergy} Ek={KineticEnergy}";

		public int PotentialEnergy => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
		public int KineticEnergy => Math.Abs(Vx) + Math.Abs(Vy) + Math.Abs(Vz);
		public int TotalEnergy => PotentialEnergy * KineticEnergy;
	}
}
