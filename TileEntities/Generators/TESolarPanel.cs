using System;
using Potentia.Tiles.Generators;
using Potentia.UI.Generators;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using TheOneLibrary.Base;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Utils;

namespace DawnOfIndustryPower.TileEntities.Generators
{
	public class TESolarPanel : BaseTE, IEnergyProvider
	{
		[Save, Sync] public EnergyStorage energy = new EnergyStorage(50000, 2500);
		public float efficiency;
		public GUI<SolarPanelUI> gui;

		public TESolarPanel()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				SolarPanelUI ui = new SolarPanelUI();
				ui.SetTileEntity(this);
				UserInterface userInterface = new UserInterface();
				ui.Activate();
				userInterface.SetState(ui);
				gui = new GUI<SolarPanelUI>(ui, userInterface);
			}
		}

		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<SolarPanel>() && tile.TopLeft();

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j);

			NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
			NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
			return -1;
		}

		public override void Update()
		{
			efficiency = Main.dayTime ? (float)(Main.time < 13500 ? Main.time / 13500 : 13500 / (Main.time - 13500))*100f : 0;

			energy.ModifyEnergyStored(Math.Min((long)(10 * efficiency), energy.GetCapacity() - energy.GetEnergy()));
		}

		public long GetEnergy() => energy.GetEnergy();

		public long GetCapacity() => energy.GetCapacity();

		public EnergyStorage GetEnergyStorage() => energy;

		public long ExtractEnergy(long maxExtract) => energy.ExtractEnergy(maxExtract);
	}
}