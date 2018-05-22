using Terraria.ID;
using TheOneLibrary.Base.Items;

namespace Potentia.Items
{
	public class Coal : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coal");
			Tooltip.SetDefault("Dinosaurs, perhaps you could use them to generate electricity");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.createTile = mod.TileType<Tiles.Coal>();
		}
	}
}