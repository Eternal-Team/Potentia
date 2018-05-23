using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Layer.Layer;
using TheOneLibrary.Utils;
using static TheOneLibrary.Base.Facing;

namespace Potentia.Cable
{
	public class CableLayer : ModLayer<Cable>
	{
		public override string Texture => TheOneLibrary.TheOneLibrary.Textures.Placeholder;

		public CableLayer()
		{
			DisplayName = Potentia.Instance.CreateTranslation("smh");
			DisplayName.SetDefault("Cable");
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!(Main.LocalPlayer.GetHeldItem().modItem is BasicCable) /*||Main.LocalPlayer.GetHeldItem()*/) return;

			Point16 mouse = Utility.MouseToWorldPoint;
			if (Main.LocalPlayer.GetHeldItem().modItem is BasicCable && !elements.ContainsKey(mouse) && Vector2.Distance(mouse.ToVector2() * 16, Main.LocalPlayer.Center) < 160) DrawPreview(Main.spriteBatch, mouse, Main.LocalPlayer.GetHeldItem().type);

			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen) zero = Vector2.Zero;

			int startX = (int)((Main.screenPosition.X - zero.X) / 16f);
			int endX = (int)((Main.screenPosition.X + Main.screenWidth + zero.X) / 16f);
			int startY = (int)((Main.screenPosition.Y - zero.Y) / 16f);
			int endY = (int)((Main.screenPosition.Y + Main.screenHeight + zero.Y) / 16f);

			if (startX < 4) startX = 4;
			if (endX > Main.maxTilesX - 4) endX = Main.maxTilesX - 4;
			if (startY < 4) startY = 4;
			if (endY > Main.maxTilesY - 4) endY = Main.maxTilesY - 4;

