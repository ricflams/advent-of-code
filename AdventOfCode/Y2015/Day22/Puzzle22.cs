using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2015.Day22
{
	internal class Puzzle22
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllText("Y2015/Day21/input.txt");

			//Hit Points: 71
			//Damage: 10
			var state = new State
			{
				PlayerHitpoints = 50,
				PlayerMana = 500,
				BossHitpoints = SimpleRegex.MatchInt(input, "Hit Points: %d"),
				BossDamage = SimpleRegex.MatchInt(input, "Damage: %d")
			};
			//state = new State
			//{
			//	PlayerHitpoints = 10,
			//	PlayerMana = 250,
			//	BossHitpoints = 14,
			//	BossDamage = 8
			//};

			var lowestCost = LowestCostOfFight(state);

			Console.WriteLine($"Day 22 Puzzle 1: {lowestCost}");
			//Debug.Assert(result == );
		}

		//internal const int BigValue = 1000000;

		internal static int LowestCostOfFight(State state)
		{
			var initialMana = state.PlayerMana;
			if (!Fight(state, true, out var playerMana))
			{
				throw new Exception("Player cannot win");
			}
			return initialMana - playerMana;
		}

		private static SafeDictionary<int, int> BiggestManaForDiff = new SafeDictionary<int, int>(-10000);

		internal static bool Fight(State state, bool isPlayersTurn, out int playerMana)
		{
			Log($"-- {(isPlayersTurn ? "Player" : "Boss")} turn --");
			Log($"- Player has {state.PlayerHitpoints} hit points, {state.PlayerArmor} armor, {state.PlayerMana} mana");
			Log($"- Boss has {state.BossHitpoints} hit points");


			if (state.ShieldEffect == 0 && state.PoisonEffect == 0 && state.RechargeEffect == 0)
			{
				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
				if (biggestManaSeen > state.PlayerMana)
				{
					playerMana = 0;
					Console.Write("#");
					return false;
				}
				Console.Write(".");
			}
			else
				Console.Write(":");

			if (state.ShieldEffect > 0)
			{
				state.ShieldEffect--;
				Log($"Shield's timer is now {state.ShieldEffect}.");
				if (state.ShieldEffect == 0)
				{
					Log($"Shield wears off, decreasing armor by 7.");
					state.PlayerArmor -= 7;
				}
			}
			if (state.PoisonEffect > 0)
			{
				state.PoisonEffect--;
				state.BossHitpoints -= 3;
				Log($"Poison deals 3 damage; it's timer is now {state.PoisonEffect}.");
				if (state.BossHitpoints <= 0)
				{
					Log($"This kills the boss, and the player wins.");
					Console.Write("B");
					playerMana = state.PlayerMana;
					return true;
				}
			}
			if (state.RechargeEffect > 0)
			{
				state.RechargeEffect--;
				state.PlayerMana += 101;
				Log($"Recharge provides 101 mana; it's timer is now {state.RechargeEffect}.");
				if (state.RechargeEffect == 0)
				{
					Log($"Recharge wears off.");
				}
			}

			if (isPlayersTurn)
			{
				playerMana = 0;
				if (state.PlayerMana >= 53)
				{
					Log($"Player cast Magic Missile, dealing 4 damage.\n");
					var newstate = state;
					newstate.PlayerMana -= 53;
					newstate.BossHitpoints -= 4;
					if (state.BossHitpoints <= 0)
					{
						Log($"This kills the boss, and the player wins.");
						Console.Write("B");
						playerMana = state.PlayerMana;
						var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
						if (playerMana > biggestManaSeen)
						{
							BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
							Console.Write("O");
						}
						return true;
					}
					if (Fight(newstate, !isPlayersTurn, out var mana))
					{
						playerMana = Math.Max(playerMana, mana);
						var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
						if (playerMana > biggestManaSeen)
						{
							BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
							Console.Write("O");
						}
					}
				}

				if (state.PlayerMana >= 73)
				{
					Log($"Player cast Drain, dealing 2 damage, and healing 2 hit points.\n");
					var newstate = state;
					newstate.PlayerMana -= 73;
					newstate.PlayerHitpoints += 2;
					newstate.BossHitpoints -= 2;
					if (state.BossHitpoints <= 0)
					{
						Log($"This kills the boss, and the player wins.");
						Console.Write("B");
						playerMana = state.PlayerMana;
						var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
						if (playerMana > biggestManaSeen)
						{
							BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
							Console.Write("O");
						}
						return true;
					}
					if (Fight(newstate, !isPlayersTurn, out var mana))
					{
						playerMana = Math.Max(playerMana, mana);
						var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
						if (playerMana > biggestManaSeen)
						{
							BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
							Console.Write("O");
						}
					}
				}

				if (state.ShieldEffect == 0 && state.PlayerMana >= 113)
				{
					Log($"Player casts Shield, dealing 2 damage, and healing 2 hit points.\n");
					var newstate = state;
					newstate.PlayerMana -= 113;
					newstate.ShieldEffect = 6;
					newstate.PlayerArmor += 7;
					if (Fight(newstate, !isPlayersTurn, out var mana))
					{
						playerMana = Math.Max(playerMana, mana);
						var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
						if (playerMana > biggestManaSeen)
						{
							BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
							Console.Write("O");
						}
					}
				}

				if (state.PoisonEffect == 0 && state.PlayerMana >= 173)
				{
					Log($"Player casts Poison.\n");
					var newstate = state;
					newstate.PlayerMana -= 173;
					newstate.PoisonEffect = 6;
					if (Fight(newstate, !isPlayersTurn, out var mana))
					{
						playerMana = Math.Max(playerMana, mana);
						var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
						if (playerMana > biggestManaSeen)
						{
							BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
							Console.Write("O");
						}
					}
				}

				if (state.RechargeEffect == 0 && state.PlayerMana >= 229)
				{
					Log($"Player casts Recharge.\n");
					var newstate = state;
					newstate.PlayerMana -= 229;
					newstate.RechargeEffect = 5;
					if (Fight(newstate, !isPlayersTurn, out var mana))
					{
						playerMana = Math.Max(playerMana, mana);
						var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
						if (playerMana > biggestManaSeen)
						{
							BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
							Console.Write("O");
						}
					}
				}

				return playerMana > 0;
			}
			else
			{
				playerMana = 0;
				var damage = Math.Max(1, state.BossDamage - state.PlayerArmor);
				Log($"Boss attacks for {damage} damage!\n");
				var newstate = state;
				newstate.PlayerHitpoints -= damage;
				if (newstate.PlayerHitpoints <= 0)
				{
					Log($"You die.\n");
					Console.Write("Y");
					return false;
				}

				return Fight(newstate, !isPlayersTurn, out playerMana);
			}

		}

		internal static void Log(string message)
		{
			//Console.WriteLine(message);
		}

		internal struct State
		{
			public int PlayerHitpoints { get; set; }
			public int PlayerArmor { get; set; }
			public int PlayerMana { get; set; }
			public int BossHitpoints { get; set; }
			public int BossDamage { get; set; }
			public int ShieldEffect { get; set; }
			public int PoisonEffect { get; set; }
			public int RechargeEffect { get; set; }
		}

		private static void Puzzle2()
		{

			//Console.WriteLine($"Day 22 Puzzle 2: {result}");
			//Debug.Assert(result == );
		}
	}
}
