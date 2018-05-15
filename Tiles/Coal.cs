using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TheOneLibrary.Base;

namespace Potentia.Tiles
{
	public class Coal : BaseTile
	{
		public override string Texture => Potentia.Textures.TilePath + "Coal";

		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileLighted[Type] = true;

			drop = mod.ItemType<Items.Coal>();

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Coal");
			AddMapEntry(Color.Black, name);
		}
	}
}