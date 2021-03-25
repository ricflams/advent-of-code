using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2015.Day22
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Wizard Simulator 20XX";
		public override int Year => 2015;
		public override int Day => 22;

		public void Run()
		{
			Run("input").Part1(0).Part2(0);
		}

		protected override int Part1(string[] input)
		{
			var rawinput = string.Join(Environment.NewLine, input);
			//Hit Points: 71
			//Damage: 10
			//var state = new State
			//{
			//	PlayerHitpoints = 50,
			//	PlayerMana = 500,
			//	BossHitpoints = SimpleRegex.MatchInt(input, "Hit Points: %d"),
			//	BossDamage = SimpleRegex.MatchInt(input, "Damage: %d")
			//};
			//state = new State
			//{
			//	PlayerHitpoints = 10,
			//	PlayerMana = 250,
			//	BossHitpoints = 14,
			//	BossDamage = 8
			//};

			var player = new Player
			{
				Armor = 0,
				Hitpoints = 50,
				Mana = 500
			};
			var boss = new Boss
			{
				Hitpoints = rawinput.RxMatch("Hit Points: %d").Get<int>(),
				Damage = rawinput.RxMatch("Damage: %d").Get<int>(),
			};

			var leastSpent = Fight(player, boss);

			return 1;

			//var player1 = new Player
			//{
			//	Armor = 0,
			//	Hitpoints = 10,
			//	Mana = 250
			//};
			//var boss1 = new Boss
			//{
			//	Hitpoints = 14,
			//	Damage = 8
			//};


			//var fixedmatch = new FixedFight(
			//	new RechargeSpell(),
			//	new ShieldSpell(),
			//	new DrainSpell(),
			//	new PoisonSpell(),
			//	new MagicMissileSpell()
			//	);
			////Fight(player1, boss1, fixedmatch);

			//var randomFight = new RandomFight();
			//var leastSpent = int.MaxValue;
			//for (var i = 0; i < 100000; i++)
			//{
			//	var player = new Player
			//	{
			//		Armor = 0,
			//		Hitpoints = 50,
			//		Mana = 500
			//	};
			//	var boss = new Boss
			//	{
			//		Hitpoints = SimpleRegex.MatchInt(input, "Hit Points: %d"),
			//		Damage = SimpleRegex.MatchInt(input, "Damage: %d")
			//	};
			//	Fight(player, boss, randomFight);
			//	if (boss.Hitpoints <= 0)
			//	{
			//		Console.Write("+");
			//		if (player.ManaSpent < leastSpent)
			//		{
			//			leastSpent = player.ManaSpent;
			//			Console.Write("#");
			//		}
			//	}
			//	else
			//	{
			//		Console.Write(".");
			//	}
			//	Console.ReadLine();
			//}
		}

		protected override int Part2(string[] input)
		{
			return 1;
		}



		private static int Fight(Player player, Boss boss)
		{
			var firstSpell = new List<MagicSpell>();
			var minManaSpent = int.MaxValue;
			for (var depth = 0; depth < 20; depth++)
			{
				var (spell, bossHits, spent) = AllSpells
					.Select(spell =>
					{
						//if (firstSpell.Sum(x => x.Cost) + spell.Cost > player.Mana)
						//{
						//	return (null, 0, int.MaxValue);
						//}
						var minManaSpent = int.MaxValue;
						var bossHits = 0;
						var thisFirstSpells = firstSpell.Append(spell);

						foreach (var spellOrder in MathHelper.CountInBaseX(AllSpells.Length, 6))
						{
							var spells = firstSpell.Append(spell).Concat(spellOrder.Select(x => AllSpells[x]));
							var spellChooser = new SelectiveFight(spells);
							var p = new Player { Hitpoints = player.Hitpoints, Mana = player.Mana };
							var b = new Boss { Hitpoints = boss.Hitpoints, Damage = boss.Damage };
							Fight(p, b, spellChooser);
							//Console.WriteLine();

							//Console.ReadLine();
							//bossHits += boss.Hitpoints - b.Hitpoints + (p.Hitpoints - player.Hitpoints);
							bossHits += (p.Hitpoints - b.Hitpoints) - (player.Hitpoints - boss.Hitpoints);

							if (b.Hitpoints <= 0 && p.ManaSpent < minManaSpent)
							{
								minManaSpent = p.ManaSpent;
							}
						}
						return (spell, bossHits, minManaSpent);
					})
					//.OrderBy(x => x.minManaSpent)
					.Where(x => x.spell != null)
					.OrderByDescending(x => x.bossHits)
					.First();
				if (spent < minManaSpent)
				{
					minManaSpent = spent;
				}
				firstSpell.Add(spell);

				Console.WriteLine($"minSpent={minManaSpent} for {bossHits} hits on {spell.GetType().Name}");

			}
			return minManaSpent;
		}


		//internal const int BigValue = 1000000;

		private static void Fight(Player player, Boss boss, ISpellChooser spellChooser)
		{
			Log("\n#############################\n");
			for (var isPlayersTurn = true; ; isPlayersTurn = !isPlayersTurn)
			{
				Log($"-- {(isPlayersTurn ? "Player" : "Boss")} turn --");
				Log($"- Player has {player.Hitpoints} hit points, {player.Armor} armor, {player.Mana} mana");
				Log($"- Boss has {boss.Hitpoints} hit points");

				player.ApplyEffects();
				if (boss.Hitpoints <= 0)
				{
					Log($"\nThis kills the boss, and the player wins.");
					return;
				}

				if (isPlayersTurn)
				{
					var spell = spellChooser.NextSpell(player);
					if (spell == null)
					{
						Log($"\nThe player has not enough mana to cast a spell, so the boss wins.");
						return;
					}

					//Console.Write(spell.GetType().Name + " ");

					player.Mana -= spell.Cost;
					player.ManaSpent += spell.Cost;
					spell.Cast(player, boss);
					if (boss.Hitpoints <= 0)
					{
						Log($"\nThis kills the boss, and the player wins.");
						return;
					}
				}
				else
				{
					boss.Attack(player);
					if (player.Hitpoints <= 0)
					{
						Log($"\nThe player lose.");
						return;
					}
				}

				Log("");
			}
		}

		internal static void Log(string message)
		{
			//Console.WriteLine(message);
		}

		private static void Puzzle2()
		{

			//Console.WriteLine($"Day 22 Puzzle 2: {result}");
			//Debug.Assert(result == );
		}

		private interface ISpellChooser
		{
			MagicSpell NextSpell(Player player);
		}

		private class FixedFight : ISpellChooser
		{
			private readonly Queue<MagicSpell> _spells;
			public FixedFight(params MagicSpell[] spells)
			{
				_spells = new Queue<MagicSpell>(spells);
			}

			public MagicSpell NextSpell(Player _) => _spells.Dequeue();
		}

		private readonly static MagicSpell[] AllSpells = new MagicSpell[]
		{
			new MagicMissileSpell(),
			new ShieldSpell(),
			new DrainSpell(),
			new PoisonSpell(),
			new RechargeSpell()
		};

		private class RandomFight : ISpellChooser
		{
			private static readonly Random Random = new Random();

			public MagicSpell NextSpell(Player player)
			{
				var spells = AllSpells
					.Where(s => s.Cost <= player.Mana)
					.Where(s => s.EffectType == null || !player.Effects.Any(e => e.GetType() == s.EffectType))
					.ToArray();
				if (!spells.Any())
				{
					return null;
				}
				var spell = spells[Random.Next(0, spells.Length-1)];
				return spell;
			}
		}


		private class SelectiveFight : ISpellChooser
		{
			private static readonly Random Random = new Random();
			private readonly Queue<MagicSpell> _firstSpells;

			public SelectiveFight(IEnumerable<MagicSpell> firstSpells)
			{
				_firstSpells = new Queue<MagicSpell>(firstSpells);
			}

			public MagicSpell NextSpell(Player player)
			{
				while (_firstSpells.Count > 0 && _firstSpells.Peek().Cost > player.Mana)
				{
					_firstSpells.Dequeue();
				}
				if (_firstSpells.Count > 0)
				{
					return _firstSpells.Dequeue();
				}
				return null;

				//////if (player.Hitpoints > 24 && player.Mana >= 229 && player.Mana < 229+173 && !player.Effects.Any(e => e.GetType() == typeof(RechargeEffect)))
				//////	return new RechargeSpell();

				////var spells = AllSpells
				////	.Where(s => s.Cost <= player.Mana)
				////	.Where(s => s.EffectType == null || !player.Effects.Any(e => e.GetType() == s.EffectType))
				////	.ToArray();
				////if (!spells.Any())
				////{
				////	return null;
				////}
				////var spell = spells[Random.Next(0, spells.Length - 1)];
				////return spell;
			}
		}


		private class Player
		{
			public int Hitpoints { get; set; }
			public int Armor { get; set; }
			public int Mana { get; set; }
			public int ManaSpent { get; set; }
			public List<MagicEffect> Effects { get; } = new List<MagicEffect>();

			public void ApplyEffects()
			{
				foreach (var effect in Effects)
				{
					effect.Apply();
				}
				Effects.RemoveAll(x => x.Timer == 0);
			}
		}

		private class Boss
		{
			public int Hitpoints { get; set; }
			public int Damage { get; set; }

			public void Attack(Player player)
			{
				if (player.Armor == 0)
				{
					Log($"Boss attacks for {Damage} damage!");
					player.Hitpoints -= Damage;
				}
				else
				{
					var damage = Math.Max(1, Damage - player.Armor);
					Log($"Boss attacks for {Damage} - {player.Armor} = {damage} damage!");
					player.Hitpoints -= damage;
				}
			}
		}

		private abstract class MagicEffect
		{
			protected readonly Player _player;
			protected readonly Boss _boss;

			public int Timer { get; protected set; }

			protected MagicEffect(Player player, Boss boss, int timer)
			{
				_player = player;
				_boss = boss;
				Timer = timer;
			}

			public abstract void Apply();
		}

		private class ShieldEffect : MagicEffect
		{
			public ShieldEffect(Player player, Boss boss)
				: base(player, boss, 6) { }

			public override void Apply()
			{
				Log($"Shield's timer is now {--Timer}.");
				if (Timer == 0)
				{
					Log($"Shield wears off, decreasing armor by 7.");
					_player.Armor -= 7;
				}
			}
		}

		private class PoisonEffect : MagicEffect
		{
			public PoisonEffect(Player player, Boss boss)
				: base(player, boss, 6) { }

			public override void Apply()
			{
				Log($"Poison deals 3 damage; its timer is now {--Timer}.");
				if (Timer == 0)
				{
					Log($"Poison wears off.");
				}
				_boss.Hitpoints -= 3;
			}
		}

		private class RechargeEffect : MagicEffect
		{
			public RechargeEffect(Player player, Boss boss)
				: base(player, boss, 5) { }

			public override void Apply()
			{
				Log($"Recharge provides 101 mana; its timer is now {--Timer}.");
				if (Timer == 0)
				{
					Log($"Recharge wears off.");
				}
				_player.Mana += 101;
			}
		}

		private abstract class MagicSpell
		{
			public abstract int Cost { get; }
			public virtual Type EffectType => null;
			public abstract void Cast(Player player, Boss boss);
		}

		private class MagicMissileSpell : MagicSpell
		{
			public override int Cost => 53;
			public override void Cast(Player player, Boss boss)
			{
				Log($"Player casts Magic Missile, dealing 4 damage.");
				boss.Hitpoints -= 4;
			}
		}

		private class DrainSpell : MagicSpell
		{
			public override int Cost => 73;
			public override void Cast(Player player, Boss boss)
			{
				Log($"Player casts Drain, dealing 2 damage, and healing 2 hit points.");
				player.Hitpoints += 2;
				boss.Hitpoints -= 2;
			}
		}

		private class ShieldSpell : MagicSpell
		{
			public override int Cost => 113;
			public override Type EffectType => typeof(ShieldEffect);
			public override void Cast(Player player, Boss boss)
			{
				Log($"Player casts Shield, increasing armor by 7.");
				player.Armor += 7;
				player.Effects.Add(new ShieldEffect(player, boss));
			}
		}

		private class PoisonSpell : MagicSpell
		{
			public override int Cost => 173;
			public override Type EffectType => typeof(PoisonEffect);
			public override void Cast(Player player, Boss boss)
			{
				Log($"Player casts Poison.");
				player.Effects.Add(new PoisonEffect(player, boss));
			}
		}

		private class RechargeSpell : MagicSpell
		{
			public override int Cost => 229;
			public override Type EffectType => typeof(RechargeEffect);
			public override void Cast(Player player, Boss boss)
			{
				Log($"Player casts Recharge.");
				player.Effects.Add(new RechargeEffect(player, boss));
			}
		}

	}
}
//public int Cast(Player player, Boss boss)
//	{
//		Log($"Player cast Magic Missile, dealing 4 damage.\n");
//		player.Mana -= Cost;

