using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.Map;
using Terraria.UI.Gamepad;

namespace LansDisplayPlayerStats
{
	class DisplayInfoHelper
	{
		public static int columns = 12;
		public static void render(LModPlayer mPlayer, int displayIndex, Texture2D texture, ref string text, string text2, string text3,  int i, ref int num2, ref int num3, ref float num4, ref int num5, int mH)
		{
			int num = (!mPlayer.displayInfoHide[displayIndex] || Main.playerInventory) ? 1:0;
			int num26;
			int num27;
			if (!Main.playerInventory)
			{
				num26 = Main.screenWidth - 280;
				num27 = -32;
				if (Main.mapStyle == 1 && Main.mapEnabled)
				{
					num27 += 254;
				}
			}
			else
			{
				bool shouldDrawInfoIconsHorizontally = Main.ShouldDrawInfoIconsHorizontally;
				if (shouldDrawInfoIconsHorizontally)
				{
					int column = num3 % columns;

					num26 = Main.screenWidth - 280 + 20 * column - 10;

					num27 = 94;
					num27 += (num3 / columns)*25;

					if (Main.mapStyle == 1 && Main.mapEnabled)
					{
						num27 += 254;
					}
				}
				else
				{
					int num28 = (int)(52f * Main.inventoryScale);
					num26 = 697 - num28 * 4 + Main.screenWidth - 800 + 20 * (num3 % 2);
					num27 = 114 + mH + num28 * 7 + num28 / 2 + 20 * (num3 / 2) + 8 * (num3 / 4) - 20;
					if (Main.EquipPage == 2)
					{
						num26 += num28 + num28 / 2;
						num27 -= num28;
					}
				}
			}
			num26 += num5;
			if (num > 0)
			{
				num3++;
				int num29 = 22;
				if (Main.screenHeight < 650)
				{
					num29 = 20;
				}
				Vector2 vector = new Vector2((float)num26, (float)(num27 + 74 + num29 * i + 52));
				int num30 = num;
				if (num30 == 8)
				{
					num30 = 7;
				}
				Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
				bool flag14 = false;
				if (Main.playerInventory)
				{
					vector = new Vector2((float)num26, (float)num27);
					if ((float)Main.mouseX >= vector.X && (float)Main.mouseY >= vector.Y && (float)Main.mouseX <= vector.X + (float)texture.Width && (float)Main.mouseY <= vector.Y + (float)texture.Height && !PlayerInput.IgnoreMouseInterface)
					{
						flag14 = true;
						Main.player[Main.myPlayer].mouseInterface = true;

						if (Main.mouseLeft && Main.mouseLeftRelease)
						{
							Main.PlaySound(12, -1, -1, 1, 1f, 0f);
							Main.mouseLeftRelease = false;
							mPlayer.displayInfoHide[displayIndex] = !mPlayer.displayInfoHide[displayIndex];
						}

						if (!Main.mouseText)
						{
							text = text3;
							Main.mouseText = true;
						}
					}

					if (mPlayer.displayInfoHide[displayIndex])
					{
						white = new Microsoft.Xna.Framework.Color(80, 80, 80, 70);
					}
				}
				else if ((float)Main.mouseX >= vector.X && (float)Main.mouseY >= vector.Y && (float)Main.mouseX <= vector.X + (float)texture.Width && (float)Main.mouseY <= vector.Y + (float)texture.Height && !Main.mouseText)
				{
					num2 = i;
					text = text3;
					Main.mouseText = true;
				}
				Main.spriteBatch.Draw(texture, vector, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height)), white, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				if (flag14)
				{
					Main.spriteBatch.Draw(texture, vector - Vector2.One * 2f, null, Main.OurFavoriteColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				}
				num26 += 20;
			}
			if (num > 0 && !Main.playerInventory)
			{
				Vector2 scale = new Vector2(1f);
				Vector2 vector2 = Main.fontMouseText.MeasureString(text2);
				if (vector2.X > num4)
				{
					scale.X = num4 / vector2.X;
				}
				if (scale.X < 0.58f)
				{
					scale.Y = 1f - scale.X / 3f;
				}
				for (int num31 = 0; num31 < 5; num31++)
				{
					int num32 = 0;
					int num33 = 0;
					Microsoft.Xna.Framework.Color black = Microsoft.Xna.Framework.Color.Black;
					if (num31 == 0)
					{
						num32 = -2;
					}
					if (num31 == 1)
					{
						num32 = 2;
					}
					if (num31 == 2)
					{
						num33 = -2;
					}
					if (num31 == 3)
					{
						num33 = 2;
					}
					if (num31 == 4)
					{
						black = new Microsoft.Xna.Framework.Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor);
					}
					if (i > num2 && i < num2 + 2)
					{
						black = new Microsoft.Xna.Framework.Color((int)(black.R / 3), (int)(black.G / 3), (int)(black.B / 3), (int)(black.A / 3));
					}
					int num34 = 22;
					if (Main.screenHeight < 650)
					{
						num34 = 20;
					}
					Main.spriteBatch.DrawString(Main.fontMouseText, text2, new Vector2((float)(num26 + num32), (float)(num27 + 74 + num34 * i + num33 + 48)), black, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (Main.playerInventory)
				{
					Main.player[Main.myPlayer].mouseInterface = true;
				}
				Main.instance.MouseText(text, 0, 0, -1, -1, -1, -1);
			}
		}
	}
}
