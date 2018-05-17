using TheOneLibrary.Base.Items;

namespace Potentia.Items.Generators
{
	public class WaterTurbine : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Water Turbine");
			Tooltip.SetDefault("Go with the flow");
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
			item.createTile = mod.TileType<Tiles.Generators.WaterTurbine>();
		}
	}
}