using Potentia.Tiles.Generators;
using Potentia.UI.Generators;
using Terraria;
using Terraria.ID;
using TheOneLibrary.Base;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Utils;

namespace Potentia.TileEntities.Generators
{
	public class TEWaterTurbine : BaseTE, IEnergyProvider
	{
		public GUI<WaterTurbineUI> gui;
		[Save, Sync] public EnergyStorage energy = new EnergyStorage(250000, 10000);

		public TEWaterTurbine()
		{
			if (Main.netMode != NetmodeID.Server) gui = Utility.SetupGUI<WaterTurbineUI>(this);
		}

		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<WaterTurbine>() && tile.TopLeft();

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 1);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 3);
			NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j - 1, Type);
			return -1;
		}

		//public override void Update()
		//{
		//	if (++waterScanTimer >= 150)
		//	{
		//		Point16 start = Utility.GetDirection(Position.X, Position.Y, Main.tile[Position.X, Position.Y].type) == TileObjectDirection.PlaceLeft ? new Point16(Position.X - 1, Position.Y + 1) : new Point16(Position.X + 3, Position.Y + 1);
		//		Utility.Trace(start, tile => !tile.active() && tile.liquidType() == Tile.Liquid_Water && tile.liquid > 0, tile => waterVolume += tile.liquidType() == Tile.Liquid_Water ? tile.liquid : 0);
		//		waterVolume = Math.Min(waterVolume, 255 * 100);
		//		waterScanTimer = 0;
		//	}

		//	energyGen = (long)Math.Min(waterVolume / 255f * 50, energy.GetCapacity() - energy.GetEnergy());

		//	this.HandleUIFar();
		//}

		public long GetEnergy() => energy.GetEnergy();

		public long GetCapacity() => energy.GetCapacity();

		public EnergyStorage GetEnergyStorage() => energy;

		public long ExtractEnergy(long maxExtract) => energy.ExtractEnergy(maxExtract);
	}
}