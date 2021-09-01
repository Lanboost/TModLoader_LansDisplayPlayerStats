using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LansDisplayPlayerStats
{
	class LModPlayer:ModPlayer
	{
		public bool[] displayInfoHide;

		public override bool CloneNewInstances => false;


		public LModPlayer()
		{
			var size = LansDisplayPlayerStats.instance.toDisplay.Count;
			displayInfoHide = new bool[size];
			for (int i = 0; i < size; i++)
			{
				displayInfoHide[i] = false;
			}
		}

		public override void clientClone(ModPlayer clientClone)
		{
			LModPlayer clone = clientClone as LModPlayer;

			var size = LansDisplayPlayerStats.instance.toDisplay.Count;
			for (int i = 0; i < size; i++)
			{
				clone.displayInfoHide[i] = displayInfoHide[i];
			}
		}

		public override TagCompound Save()
		{
			var tagComp = new TagCompound();
			var size = LansDisplayPlayerStats.instance.toDisplay.Count;

			byte[] byteArr = new byte[size];
			for (int i = 0; i<byteArr.Length; i++)
			{
				byteArr[i] = (byte)(displayInfoHide[i] ? 1 : 0);
			}

			tagComp.Add("LansDisplayPlayerStats", new TagCompound {
				{"displayInfoVisible", byteArr},
			});
			
			return tagComp;
		}

		public override void Load(TagCompound tag)
		{
			var size = LansDisplayPlayerStats.instance.toDisplay.Count;

			var tagComp = new TagCompound();

			try
			{
				var tempTag = tag.Get<TagCompound>("LansDisplayPlayerStats");
				if (tempTag != null)
				{

					var byteArr = tempTag.GetByteArray("displayInfoVisible");


					if (byteArr.Length == displayInfoHide.Length)
					{
						for (int i = 0; i < byteArr.Length; i++)
						{
							displayInfoHide[i] = byteArr[i] == 1 ? true : false;
						}
					}
					else
					{


					}

				}
			}
			catch
			{
			}
		}
	}
}
