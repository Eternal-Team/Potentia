using TheOneLibrary.Base;
using TheOneLibrary.Base.Items;

namespace PotentiaCore.Items
{
	[EnergyTile]
	public class WaterPump : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Water Pump");
			Tooltip.SetDefault("Those power plants need some cooling");
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
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType<Tiles.WaterPump>();
		}
	}
}