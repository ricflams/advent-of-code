using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Day20
{
	internal class PortalMaze : Maze
	{
		public class Portal
		{
			public string Name { get; set; }
			public Point Pos { get; set; }
			public bool IsDownward { get; set; }
		}

		public PortalMaze(string filename)
			: this(ReadMapFromFile(filename))
		{
		}

		public PortalMaze(CharMap map)
		{
			Map = map;

			var portalinfo = Map.AllPoints(char.IsUpper).OrderBy(p => p.Y).ThenBy(p => p.X);
			var portalsByName = new Dictionary<string, List<Tuple<Point, Point>>>();

			// Map all entry-portals
			foreach (var p in portalinfo)
			{

				if (Map[p.Down] == '.')
				{
					//       X
					// p ->  Y  <- exit
					//       .  <- arrival
					AddPortal(p.Up, p, p, p.Down);
				}
				else if (Map[p.Up] == '.')
				{
					//       .  <- arrival
					// p ->  X  <- exit
					//       Y
					AddPortal(p, p.Down, p, p.Up);
				}
				//};

				//// Next find the portal exits and match with their entry
				//foreach (var p in portalinfo)
				//{
				else if (Map[p.Right] == '.')
				{
					// exit vv arrival 
					//     XY.
					//      ^ p
					AddPortal(p.Left, p, p, p.Right);
				}
				else if (Map[p.Left] == '.')
				{
					// arrival vv exit
					//         .XY
					//          ^ p
					AddPortal(p, p.Right, p, p.Left);
				}
			};

			// Link up portals
			Entry = portalsByName["AA"].First().Item2;
			Exit = portalsByName["ZZ"].First().Item2;
			Map[portalsByName["AA"].First().Item1] = '#';
			Map[portalsByName["ZZ"].First().Item1] = '#';
			ExternalMapPoints = new Point[] { Exit };

			Portals = new SparseMap<Portal>();
			var area = Map.Area();
			foreach (var pair in portalsByName.Where(p => p.Key != "AA" && p.Key != "ZZ"))
			{
				var p1 = pair.Value[0];
				var p2 = pair.Value[1];
				Portals[p1.Item1] = new Portal
				{
					Name = pair.Key,
					Pos = p2.Item2,
					IsDownward = !IsOuterPortal(p1.Item1)
				};
				Portals[p2.Item1] = new Portal
				{
					Name = pair.Key,
					Pos = p1.Item2,
					IsDownward = !IsOuterPortal(p2.Item1)
				};
			}

			bool IsOuterPortal(Point p) => p.X < 4 || p.X > area.Item2.X - 4 || p.Y < 4 || p.Y > area.Item2.Y - 4;

			void AddPortal(Point pos1, Point pos2, Point departure, Point arrival)
			{
				var name = new string(new char[] { Map[pos1], Map[pos2] });
				if (!portalsByName.TryGetValue(name, out var pn))
				{
					portalsByName[name] = new List<Tuple<Point, Point>>();
				}
				portalsByName[name].Add(new Tuple<Point, Point>(departure, arrival));
			}
		}

		public Point Exit { get; private set; }
		public SparseMap<Portal> Portals { get; set; }

		public override Point Transform(Point p)
		{
			return Portals[p]?.Pos ?? p;
		}
	}
}
