using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day19
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Beacon Scanner";
		public override int Year => 2021;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1(79).Part2(3621);
			Run("input").Part1(403).Part2(10569);
			Run("extra").Part1(308).Part2(12124);
			Run("extra").Part1(308).Part2(12124);
		}

		protected override long Part1(string[] input)
		{
			// Align all scanner's beacons and count the total using a hashset
			var scanners = Scanner.AlignScanners(input);
			var beacons = new HashSet<Point3D>(scanners.SelectMany(s => s.AlignedBeacons.Beacons));
			return beacons.Count;
		}

		protected override long Part2(string[] input)
		{
			// Align all scanner's beacons and find the largest distance between any two
			var scanners = Scanner.AlignScanners(input);
			var maxdist = scanners.Max(a => scanners.Max(b => a.Offset.ManhattanDistanceTo(b.Offset)));
			return maxdist;
		}


		internal class Scanner
		{
			public Scanner(Point3D[] beacons)
			{
				Beacons = new BeaconSet(beacons);
				AlignedBeacons = Beacons;
				RotatedBeacons = BeaconsRot().ToArray();

				// Pre-calculate all 24 possible rotations to make comparisons fast
				IEnumerable<BeaconSet> BeaconsRot()
				{
					var rotPoints = Beacons.Beacons.Select(b => b.Rotate().ToArray()).ToArray();
					for (var i = 0; i < 24; i++)
					{
						var beacons = rotPoints.Select(rp => rp[i]).ToArray();
						yield return new BeaconSet(beacons);
					}
				}
			}

			private BeaconSet Beacons { get; init; }
			private BeaconSet[] RotatedBeacons { get; init; }
			public BeaconSet AlignedBeacons { get; private set; }
			public Point3D Offset { get; private set; } = Point3D.Origin;

			public class BeaconSet
			{
				internal BeaconSet(IEnumerable<Point3D> beacons)
				{
					// Pre-calculate the diff between all beacons for faster beacon-set matching
					Beacons = beacons.ToArray();
					for (var i = 0; i < Beacons.Length; i++)
						for (var j = i + 1; j < Beacons.Length; j++)
							Diffs.Add(Beacons[j] - Beacons[i]);
				}
				internal readonly Point3D[] Beacons;
				internal readonly HashSet<Point3D> Diffs = new();
			}

			public static List<Scanner> AlignScanners(string[] input)
			{
				var unaligned = input
					.GroupByEmptyLine()
					.Select(lines => lines
						.Skip(1)
						.Select(Point3D.Parse)
						.ToArray()
					)
					.Select(x => new Scanner(x))
					.ToHashSet();

				// The approach is:
				// Just decide the first scanner is aligned correctly; that's the "aligned" set
				// The check all the unaligned scanners against this first aligned scanner
				// Any matching scanners are now aligned and they becomes the next batch of scanners to check against
				// (there's no need to re-check against all aligned scanners since they apparently didn't
				//  match any of the remaining unaligned scanners anyway)
				var checking = new HashSet<Scanner> { unaligned.First() };
				unaligned.Remove(checking.First());
				var aligned = new List<Scanner>();
				while (unaligned.Any())
				{
					var found = unaligned.Where(s => checking.Any(x => s.Match12Beacons(x))).ToHashSet();
					unaligned.ExceptWith(found);
					aligned.AddRange(checking);
					checking = found;
				}
				aligned.AddRange(checking);

				return aligned;
			}

			private bool Match12Beacons(Scanner world)
			{
				var truth = world.AlignedBeacons;
				foreach (var rotation in RotatedBeacons)
				{
					// If no two beacons in both sets are positioned exactly the same way relative
					// to each other then we can be sure this rotation doesn't match. This check is
					// really cheap to make before next checking how many beacons may align.
					if (!rotation.Diffs.Overlaps(truth.Diffs))
						continue;

					// Check how many beacon-pairs in each set have the exact same diff/vector; if
					// there's less than 12 then we know for sure there won't be 12 matching beacons.
					var matches = rotation.Diffs.Intersect(truth.Diffs).Count();
					if (matches < 12)
						continue;

					// Find the offset between all combinations of beacons and stop when we find some
					// offset that occur 12 times; that means this rotation is right. So calculate the
					// aligned beacons by shifting this rotation into the right position.
					var offsets = new SafeDictionary<Point3D, int>();
					foreach (var a in rotation.Beacons)
					{
						foreach (var b in truth.Beacons)
						{
							var offset = b - a;
							var n = ++offsets[offset];
							if (n == 12)
							{
								AlignedBeacons = new BeaconSet(rotation.Beacons.Select(p => p + offset));
								Offset = offset;
								return true;
							}
						}
					}
				}
				return false;
			}
		}
	}
}
