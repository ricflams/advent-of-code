using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;

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
			Run("input").Part1(1824).Part2(1937);
		}

		protected override int Part1(string[] input)
		{
			return MinimumManaSpentOnFight(input, 0);
		}

		protected override int Part2(string[] input)
		{
			return MinimumManaSpentOnFight(input, 1);
		}

		private static int MinimumManaSpentOnFight(string[] input, int extraDamage)
		{
			var bossHitpoints = input[0].RxMatch("Hit Points: %d").Get<int>();
			var bossDamage = input[1].RxMatch("Damage: %d").Get<int>();

			var game0 = new Game
			{
				IsPlayersTurn = true,
				TotalManaSpent = 0,
				PlayerHitpoints = 50,
				PlayerMana = 500,
				BossHitpoints = bossHitpoints,
				BossDamage = bossDamage,
				EffectShield = 0,
				EffectPoison = 0,
				EffectRecharge = 0
			};

			var minManaSpent = int.MaxValue;
			var queue = Quack<Game>.Create(QuackType.Stack);
			queue.Put(game0);

			while (queue.TryGet(out var game))
			{
				// Skip exploring games that can't end up spending less mana
				if (game.TotalManaSpent >= minManaSpent)
					continue;

				// Make a copy of game to modify
				game = game with { };

				// In part 2 the player sustains additional damage
				if (extraDamage > 0 && game.IsPlayersTurn)
				{
					game.PlayerHitpoints -= extraDamage;
					if (game.PlayerHitpoints <= 0)
						continue;
				}

				// Effect of poison, if any
				if (game.EffectPoison > 0)
				{
					game.EffectPoison--;
					game.BossHitpoints -= 3;
				}

				// Effect of recharge, if any
				if (game.EffectRecharge > 0)
				{
					game.EffectRecharge--;
					game.PlayerMana += 101;
				}

				// Effect of shield on armor, if any
				var armor = 0;
				if (game.EffectShield > 0)
				{
					game.EffectShield--;
					armor += 7;
				}

				// Check if boss is dead
				if (game.BossHitpoints <= 0)
				{
					minManaSpent = game.TotalManaSpent;
					continue;
				}

				// Play the round
				if (game.IsPlayersTurn)
				{
					// Player lose if no spells can be cast
					if (game.PlayerMana < Game.CostMinimum)
						continue;
					if (game.CanCastMagicMissile) queue.Put(game.CastMagicMissile());
					if (game.CanCastDrain) queue.Put(game.CastDrain());
					if (game.CanCastShield) queue.Put(game.CastShield());
					if (game.CanCastPoison) queue.Put(game.CastPoison());
					if (game.CanCastRecharge) queue.Put(game.CastRecharge());
				}
				else
				{
					var damage = Math.Max(1, game.BossDamage - armor);
					game.PlayerHitpoints -= damage;
					if (game.PlayerHitpoints > 0)
					{
						game.IsPlayersTurn = true;
						queue.Put(game);
					}
				}
			}

			return minManaSpent;
		}

		private record Game
		{
			public bool IsPlayersTurn;
			public int TotalManaSpent;
			public int PlayerHitpoints;
			public int PlayerMana;
			public int BossHitpoints;
			public int BossDamage;
			public int EffectShield;
			public int EffectPoison;
			public int EffectRecharge;

			public override string ToString()
			{
				return $"{(IsPlayersTurn ? "You" : "Boss")} spent={TotalManaSpent} hp={PlayerHitpoints} mana={PlayerMana} bosshp={BossHitpoints} damage={BossDamage} shield={EffectShield} poison={EffectPoison} recharge={EffectRecharge}";
			}

			private static int CostMagicMissile = 53;
			private static int CostDrain = 73;
			private static int CostShield = 113;
			private static int CostPoison = 173;
			private static int CostRecharge = 229;
			public static int CostMinimum = CostMagicMissile;

			public bool CanCastMagicMissile => PlayerMana >= CostMagicMissile;
			public bool CanCastDrain => PlayerMana >= CostDrain;
			public bool CanCastShield => PlayerMana >= CostShield && EffectShield == 0;
			public bool CanCastPoison => PlayerMana >= CostPoison && EffectPoison == 0;
			public bool CanCastRecharge => PlayerMana >= CostRecharge && EffectRecharge == 0;

			public Game CastMagicMissile() => Spend(CostMagicMissile) with { BossHitpoints = BossHitpoints - 4 };
			public Game CastDrain() => Spend(CostDrain) with { BossHitpoints = BossHitpoints - 2, PlayerHitpoints = PlayerHitpoints + 2 };
			public Game CastShield() => Spend(CostShield) with { EffectShield = 6 };
			public Game CastPoison() => Spend(CostPoison) with { EffectPoison = 6 };
			public Game CastRecharge() => Spend(CostRecharge) with { EffectRecharge = 5 };
			private Game Spend(int mana) => this with
			{
				IsPlayersTurn = !IsPlayersTurn,
				PlayerMana = PlayerMana - mana,
				TotalManaSpent = TotalManaSpent + mana
			};
		}
	}
}

