//using System;
//using System.Linq;

//namespace AdventOfCode2019.NBody
//{
//    internal class Moon
//    {
//		public Moon(int x, int y, int z)
//		{
//			V = new Vector[]
//			{
//				new Vector { Pos = x, Velocity = 0 },
//				new Vector { Pos = y, Velocity = 0 },
//				new Vector { Pos = z, Velocity = 0 }
//			};
//		}

//		public readonly Vector[] V;

//		public static Moon ParseFrom(string s)
//		{
//			var pos = s.Split(',')
//				.Select(x => x.Trim("<=, >xyz".ToArray()))
//				.Select(int.Parse)
//				.ToArray();
//			return new Moon(pos[0], pos[1], pos[2]);
//		}

//		public void ApplyGravityFrom(Moon otherMoon)
//		{
//			for (int i = 0; i < V.Length; i++)
//			{
//				V[i].Velocity += VelocityDelta(V[i].Pos, otherMoon.V[i].Pos);
//			}
//			int VelocityDelta(int p1, int p2) => p1 == p2 ? 0 : p1 < p2 ? 1 : -1;
//		}

//		public void ApplyVelocity()
//		{
//			foreach (var v in V)
//			{
//				v.Pos += v.Velocity;
//			}
//		}

//		public int PotentialEnergy => V.Sum(v => Math.Abs(v.Pos));
//		public int KineticEnergy => V.Sum(v => Math.Abs(v.Velocity));
//		public int TotalEnergy => PotentialEnergy * KineticEnergy;

//		public bool IsSamePosition(Moon other) =>
//			V[0].Pos == other.V[0].Pos &&
//			V[1].Pos == other.V[1].Pos &&
//			V[2].Pos == other.V[2].Pos &&
//			V[0].Velocity == other.V[0].Velocity &&
//			V[1].Velocity == other.V[1].Velocity &&
//			V[2].Velocity == other.V[2].Velocity;
//	}
//}




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

		public Moon Copy()
		{
			return new Moon(X, Y, Z)
			{
				Vx = Vx,
				Vy = Vy,
				Vz = Vz
			};
		}

		public void ApplyGravityFrom(Moon otherMoon)
		{
			//Vx += VelocityDelta(X, otherMoon.X);
			//Vy += VelocityDelta(Y, otherMoon.Y);
			//Vz += VelocityDelta(Z, otherMoon.Z);

			Vx += X == otherMoon.X ? 0 : X < otherMoon.X ? 1 : -1;
			Vy += Y == otherMoon.Y ? 0 : Y < otherMoon.Y ? 1 : -1;
			Vz += Z == otherMoon.Z ? 0 : Z < otherMoon.Z ? 1 : -1;

			//int VelocityDelta(int p1, int p2) => p1 == p2 ? 0 : p1 < p2 ? 1 : -1;
		}

		public void ApplyVelocity()
		{
			X += Vx;
			Y += Vy;
			Z += Vz;
		}

		public int PotentialEnergy => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
		public int KineticEnergy => Math.Abs(Vx) + Math.Abs(Vy) + Math.Abs(Vz);
		public int TotalEnergy => PotentialEnergy * KineticEnergy;

		//public bool IsSamePosition(Moon other) =>
		//	X == other.X &&
		//	Y == other.Y &&
		//	Z == other.Z &&
		//	Vx == other.Vx &&
		//	Vy == other.Vy &&
		//	Vz == other.Vz;

		public bool IsSamePosition(Moon other) =>
			X == other.X &&
			Y == other.Y &&
			Z == other.Z;
	}
}





//using System;
//using System.Linq;

//namespace AdventOfCode2019.NBody
//{
//	internal class Moon
//	{
//		public Moon(int x, int y, int z)
//		{
//			Position = new Position { X = x, Y = y, Z = z };
//			Velocity = new Velocity { Vx = 0, Vy = 0, Vz = 0 };
//		}
//		public Position Position { get; }
//		public Velocity Velocity { get; }

//		public static Moon ParseFrom(string s)
//		{
//			var pos = s.Split(',')
//				.Select(x => x.Trim("<=, >xyz".ToArray()))
//				.Select(int.Parse)
//				.ToArray();
//			return new Moon(pos[0], pos[1], pos[2]);
//		}

//		public void ApplyGravityFrom(Moon otherMoon)
//		{
//			Velocity.Vx += VelocityDelta(Position.X, otherMoon.Position.X);
//			Velocity.Vy += VelocityDelta(Position.Y, otherMoon.Position.Y);
//			Velocity.Vz += VelocityDelta(Position.Z, otherMoon.Position.Z);
//			int VelocityDelta(int p1, int p2) => p1 == p2 ? 0 : p1 < p2 ? 1 : -1;
//		}

//		public void ApplyVelocity()
//		{
//			Position.X += Velocity.Vx;
//			Position.Y += Velocity.Vy;
//			Position.Z += Velocity.Vz;
//		}

//		public int PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);
//		public int KineticEnergy => Math.Abs(Velocity.Vx) + Math.Abs(Velocity.Vy) + Math.Abs(Velocity.Vz);
//		public int TotalEnergy => PotentialEnergy * KineticEnergy;

//		public bool IsSamePosition(Moon other) =>
//			Position.X == other.Position.X &&
//			Position.Y == other.Position.Y &&
//			Position.Z == other.Position.Z &&
//			Velocity.Vx == other.Velocity.Vx &&
//			Velocity.Vy == other.Velocity.Vy &&
//			Velocity.Vz == other.Velocity.Vz;
//	}
//}
