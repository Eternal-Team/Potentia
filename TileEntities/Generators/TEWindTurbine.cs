using Potentia.Tiles.Generators;
using Terraria;
using Terraria.ID;
using TheOneLibrary.Base;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Utils;

namespace DawnOfIndustryPower.TileEntities.Generators
{
	public class TEWindTurbine : BaseTE, IEnergyProvider
	{
		[Save, Sync] public EnergyStorage energy = new EnergyStorage(100000, 5000);
		
		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<WindTurbine>() && tile.TopLeft();

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 5);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 5, 6);
			NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j - 5, Type);
			return -1;
		}

		//public override void Update()
		//{
		//	int reverseHeight = Main.maxTilesY - Position.Y + 1;
		//	energyGen = Math.Min(reverseHeight, energy.GetCapacity() - energy.GetEnergy());

		//	//energy.ModifyEnergyStored(energyGen);

		//	this.HandleUIFar();
		//}

		public long GetEnergy() => energy.GetEnergy();

		public long GetCapacity() => energy.GetCapacity();

		public EnergyStorage GetEnergyStorage() => energy;

		public long ExtractEnergy(long maxExtract) => energy.ExtractEnergy(maxExtract);
	}
}