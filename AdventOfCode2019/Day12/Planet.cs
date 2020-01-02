using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.NBody
{
    internal class Planet
    {
		private readonly Moon[] _moons;

		public Planet(params Moon[] moons)
		{
			_moons = moons;
			SimulationStep = 0;
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

			SimulationStep++;
		}

		public long SimulationStep { get; private set; }

		public int TotalEnergy => _moons.Sum(m => m.TotalEnergy);
		public int PotentialEnergy => _moons.Sum(m => m.PotentialEnergy);
		public int KineticEnergy => _moons.Sum(m => m.KineticEnergy);

		// For puzzle2
		public IEnumerable<int> XVectors => _moons.Select(x => x.X).Concat(_moons.Select(x => x.Vx));
		public IEnumerable<int> YVectors => _moons.Select(x => x.Y).Concat(_moons.Select(x => x.Vy));
		public IEnumerable<int> ZVectors => _moons.Select(x => x.Z).Concat(_moons.Select(x => x.Vz));
	}
}
