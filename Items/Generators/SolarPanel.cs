using TheOneLibrary.Base;
using TheOneLibrary.Base.Items;

namespace Potentia.Items.Generators
{
	[EnergyTile]
	public class SolarPanel : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solar Panel");
			Tooltip.SetDefault("I'm walking on sunshine!");
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
			item.createTile = mod.TileType<Tiles.Generators.SolarPanel>();
		}
	}
}