//		var newstate = state;
//		newstate.PlayerMana -= 53;
//		newstate.BossHitpoints -= 4;
//		if (state.BossHitpoints <= 0)
//		{
//			Log($"This kills the boss, and the player wins.");
//			Console.Write("B");
//			playerMana = state.PlayerMana;
//			var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//			if (playerMana > biggestManaSeen)
//			{
//				BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//				Console.Write("O");
//			}
//			return true;
//		}

//	}



//private static SafeDictionary<int, int> BiggestManaForDiff = new SafeDictionary<int, int>(-10000);


//private static int Fight(pl)
//{

//}


//internal static bool Fight(State state, bool isPlayersTurn, out int playerMana)
//{
//	Log($"-- {(isPlayersTurn ? "Player" : "Boss")} turn --");
//	Log($"- Player has {state.PlayerHitpoints} hit points, {state.PlayerArmor} armor, {state.PlayerMana} mana");
//	Log($"- Boss has {state.BossHitpoints} hit points");


//	if (state.ShieldEffect == 0 && state.PoisonEffect == 0 && state.RechargeEffect == 0)
//	{
//		var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//		if (biggestManaSeen > state.PlayerMana)
//		{
//			playerMana = 0;
//			Console.Write("#");
//			return false;
//		}
//		Console.Write(".");
//	}
//	else
//		Console.Write(":");

