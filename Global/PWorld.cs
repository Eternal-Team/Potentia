using Microsoft.Xna.Framework.Graphics;
using Potentia.Cable;
using Potentia.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using TheOneLibrary.Utils;

namespace Potentia.Global
{
	public class PWorld : ModWorld
	{
		public static PWorld Instance;

		public int[,] oil;
		public int[,] gas;

		public CableLayer layer = new CableLayer();

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

		public override void PreUpdate()
		{
			layer.Update();
		}

		public override void PostDrawTiles()
		{
			RasterizerState rasterizer = Main.gameMenu || Math.Abs(Main.LocalPlayer.gravDir - 1.0) < 0.1 ? RasterizerState.CullCounterClockwise : RasterizerState.CullClockwise;
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

			layer.Draw(Main.spriteBatch);

			Main.spriteBatch.End();
		}

		public override TagCompound Save() => new TagCompound
		{
			["Width"] = oil.GetLength(0),
			["Height"] = oil.GetLength(1),
			["DataOil"] = oil.Cast<int>().ToList(),
			["DataGas"] = gas.Cast<int>().ToList(),
			["Layer"] = layer.Save()
		};

		public override void Load(TagCompound tag)
		{
			int width = tag.GetInt("Width");
			int height = tag.GetInt("Height");

			oil = tag.GetList<int>("DataOil").To2DArray(width, height);
			gas = tag.GetList<int>("DataGas").To2DArray(width, height);

			layer.Load(tag.GetList<TagCompound>("Layer").ToList());
		}

		// => TagIO.Write(Save(), writer);
		public override void NetSend(BinaryWriter writer)
		{
			TagIO.Write(new TagCompound { ["Layer"] = layer.Save() }, writer);
		}

		// => Load(TagIO.Read(reader));
		public override void NetReceive(BinaryReader reader)
		{
			layer.Load(TagIO.Read(reader).GetList<TagCompound>("Layer").ToList());
		}
	}
}