using Microsoft.Xna.Framework;
using PotentiaCore.TileEntities.Generators;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheOneLibrary.Base;
using TheOneLibrary.Utils;

namespace PotentiaCore.Tiles.Generators
{
	public class WaterTurbine : BaseTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEWaterTurbine>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Water Turbine");
			AddMapEntry(Color.LightYellow, name);
		}

		public override void RightClick(int i, int j)
		{
			TEWaterTurbine panel = mod.GetTileEntity<TEWaterTurbine>(i, j);
			panel?.HandleUI();
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TEWaterTurbine panel = mod.GetTileEntity<TEWaterTurbine>(i, j);
			panel?.CloseUI();

			Item.NewItem(i * 16, j * 16, 48, 32, mod.ItemType<Items.Generators.WaterTurbine>());
			mod.GetTileEntity<TEWaterTurbine>().Kill(i, j);
		}
	}
}