//	if (state.ShieldEffect > 0)
//	{
//		state.ShieldEffect--;
//		Log($"Shield's timer is now {state.ShieldEffect}.");
//		if (state.ShieldEffect == 0)
//		{
//			Log($"Shield wears off, decreasing armor by 7.");
//			state.PlayerArmor -= 7;
//		}
//	}
//	if (state.PoisonEffect > 0)
//	{
//		state.PoisonEffect--;
//		state.BossHitpoints -= 3;
//		Log($"Poison deals 3 damage; it's timer is now {state.PoisonEffect}.");
//		if (state.BossHitpoints <= 0)
//		{
//			Log($"This kills the boss, and the player wins.");
//			Console.Write("B");
//			playerMana = state.PlayerMana;
//			return true;
//		}
//	}
//	if (state.RechargeEffect > 0)
//	{
//		state.RechargeEffect--;
//		state.PlayerMana += 101;
//		Log($"Recharge provides 101 mana; it's timer is now {state.RechargeEffect}.");
//		if (state.RechargeEffect == 0)
//		{
//			Log($"Recharge wears off.");
//		}
//	}

//	if (isPlayersTurn)
//	{
//		playerMana = 0;
//		if (state.PlayerMana >= 53)
//		{
//			Log($"Player cast Magic Missile, dealing 4 damage.\n");
//			var newstate = state;
//			newstate.PlayerMana -= 53;
//			newstate.BossHitpoints -= 4;
//			if (state.BossHitpoints <= 0)
//			{
//				Log($"This kills the boss, and the player wins.");
//				Console.Write("B");
//				playerMana = state.PlayerMana;
//				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//				if (playerMana > biggestManaSeen)
//				{
//					BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//					Console.Write("O");
//				}
//				return true;
//			}
//			if (Fight(newstate, !isPlayersTurn, out var mana))
//			{
//				playerMana = Math.Max(playerMana, mana);
//				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//				if (playerMana > biggestManaSeen)
//				{
//					BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//					Console.Write("O");
//				}
//			}
//		}

