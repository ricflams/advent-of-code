using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day25
{
	internal class AutoplayController : IGameController
	{
		private static readonly string[] DangerousItems = // Could be learned, but hardcode for now
		{
			"escape pod", "infinite loop", "photons", "molten lava", "giant electromagnet"
		};

		private class RoomNode
		{
			public Room Room { get; set; }
			public bool IsEntryPoint { get; set; }
			public HashSet<string> Explored { get; } = new HashSet<string>();
		}

		private enum ExploreMode
		{
			GatherAllItems,
			FindSecurityCheckpoint,
			PrepareToPassSecurityCheckpoint,
			PassSecurityCheckpoint
		};

		private readonly Dictionary<string, RoomNode> _nodes = new Dictionary<string, RoomNode>();
		private ExploreMode _mode = ExploreMode.GatherAllItems;
		private string _wayBack = null;
		private string _checkpointDoor;
		private readonly Queue<string> _checkpointAttempts = new Queue<string>();

		public string WhatNext(Game game)
		{
			var room = game.CurrentRoom;
			var node = GetOrCreateRoomNode(room);

			while (true)
			{
				switch (_mode)
				{
					case ExploreMode.GatherAllItems:
						{
							// Pick up anything non-dangerous
							var pickUpItem = room.Items.FirstOrDefault(x => !DangerousItems.Contains(x));
							if (pickUpItem != null)
							{
								return $"take {pickUpItem}";
							}

							// Follow any unexplored door
							var door = room.Doors.Except(node.Explored).FirstOrDefault() ?? node.Explored.First();
							node.Explored.Add(door);
							_wayBack = Backwards(door);

							if (door != null)
							{
								return door;
							}

							// A bit brute: simply forget all rooms and search for the checkpoint
							_nodes.Clear();
							_wayBack = null;
							node = GetOrCreateRoomNode(room);
							_mode = ExploreMode.FindSecurityCheckpoint;
						}
						break;

					case ExploreMode.FindSecurityCheckpoint:
						{
							if (room.IsSecurityCheckpoint)
							{
								_mode = ExploreMode.PrepareToPassSecurityCheckpoint;
								_checkpointDoor = room.Doors.First(d => d != _wayBack);
							}
							else
							{
								var door = room.Doors.Except(node.Explored).FirstOrDefault() ?? node.Explored.First();
								node.Explored.Add(door);
								_wayBack = Backwards(door);
								return door;
							}
						}
						break;
					case ExploreMode.PrepareToPassSecurityCheckpoint:
						{
							var inventory = game.Inventory;
							var n = inventory.Count();

							var prevmask = (1 << n) - 1; // set all bits because all items are currently taken
							for (var combo = 0; combo < 1 << n; combo++)
							{
								var keepmask = combo ^ (combo >> 1);
								for (var i = 0; i < n; i++)
								{
									var itemmask = 1 << i;
									var use = (keepmask & itemmask) != 0;
									var has = (prevmask & itemmask) != 0;
									if (use && !has)
									{
										_checkpointAttempts.Enqueue($"take {inventory[i]}");
									}
									else if (!use && has)
									{
										_checkpointAttempts.Enqueue($"drop {inventory[i]}");
									}
								}
								_checkpointAttempts.Enqueue(_checkpointDoor);
								prevmask = keepmask;
							}
							_mode = ExploreMode.PassSecurityCheckpoint;
						}
						break;

					case ExploreMode.PassSecurityCheckpoint:
						return _checkpointAttempts.Dequeue();
				}
			}
		}

		public void OnGameOver(Game game)
		{
		}

		private RoomNode GetOrCreateRoomNode(Room room)
		{
			if (!_nodes.TryGetValue(room.Name, out var node))
			{
				node = new RoomNode { Room = room };
				node.Explored.Add(_wayBack);
				_nodes[room.Name] = node;
			}
			return node;
		}

		private string[] doors = { "north", "east", "south", "west" };
		private string Backwards(string door) => doors[(Array.IndexOf(doors, door) + doors.Length / 2) % doors.Length];
	}
}
