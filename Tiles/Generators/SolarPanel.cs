using DawnOfIndustryPower.TileEntities.Generators;
using Microsoft.Xna.Framework;
using Potentia.UI.Generators;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheOneLibrary.Base;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Utils;

namespace Potentia.Tiles.Generators
{
	public class SolarPanel : BaseTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TESolarPanel>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Solar Panel");
			AddMapEntry(Color.LightYellow, name);
		}

		public override void RightClick(int i, int j)
		{
			int ID = mod.GetID<TESolarPanel>(i, j);
			if (ID == -1) return;

			TESolarPanel panel = (TESolarPanel)TileEntity.ByID[ID];
			GUI<SolarPanelUI> gui = panel.gui;
			if (!Potentia.Instance.TEUI.ContainsValue(gui))
			{
				gui.ui.Load();
				Potentia.Instance.TEUI.Add(panel, gui);
			}
			else
			{
				gui.ui.Unload();
				Potentia.Instance.TEUI.Remove(panel);
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			int ID = mod.GetID<TESolarPanel>(i, j);
			if (ID != -1) Potentia.Instance.CloseUI(ID);

			Item.NewItem(i * 16, j * 16, 48, 32, mod.ItemType<Items.Generators.SolarPanel>());
			mod.GetTileEntity<TESolarPanel>().Kill(i, j);
		}
	}
}