			for (int i = startX; i < endX; i++)
			{
				for (int j = startY; j < endY; j++)
				{
					if (elements.ContainsKey(i, j))
					{
						elements[i, j].Draw(spriteBatch);
					}
				}
			}
		}

		// ugh, oh

		public void DrawPreview(SpriteBatch spriteBatch, Point16 mouse, int type)
		{
			int frameX = 0;
			int frameY = 0;

			if (elements.ContainsKey(mouse.X - 1, mouse.Y) && elements[mouse.X - 1, mouse.Y].Name==Name&& elements[mouse.X - 1, mouse.Y].connections[Left]) frameX += 18;
			if (elements.ContainsKey(mouse.X + 1, mouse.Y) && elements[mouse.X + 1, mouse.Y].Name==Name&& elements[mouse.X + 1, mouse.Y].connections[Right]) frameX += 36;
			if (elements.ContainsKey(mouse.X, mouse.Y - 1) && elements[mouse.X, mouse.Y - 1].Name==Name&& elements[mouse.X, mouse.Y - 1].connections[Up]) frameY += 18;
			if (elements.ContainsKey(mouse.X, mouse.Y + 1) && elements[mouse.X, mouse.Y + 1].Name==Name&& elements[mouse.X, mouse.Y + 1].connections[Down]) frameY += 36;

			spriteBatch.Draw(Potentia.Textures.cableTexture, mouse.ToVector2() * 16 - Main.screenPosition, new Rectangle(frameX, frameY, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

			foreach (Point16 side in Cable.sides)
			{
				if (elements.ContainsKey(mouse + side))
				{
					int otherFrameX = 0;
					int otherFrameY = 0;

					if (mouse.X + 1 == mouse.X + side.X && elements[mouse.X + side.X, mouse.Y + side.Y].Name==Name && elements[mouse.X + side.X, mouse.Y + side.Y].connections[Left]) otherFrameX += 18;
					if (mouse.X - 1 == mouse.X + side.X && elements[mouse.X + side.X, mouse.Y + side.Y].Name==Name && elements[mouse.X + side.X, mouse.Y + side.Y].connections[Right]) otherFrameX += 36;
					if (mouse.Y + 1 == mouse.Y + side.Y && elements[mouse.X + side.X, mouse.Y + side.Y].Name==Name && elements[mouse.X + side.X, mouse.Y + side.Y].connections[Up]) otherFrameY += 18;
					if (mouse.Y - 1 == mouse.Y + side.Y && elements[mouse.X + side.X, mouse.Y + side.Y].Name == Name && elements[mouse.X + side.X, mouse.Y + side.Y].connections[Down]) otherFrameY += 36;

					spriteBatch.Draw(Potentia.Textures.cableTexture, (mouse + side).ToVector2() * 16 - Main.screenPosition, new Rectangle(otherFrameX, otherFrameY, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
				}
			}
		}

		public override void Update()
		{
			foreach (Cable wire in elements.Values)
			{
				Point16 check = Utility.TileEntityTopLeft(wire.position.X, wire.position.Y);

				if (TileEntity.ByPosition.ContainsKey(check))
				{
					CableGrid grid = wire.grid;
					TileEntity te = TileEntity.ByPosition[check];

					if ((wire.IO == Connection.In || wire.IO == Connection.Both) && te is IEnergyProvider)
					{
						IEnergyProvider provider = (IEnergyProvider)te;
						provider.GetEnergyStorage().ModifyEnergyStored(-grid.energy.ReceiveEnergy(Utility.Min(grid.energy.GetMaxReceive(), Utility.Min(grid.energy.GetCapacity() - grid.energy.GetEnergy(), provider.GetEnergyStorage().GetEnergy()))));

						// remove cables which are on a IEnergyStorage after you extract from it?
					}

					if ((wire.IO == Connection.Out || wire.IO == Connection.Both) && te is IEnergyReceiver)
					{
						IEnergyReceiver receiver = (IEnergyReceiver)te;
						receiver.GetEnergyStorage().ModifyEnergyStored(grid.energy.ExtractEnergy(Utility.Min(grid.energy.GetMaxExtract(), Utility.Min(grid.energy.GetEnergy(), receiver.GetCapacity() - receiver.GetEnergy()))));
					}
				}
			}
		}

		public override List<TagCompound> Save()
		{
			List<TagCompound> tags = new List<TagCompound>();
			foreach (KeyValuePair<Point16, Cable> pair in elements)
			{
				TagCompound tag = new TagCompound();
				tag["Key"] = pair.Key;
				tag["Value"] = pair.Value;
				tags.Add(tag);
			}

			return tags;
		}

		public override void Load(List<TagCompound> tags)
		{
			foreach (TagCompound tag in tags) elements.internalDict.Add(tag.Get<Point16>("Key"), tag.Get<Cable>("Value"));

			foreach (Cable wire in elements.Values)
			{
				CableGrid grid = new CableGrid();
				grid.energy.SetCapacity(wire.maxIO * 2);
				grid.energy.SetMaxTransfer(wire.maxIO);
				grid.tiles.Add(wire);
				grid.energy.ModifyEnergyStored(wire.share);
				wire.layer = this;
				wire.share = 0;
				wire.grid = grid;
			}

			foreach (Cable wire in elements.Values) wire.Merge();
		}

		public override bool Place(Point16 mouse, Player player, int type)
		{
			if (elements == null || elements.ContainsKey(mouse)) return false;

			// Creates a wire and places it in grid
			Cable wire = new Cable();
			wire.SetDefaults(type);
			wire.position = mouse;
			wire.layer = this;
			elements.Add(mouse, wire);

			// Creates a grid and set base values
			CableGrid grid = new CableGrid();
			grid.energy.SetMaxTransfer(wire.maxIO);
			grid.energy.SetCapacity(wire.maxIO * 2);
			grid.tiles.Add(wire);
			wire.grid = grid;

			// Merges and frames the wire
			wire.Merge();
			wire.Frame();

			// Frames all surrounding wires
			foreach (Point16 add in Cable.sides)
			{
				if (elements.ContainsKey(mouse.X + add.X, mouse.Y + add.Y))
				{
					Cable merge = elements[mouse + add];
					if (merge.Name == Name) merge.Frame();
				}
			}

			return true;
		}

		public override void Remove(Point16 mouse, Player player)
		{
			if (elements != null && elements.ContainsKey(mouse))
			{
				Cable cable = elements[mouse];

				cable.grid.tiles.Remove(cable);
				elements.Remove(mouse);

				cable.grid.ReformGrid();

				player.PutItemInInventory(Potentia.Instance.ItemType(cable.Name));

				foreach (Point16 check in Cable.sides)
					if (elements.ContainsKey(mouse + check))
						elements[mouse + check].Frame();
			}
		}

		public override void Modify(Point16 mouse)
		{
			Cable wire = elements[mouse];

			int x = (int)Main.MouseWorld.X - mouse.X * 16;
			int y = (int)Main.MouseWorld.Y - mouse.Y * 16;

			Rectangle io = new Rectangle(4, 4, 8, 8);
			if (!io.Contains(x, y))
			{
				if (Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(8, 8), new Point(0, 16)))
				{
					wire.connections[Left] = !wire.connections[Left];
					wire.Frame();

					if (!wire.connections[Left]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X - 1, mouse.Y))
					{
						Cable secCable = elements[mouse.X - 1, mouse.Y];
						secCable.connections[Right] = !secCable.connections[Right];
						if (wire.connections[Left]) wire.grid.MergeGrids(secCable.grid);
						secCable.Frame();
					}
				}
				else if (Utility.PointInTriangle(new Point(x, y), new Point(16, 0), new Point(16, 16), new Point(8, 8)))
				{
					wire.connections[Right] = !wire.connections[Right];
					wire.Frame();

					if (!wire.connections[Right]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X + 1, mouse.Y))
					{
						Cable secCable = elements[mouse.X + 1, mouse.Y];
						secCable.connections[Left] = !secCable.connections[Left];
						if (wire.connections[Right]) wire.grid.MergeGrids(secCable.grid);
						secCable.Frame();
					}
				}
				else if (Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(16, 0), new Point(8, 8)))
				{
					wire.connections[Up] = !wire.connections[Up];
					wire.Frame();

					if (!wire.connections[Up]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X, mouse.Y - 1))
					{
						Cable secCable = elements[mouse.X, mouse.Y - 1];
						secCable.connections[Down] = !secCable.connections[Down];
						if (wire.connections[Up]) wire.grid.MergeGrids(secCable.grid);
						secCable.Frame();
					}
				}
				else if (Utility.PointInTriangle(new Point(x, y), new Point(0, 16), new Point(8, 8), new Point(16, 16)))
				{
					wire.connections[Down] = !wire.connections[Down];
					wire.Frame();

					if (!wire.connections[Down]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X, mouse.Y + 1))
					{
						Cable secCable = elements[mouse.X, mouse.Y + 1];
						secCable.connections[Up] = !secCable.connections[Up];
						if (wire.connections[Down]) wire.grid.MergeGrids(secCable.grid);
						secCable.Frame();
					}
				}
			}
		}

		public override void Info(Point16 mouse)
		{
			Cable wire = elements[mouse];

			Main.NewText("Tiles: " + wire.grid.tiles.Count);
			Main.NewText("Current capacity: " + wire.grid.energy.GetCapacity());
			Main.NewText("Current IO: " + wire.grid.energy.GetMaxReceive() + "/" + wire.grid.energy.GetMaxExtract());
			Main.NewText("Current energy: " + wire.grid.energy.GetEnergy());
		}
	}
}