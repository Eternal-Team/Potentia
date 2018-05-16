﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Potentia.Tiles;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using TheOneLibrary.Base;
using TheOneLibrary.Utils;

namespace Potentia.Global
{
	public class PWorld : ModWorld
	{
		[Null] public static PWorld Instance;

		public int[,] oil;
		public int[,] gas;

		public float RandomWeighted(float min, float max, float cap, int percent) => Main.rand.Next(0, 101) > percent ? Main.rand.NextFloat(min, cap) : Main.rand.NextFloat(cap, max);

		public override void Initialize()
		{
			Instance = this;
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
			if (ShiniesIndex != -1)
			{
				tasks.Insert(ShiniesIndex, new PassLegacy("Potentia:FossilisedFuels", progress =>
				{
					progress.Message = "Fossilising dinosaurs";

					GenerateOilGas();

					for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 3E-05); k++)
					{
						int x = WorldGen.genRand.Next(0, Main.maxTilesX);
						int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY - 200);

						WorldGen.TileRunner(x, y, WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(150, 200), mod.TileType<Coal>());
					}
				}));
			}
		}

		public void GenerateOilGas()
		{
			oil = new int[Main.maxTilesX / 50, Main.maxTilesY / 50];
			gas = new int[Main.maxTilesX / 50, Main.maxTilesY / 50];

			for (int i = 0; i < oil.GetLength(0); i++)
			{
				for (int j = 0; j < oil.GetLength(1); j++)
				{
					oil[i, j] = (int)(1E6 * RandomWeighted(0.3f, 1.8f, 1f, 15));
					gas[i, j] = (int)(1E6 * RandomWeighted(0.5f, 2.5f, 1.6f, 15));
				}
			}
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			tag["Width"] = oil.GetLength(0);
			tag["Height"] = oil.GetLength(1);
			tag["DataOil"] = oil.Cast<int>().ToList();
			tag["DataGas"] = gas.Cast<int>().ToList();

			return tag;
		}

		public override void Load(TagCompound tag)
		{
			int width = tag.GetInt("Width");
			int height = tag.GetInt("Height");

			oil = tag.GetList<int>("DataOil").To2DArray(width, height);
			gas = tag.GetList<int>("DataGas").To2DArray(width, height);
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetReceive(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}