//		if (state.PlayerMana >= 73)
//		{
//			Log($"Player cast Drain, dealing 2 damage, and healing 2 hit points.\n");
//			var newstate = state;
//			newstate.PlayerMana -= 73;
//			newstate.PlayerHitpoints += 2;
//			newstate.BossHitpoints -= 2;
//			if (state.BossHitpoints <= 0)
//			{
//				Log($"This kills the boss, and the player wins.");
//				Console.Write("B");
//				playerMana = state.PlayerMana;
//				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//				if (playerMana > biggestManaSeen)
//				{
//					BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//					Console.Write("O");
//				}
//				return true;
//			}
//			if (Fight(newstate, !isPlayersTurn, out var mana))
//			{
//				playerMana = Math.Max(playerMana, mana);
//				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//				if (playerMana > biggestManaSeen)
//				{
//					BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//					Console.Write("O");
//				}
//			}
//		}

//		if (state.ShieldEffect == 0 && state.PlayerMana >= 113)
//		{
//			Log($"Player casts Shield.\n");
//			var newstate = state;
//			newstate.PlayerMana -= 113;
//			newstate.ShieldEffect = 6;
//			newstate.PlayerArmor += 7;
//			if (Fight(newstate, !isPlayersTurn, out var mana))
//			{
//				playerMana = Math.Max(playerMana, mana);
//				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//				if (playerMana > biggestManaSeen)
//				{
//					BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//					Console.Write("O");
//				}
//			}
//		}

