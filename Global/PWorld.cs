using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Potentia.Grid;
using Potentia.TileEntities.Generators;
using Potentia.Tiles;
using Terraria;
using Terraria.DataStructures;
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

		#region Generation
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
			if (ShiniesIndex != -1)
			{
				tasks.Insert(ShiniesIndex, new PassLegacy("Potentia:FossilisedFuels", progress =>
				{
					progress.Message = "Fossilising dinosaurs";

					GenerateOilGas();
					GenerateCoal();
				}));
			}
		}

		private void GenerateCoal()
		{
			for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 3E-05); k++)
			{
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY - 200);

				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(150, 200), mod.TileType<Coal>());
			}
		}

		private void GenerateOilGas()
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
		#endregion

		public override void PreUpdate()
		{
			layer.Update();
		}

		public float angle;

		public override void PostDrawTiles()
		{
			RasterizerState rasterizer = Main.gameMenu || Math.Abs(Main.LocalPlayer.gravDir - 1.0) < 0.1 ? RasterizerState.CullCounterClockwise : RasterizerState.CullClockwise;
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

			foreach (KeyValuePair<Point16, TileEntity> kvp in TileEntity.ByPosition.Where(x => x.Value is TEWindTurbine))
			{
				Vector2 position = new Vector2(kvp.Key.X * 16 - (int)Main.screenPosition.X + 8, kvp.Key.Y * 16 - (int)Main.screenPosition.Y + 22);

				Color color = Lighting.GetColor(kvp.Key.X, kvp.Key.Y);

				Main.spriteBatch.Draw(Potentia.Textures.turbineBladeTexture, position, null, color, MathHelper.ToRadians(angle), new Vector2(6, 42), Vector2.One, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Potentia.Textures.turbineBladeTexture, position, null, color, MathHelper.ToRadians(angle + 120f), new Vector2(6, 42), Vector2.One, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Potentia.Textures.turbineBladeTexture, position, null, color, MathHelper.ToRadians(angle + 240f), new Vector2(6, 42), Vector2.One, SpriteEffects.None, 0f);

				angle += Math.Abs(Main.windSpeed * 15f);
				if (angle > 360f) angle = 0;
			}

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

		public override void NetSend(BinaryWriter writer)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				TagIO.ToStream(Save(), stream);
				byte[] data = stream.ToArray();
				writer.Write(data.Length);
				writer.Write(data);
			}
		}

		public override void NetReceive(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			using (MemoryStream stream = new MemoryStream(reader.ReadBytes(count)))
			{
				Load(TagIO.FromStream(stream));
			}
		}
	}
}