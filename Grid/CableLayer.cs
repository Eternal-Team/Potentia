using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Potentia.Global;
using Potentia.Items.Cables;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Layer;
using TheOneLibrary.Utils;

namespace Potentia.Grid
{
	public class CableLayer : ModLayer<Cable>
	{
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Main.LocalPlayer.GetHeldItem().modItem == null) return;
			if (!(Main.LocalPlayer.GetHeldItem().modItem is BaseCable) && !Main.LocalPlayer.GetHeldItem().modItem.GetType().HasAttribute<EnergyTileAttribute>()) return;

			DrawPreview(Main.spriteBatch, Main.LocalPlayer.GetHeldItem().modItem.Name);

			Vector2 zero = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen) zero = Vector2.Zero;

			int startX = (int)((Main.screenPosition.X - zero.X) / 16f);
			int endX = (int)((Main.screenPosition.X + Main.screenWidth + zero.X) / 16f);
			int startY = (int)((Main.screenPosition.Y - zero.Y) / 16f);
			int endY = (int)((Main.screenPosition.Y + Main.screenHeight + zero.Y) / 16f);

			if (startX < 4) startX = 4;
			if (endX > Main.maxTilesX - 4) endX = Main.maxTilesX - 4;
			if (startY < 4) startY = 4;
			if (endY > Main.maxTilesY - 4) endY = Main.maxTilesY - 4;

			foreach (KeyValuePair<Point16, Cable> keyValuePair in this.Where(x => x.Key.X > startX && x.Key.X < endX && x.Key.Y > startY && x.Key.Y < endY)) keyValuePair.Value.Draw(spriteBatch);
		}

		public void DrawPreview(SpriteBatch spriteBatch, string name)
		{
			Point16 mouse = new Point16(Player.tileTargetX, Player.tileTargetY);

			if (!(!ContainsKey(mouse) && Main.LocalPlayer.GetHeldItem().modItem is BaseCable && Vector2.Distance(mouse.ToVector2() * 16, Main.LocalPlayer.Center) < 160)) return;

			Point16 frame = Cable.sides.Select(x => x + mouse).Select((x, i) => ContainsKey(x) && this[x].name == name && this[x].connections[i.Counterpart()] ? Cable.frameOffset[i] : Point16.Zero).Aggregate((x, y) => x + y);

			spriteBatch.Draw(Potentia.Textures.cableTexture, mouse.ToVector2() * 16 - Main.screenPosition, new Rectangle(frame.X, frame.Y, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

			foreach (Point16 point in Cable.sides.Select(x => x + mouse).Where(ContainsKey))
			{
				Point16 frameOther = Cable.sides.Select((x, i) => x + point == mouse && this[point].connections[i] ? Cable.frameOffset[i] : Point16.Zero).Aggregate((x, y) => x + y);

				spriteBatch.Draw(Potentia.Textures.cableTexture, point.ToVector2() * 16 - Main.screenPosition, new Rectangle(frameOther.X, frameOther.Y, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
			}
		}

		public override bool Place(Player player, string name)
		{
			Point16 mouse = new Point16(Player.tileTargetX, Player.tileTargetY);
			if (ContainsKey(mouse)) return false;

			Cable cable = new Cable();
			cable.SetDefaults(name);
			cable.position = mouse;
			cable.layer = this;
			cable.grid = new CableGrid
			{
				energy = new EnergyStorage(cable.maxIO * 2, cable.maxIO),
				tiles = new List<Cable> { cable }
			};
			Add(mouse, cable);

			cable.Merge();
			cable.Frame();

			foreach (Cable merge in Cable.sides.Select(x => x + mouse).Where(ContainsKey).Select(x => this[x]).Where(x => x.name == name)) merge.Frame();

			Net.SendCablePlacement(mouse, name);

			return true;
		}

		public override void Remove(Player player)
		{
			Point16 mouse = new Point16(Player.tileTargetX, Player.tileTargetY);
			if (ContainsKey(mouse)) this[mouse].Remove();
			//if (ContainsKey(mouse)) Info(player);
		}

		public override void Modify(Player player)
		{
			Point16 mouse = new Point16(Player.tileTargetX, Player.tileTargetY);
			if (ContainsKey(mouse)) this[mouse].Modify();
		}

		public override void Info(Player player)
		{
			Point16 mouse = new Point16(Player.tileTargetX, Player.tileTargetY);
			if (ContainsKey(mouse))
			{
				Cable cable = this[mouse];

				Main.NewText("Tiles: " + cable.grid.tiles.Count);
				Main.NewText("Current capacity: " + cable.grid.energy.GetCapacity());
				Main.NewText("Current IO: " + cable.grid.energy.GetMaxReceive() + "/" + cable.grid.energy.GetMaxExtract());
				Main.NewText("Current energy: " + cable.grid.energy.GetEnergy());
			}
		}

		public override void Update()
		{
			foreach (Cable cable in Values) cable.Update();
		}

		public override List<TagCompound> Save()
		{
			List<TagCompound> tags = new List<TagCompound>();
			foreach (KeyValuePair<Point16, Cable> pair in this)
			{
				TagCompound tag = new TagCompound();
				tag["Key"] = pair.Key;
				tag["Value"] = pair.Value.SaveAtt();
				tags.Add(tag);
			}

			return tags;
		}

		public override void Load(List<TagCompound> tags)
		{
			Clear();
			foreach (var pair in tags.Select(x => new KeyValuePair<Point16, Cable>(x.Get<Point16>("Key"), (Cable)new Cable().LoadAtt(x.GetCompound("Value")))).ToDictionary(x => x.Key, x => x.Value))
			{
				Cable cable = pair.Value;
				cable.maxIO = ((BaseCable)Potentia.Instance.GetItem(cable.name)).MaxIO;
				cable.layer = this;
				cable.grid = new CableGrid
				{
					energy = new EnergyStorage(cable.maxIO * 2, cable.maxIO).ModifyEnergyStored(cable.share),
					tiles = new List<Cable> { cable }
				};
				ErrorLogger.Log(cable.grid.energy);
				cable.share = 0;
				cable.Merge();
				Add(pair.Key, cable);
			}
		}
	}
}