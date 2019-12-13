using System.Linq;

namespace AdventOfCode2019.NBody
{
    internal class Planet
    {
		private readonly Moon[] _moons;

		public Planet(params Moon[] moons)
		{
			_moons = moons;
		}

		public void SimulateMotionStep()
		{
			// Apply gravity to change the velocity of each pair
			foreach (var m1 in _moons)
			{
				foreach (var m2 in _moons) //.Where(m => m != m1))
				{
					if (m1 != m2)
					{
						m1.ApplyGravityFrom(m2);
					}
				}
			}

			// Apply the velocity to change position
			foreach (var m in _moons)
			{
				m.ApplyVelocity();
			}
		}

		public int TotalEnergy => _moons.Sum(m => m.TotalEnergy);
		public int PotentialEnergy => _moons.Sum(m => m.PotentialEnergy);
		public int KineticEnergy => _moons.Sum(m => m.KineticEnergy);
	}
}
