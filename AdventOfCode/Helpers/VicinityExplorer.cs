using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class VicinityExplorer
	{
		private readonly HashSet<uint> _produced = new HashSet<uint>();

		public IEnumerable<int[]> Explore(int[] positions)
		{
			var N = positions.Length;
			for (var aim = 0; aim < N; aim++)
			{
				for (var i = 0; i < N; i++)
				{
					if (i == aim)
					{
						continue;
					}
					var explore = positions.ToArray();
					explore[aim]++;
					explore[i]--;
					var id = Hashing.Hash(explore);
					if (_produced.Contains(id))
					{
						continue;
					}
					_produced.Add(id);
					yield return explore;
				}
			}
		}
	}
}
