using System;
using System.Collections.Generic;
using Potentia.Tiles;
using Terraria;
using Terraria.ID;
using TheOneLibrary.Base;
using TheOneLibrary.Fluid;
using TheOneLibrary.Fluid.VanillaFluids;
using TheOneLibrary.Storage;
using TheOneLibrary.Utils;

namespace Potentia.TileEntities
{
	public class TEWaterPump : BaseTE, IFluidContainer
	{
		[Save, Sync] public ModFluid fluid;

		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<WaterPump>() && tile.TopLeft();

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 1);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 3);
			NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j - 1, Type);
			return -1;
		}

		public override void Update()
		{
			if (fluid == null) Utility.SetDefaults(ref fluid, FluidLoader.FluidType<Water>());

			int pumpRate = 0;
			for (int x = Position.X; x < Position.X + 3; x++)
			{
				for (int y = Position.Y; y < Position.Y + 2; y++)
				{
					if (Main.tile[x, y].liquidType() == Tile.Liquid_Water) pumpRate += Main.tile[x, y].liquid;
				}
			}

			fluid.volume += Math.Min(pumpRate, GetFluidCapacity() - fluid.volume);
		}

		public List<ModFluid> GetFluids() => new List<ModFluid> { fluid };

		public void SetFluid(ModFluid value, int slot = 0) => fluid = value;

		public ModFluid GetFluid(int slot = 0) => fluid;

		public int GetFluidCapacity(int slot = 0) => 32000;

		public void Sync(int slot = 0) => Utility.SendTEData(this);
	}
}