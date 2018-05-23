using Microsoft.Xna.Framework;
using Potentia.Global;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TheOneLibrary.Base.Items;
using TheOneLibrary.Utils;

namespace Potentia.Cable
{
	public class BasicCable : BaseItem
	{
		public int maxIO = 1000;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Basic Wire");
			Tooltip.SetDefault("Transfers electricity\nMax IO: 1000W");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.rare = 0;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTime = 10;
			item.useAnimation = 10;
			item.consumable = true;
			item.useTurn = true;
			item.autoReuse = true;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool ConsumeItem(Player player)
		{
			Point16 mouse = Utility.MouseToWorldPoint;

			if (player.altFunctionUse == 2) PWorld.Instance.layer.Remove(mouse, player);
			else return PWorld.Instance.layer.Place(mouse, player, item.type);

			return false;
		}

		public override bool UseItem(Player player) => Vector2.Distance(Main.MouseWorld, player.Center) < 160;
	}
}