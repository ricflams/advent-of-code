using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public interface IGeogebraObject
	{
		IEnumerable<string> Command(Func<int> NextId);
	}

	public class Polygon : IGeogebraObject
	{
		public List<Point3D> _points = new();
		public IEnumerable<string> Command(Func<int> NextId)
		{
			// https://wiki.geogebra.org/en/Naming_Objects

			var pts = string.Join(",", _points.Select(p => $"({p.X},{p.Y},{p.Z})"));
			yield return $"p{NextId()}=Plane(Polygon({pts}))";
		}
	}

	public class Line : IGeogebraObject
	{
		public List<Point3D> _points = new();
		public IEnumerable<string> Command(Func<int> NextId)
		{
			// https://wiki.geogebra.org/en/Naming_Objects

			var pts = string.Join(",", _points.Select(p => $"({p.X},{p.Y},{p.Z})"));
			yield return $"p{NextId()}=Plane(Polygon({pts}))";
		}
	}
	public class Geogebra
	{
		private int _index;
		private readonly List<IGeogebraObject> _objects = new();
		public void Add(IGeogebraObject obj) => _objects.Add(obj);

		internal int NextId() => _index++;

		public string AsExecuteCommands()
		{
			var rawobjs = _objects.SelectMany(o => o.Command(NextId)).Select(c => '"' + c + '"');
			var objs = string.Join(",", rawobjs);
			var exe = $"Execute({{{objs}}})";
			return exe;
		}

		public void Print()
		{
			Console.WriteLine(AsExecuteCommands());
		}
	}
}