//		if (state.PoisonEffect == 0 && state.PlayerMana >= 173)
//		{
//			Log($"Player casts Poison.\n");
//			var newstate = state;
//			newstate.PlayerMana -= 173;
//			newstate.PoisonEffect = 6;
//			if (Fight(newstate, !isPlayersTurn, out var mana))
//			{
//				playerMana = Math.Max(playerMana, mana);
//				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//				if (playerMana > biggestManaSeen)
//				{
//					BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//					Console.Write("O");
//				}
//			}
//		}

//		if (state.RechargeEffect == 0 && state.PlayerMana >= 229)
//		{
//			Log($"Player casts Recharge.\n");
//			var newstate = state;
//			newstate.PlayerMana -= 229;
//			newstate.RechargeEffect = 5;
//			if (Fight(newstate, !isPlayersTurn, out var mana))
//			{
//				playerMana = Math.Max(playerMana, mana);
//				var biggestManaSeen = BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints];
//				if (playerMana > biggestManaSeen)
//				{
//					BiggestManaForDiff[state.PlayerHitpoints - state.BossHitpoints] = biggestManaSeen;
//					Console.Write("O");
//				}
//			}
//		}

//		return playerMana > 0;
//	}
//	else
//	{
//		playerMana = 0;
//		var damage = Math.Max(1, state.BossDamage - state.PlayerArmor);
//		Log($"Boss attacks for {damage} damage!\n");
//		var newstate = state;
//		newstate.PlayerHitpoints -= damage;
//		if (newstate.PlayerHitpoints <= 0)
//		{
//			Log($"You die.\n");
//			Console.Write("Y");
//			return false;
//		}

//		return Fight(newstate, !isPlayersTurn, out playerMana);
//	}

//}

//internal struct State
//{
//	public int PlayerHitpoints { get; set; }
//	public int PlayerArmor { get; set; }
//	public int PlayerMana { get; set; }
//	public int BossHitpoints { get; set; }
//	public int BossDamage { get; set; }
//	public int ShieldEffect { get; set; }
//	public int PoisonEffect { get; set; }
//	public int RechargeEffect { get; set; }
//}



