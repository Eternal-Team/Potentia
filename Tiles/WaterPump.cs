using Microsoft.Xna.Framework;
using Potentia.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheOneLibrary.Base;
using TheOneLibrary.Utils;

namespace Potentia.Tiles
{
	public class WaterPump : BaseTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile = TileObjectData.Style3x2;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEWaterPump>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Water Pump");
			AddMapEntry(Color.LightBlue, name);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TEWaterPump pump = mod.GetTileEntity<TEWaterPump>(i, j);
			pump.CloseUI();

			Item.NewItem(i * 16, j * 16, 80, 48, mod.ItemType<Items.WaterPump>());
			mod.GetTileEntity<TEWaterPump>().Kill(i, j);
		}
	}
}