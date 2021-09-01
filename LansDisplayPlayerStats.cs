using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LansDisplayPlayerStats
{

	public struct AccDisplay
	{

		public Texture2D texture;
		public string infoText;
		public Func<bool> isVisible;
		public Func<string> getAccValue;

		public AccDisplay(Texture2D texture, string infoText, Func<bool> isVisible, Func<string> getAccValue)
		{
			this.texture = texture;
			this.infoText = infoText;
			this.isVisible = isVisible;
			this.getAccValue = getAccValue;
		}
	}


	public class LansDisplayPlayerStats : Mod
	{
		public static LansDisplayPlayerStats instance;

		//Maybe move this to a injected method instead if someone decideds to IL edit for ....reasons?
		public float[] getWeaponStats()
		{
			var item = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem];

			float add = Main.LocalPlayer.allDamage;
			float mult = Main.LocalPlayer.allDamageMult;
			float flat = 0f;
			int damage = item.damage;

			

			if (item.melee)
			{
				add += Main.LocalPlayer.meleeDamage - 1;
				mult *= Main.LocalPlayer.meleeDamageMult;
			}
			if (item.ranged)
			{
				add += Main.LocalPlayer.rangedDamage - 1;
				mult *= Main.LocalPlayer.rangedDamageMult;

				if (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Stake)
				{
					mult *= Main.LocalPlayer.arrowDamage;
				}
				if (item.useAmmo == AmmoID.Arrow && Main.LocalPlayer.archery)
				{
					mult *= 1.2f;
				}
				if (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.CandyCorn)
				{
					mult *= Main.LocalPlayer.bulletDamage;
				}
				if (item.useAmmo == AmmoID.Rocket || item.useAmmo == AmmoID.StyngerBolt || item.useAmmo == AmmoID.JackOLantern || item.useAmmo == AmmoID.NailFriendly)
				{
					mult *= Main.LocalPlayer.rocketDamage;
				}
			}
			if (item.magic)
			{
				add += Main.LocalPlayer.magicDamage - 1;
				mult *= Main.LocalPlayer.magicDamageMult;
			}
			if (item.summon)
			{
				add += Main.LocalPlayer.minionDamage - 1;
				mult *= Main.LocalPlayer.minionDamageMult;
			}
			if (item.thrown)
			{
				add += Main.LocalPlayer.thrownDamage - 1;
				mult *= Main.LocalPlayer.thrownDamageMult;
			}

			if (item.modItem?.IgnoreDamageModifiers == true)
			{
				add = 1;
				mult = 1;
				flat = 0;
			}
			else
			{
				CombinedHooks.ModifyWeaponDamage(Main.LocalPlayer, item, ref add, ref mult, ref flat);
				damage = (int)(item.damage * add * mult + 5E-06f + flat);
				CombinedHooks.GetWeaponDamage(Main.LocalPlayer, item, ref damage);
			}

			return new float[] { add, mult, flat, damage };
		}

		public double[] damageReduction()
		{
			double flatReduction = Main.LocalPlayer.statDefense * 0.5;
			double multiReduction = 1f;

			if (Main.expertMode)
			{
				flatReduction = Main.LocalPlayer.statDefense * 0.75;
			}



			multiReduction *= (1-Main.LocalPlayer.endurance);

			if (Main.LocalPlayer.setSolar && Main.LocalPlayer.solarShields >= 0)
			{
				float num3 = 0.3f;
				multiReduction *= (1-num3);

			}
			if (Main.LocalPlayer.beetleDefense && Main.LocalPlayer.beetleOrbs > 0)
			{
				float num5 = 0.15f * (float)Main.LocalPlayer.beetleOrbs;
				multiReduction *= (1-num5);
					
			}
			if (Main.LocalPlayer.defendedByPaladin)
			{
				bool flag4 = false;
				for (int l = 0; l < 255; l++)
				{
					if (l != Main.myPlayer && Main.player[l].active && !Main.player[l].dead && !Main.player[l].immune && Main.player[l].hasPaladinShield && Main.player[l].team == Main.LocalPlayer.team && (float)Main.player[l].statLife > (float)Main.player[l].statLifeMax2 * 0.25f)
					{
						flag4 = true;
						break;
					}
				}
				if (flag4)
				{
					multiReduction *= 0.75;
				}
			}

			return new double[] { flatReduction, multiReduction };
		}


		public List<AccDisplay> toDisplay = new List<AccDisplay>();

		public LansDisplayPlayerStats()
		{
			instance = this;
		}

		public override void Load()
		{
			base.Load();

			if (Main.netMode != NetmodeID.Server)
			{


				toDisplay.Add(new AccDisplay(this.GetTexture("towerpillars"), "Tower Power", delegate ()
				{
					return Main.LocalPlayer.ZoneTowerSolar || Main.LocalPlayer.ZoneTowerNebula || Main.LocalPlayer.ZoneTowerStardust || Main.LocalPlayer.ZoneTowerVortex;
				},
				delegate ()
				{
					int count = 0;
					if (Main.LocalPlayer.ZoneTowerSolar)
					{
						count = NPC.ShieldStrengthTowerSolar;
					}
					else if (Main.LocalPlayer.ZoneTowerNebula)
					{
						count = NPC.ShieldStrengthTowerNebula;
					}
					else if (Main.LocalPlayer.ZoneTowerStardust)
					{
						count = NPC.ShieldStrengthTowerStardust;
					}
					else if (Main.LocalPlayer.ZoneTowerVortex)
					{
						count = NPC.ShieldStrengthTowerVortex;
					}

					if (count <= 0)
					{
						return "Attackable";
					}
					return "Shield has " + count + " life.";
				}
				));


				toDisplay.Add(new AccDisplay(this.GetTexture("lifeRegen"), "Life Regeneration", delegate ()
				{
					return true;
				},
				delegate ()
				{
					var regen = Main.LocalPlayer.lifeRegen + (Main.LocalPlayer.palladiumRegen ? 6 : 0);
					var floatRegen = regen / 2.0f;
					return floatRegen.ToString("0.0");
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("manaRegen"), "Mana Regeneration", delegate ()
				{
					return true;
				},
				delegate ()
				{
					var regen = Main.LocalPlayer.manaRegen;
					var floatRegen = regen / 2.0f;
					return floatRegen.ToString("0.0");
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("armorPen"), "Armor Penetration", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return Main.LocalPlayer.armorPenetration.ToString();
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("manaCost"), "Mana Cost Multiplier", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return Main.LocalPlayer.manaCost.ToString("0.00");
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("useTime"), "Item Use Time", delegate ()
				{
					return true;
				},
				delegate ()
				{
					var sItem = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem];
					var itemAnimation = 0;
					if (sItem.melee)
					{
						itemAnimation = PlayerHooks.TotalMeleeTime(sItem.useAnimation * Main.LocalPlayer.meleeSpeed, Main.LocalPlayer, sItem);
					}
					else if (sItem.createTile >= 0)
					{
						itemAnimation = PlayerHooks.TotalMeleeTime(sItem.useAnimation * Main.LocalPlayer.tileSpeed, Main.LocalPlayer, sItem);
					}
					else if (sItem.createWall >= 0)
					{
						itemAnimation = PlayerHooks.TotalMeleeTime(sItem.useAnimation * Main.LocalPlayer.wallSpeed, Main.LocalPlayer, sItem);
					}
					else
					{

						itemAnimation = PlayerHooks.TotalMeleeTime(sItem.useAnimation, Main.LocalPlayer, sItem);
						itemAnimation = Math.Max(itemAnimation, (int)(sItem.reuseDelay / PlayerHooks.TotalUseTimeMultiplier(Main.LocalPlayer, sItem)));
					}


					return ((float)itemAnimation / 60.0f).ToString("0.00") + "s";
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("damage"), "Item Damage", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return getWeaponStats()[3].ToString("0");
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("flatDamage"), "Item Flat Damage", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return getWeaponStats()[2].ToString("0");
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("additionalDamage"), "Item Additional Multiplier", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return (getWeaponStats()[0] * 100).ToString("0") + "%";
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("multiDamage"), "Item Multiplier Multiplier", delegate ()
				{
					return false;
				},
				delegate ()
				{
					return (getWeaponStats()[1] * 100).ToString("0") + "%";
				}
				));


				toDisplay.Add(new AccDisplay(this.GetTexture("endurance"), "Endurance", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return (Main.LocalPlayer.endurance * 100).ToString("0") + "%";
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("flatReduction"), "Flat damage reduction", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return damageReduction()[0].ToString("0");
				}
				));

				toDisplay.Add(new AccDisplay(this.GetTexture("multiReduction"), "Damage reduction", delegate ()
				{
					return true;
				},
				delegate ()
				{
					return ((1 - damageReduction()[1]) * 100).ToString("0") + "%";
				}
				));


				IL.Terraria.Main.DrawInfoAccs += injectIntoDrawInfoAcc;


			}
		}


		public void injectIntoDrawInfoAcc(ILContext il)
		{
			var c = new ILCursor(il);


			if (c.TryGotoNext(i => i.MatchLdloc(15), i => i.MatchCall<String>("IsNullOrEmpty")))
			{

				c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, 15);
				
				c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, 13);
				c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, 14);
				c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, 16);
				c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, 17);

				c.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, typeof(Main).GetField("mH", (BindingFlags)(-1)));

				c.EmitDelegate<Action<string, int, int, float, int, int>>(
				delegate (string text, int num2, int num3, float num4, int num5, int mH)
				{

					LModPlayer mPlayer = Main.LocalPlayer.GetModPlayer<LModPlayer>();

					int rendered = num3;

					string refText = text;
					int refNum2 = num2;
					int refNum3 = num3;
					float refNum4 = num4;
					int refNum5 = num5;

					for (int cur = 0; cur < this.toDisplay.Count; cur++)
					{
						var display = toDisplay[cur];
						if (display.isVisible())
						{

							DisplayInfoHelper.render(mPlayer, cur, display.texture, ref refText, display.getAccValue(), display.infoText, rendered, ref refNum2, ref refNum3, ref refNum4, ref refNum5, mH);
							rendered = refNum3;
						}
					}
				});

			}


		}



	}
}