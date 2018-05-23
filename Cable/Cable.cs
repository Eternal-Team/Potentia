using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Layer.Layer;
using TheOneLibrary.Utils;
using static TheOneLibrary.Base.Facing;

namespace Potentia.Cable
{
	public enum Connection
	{
		In,
		Out,
		Both,
		Blocked
	}

	public class Cable : ModLayerElement
	{
		public static readonly Point16[] sides =
		{
			new Point16(-1, 0),
			new Point16(1, 0),
			new Point16(0, -1),
			new Point16(0, 1)
		};

		public CableGrid grid;
		public CableLayer layer;

		public Point16 position;
		public string Name;
		public short frameX;
		public short frameY;
		public long share;
		public long maxIO;

		public Connection IO = Connection.Both;

		public FacingIndexedArray<bool> connections = new FacingIndexedArray<bool> { true, true, true, true };

		public void SetDefaults(int type)
		{
			ModItem modItem = ItemLoader.GetItem(type);
			Name = modItem.Name;
			maxIO = ((BasicCable)modItem).maxIO;
		}

		public void Frame()
		{
			P16Dictionary<Cable> wires = layer.elements;

			frameX = 0;
			frameY = 0;

			if (wires.ContainsKey(position.X - 1, position.Y) && wires[position.X - 1, position.Y].Name == Name && connections[Left]) frameX += 18;
			if (wires.ContainsKey(position.X + 1, position.Y) && wires[position.X + 1, position.Y].Name == Name && connections[Right]) frameX += 36;
			if (wires.ContainsKey(position.X, position.Y - 1) && wires[position.X, position.Y - 1].Name == Name && connections[Up]) frameY += 18;
			if (wires.ContainsKey(position.X, position.Y + 1) && wires[position.X, position.Y + 1].Name == Name && connections[Down]) frameY += 36;
		}

		public void Merge()
		{
			P16Dictionary<Cable> wires = layer.elements;

			foreach (Point16 check in Utility.CheckNeighbours())
			{
				Point16 point = position + check;

				Cable wire = wires.ContainsKey(point) ? wires[point] : null;

				if (wire != null)
				{
					if (check.X == -1 && connections[Left] && wire.connections[Right]) grid.MergeGrids(wire.grid);
					else if (check.X == 1 && connections[Right] && wire.connections[Left]) grid.MergeGrids(wire.grid);
					if (check.Y == -1 && connections[Up] && wire.connections[Down]) grid.MergeGrids(wire.grid);
					else if (check.Y == 1 && connections[Down] && wire.connections[Up]) grid.MergeGrids(wire.grid);
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 position = -Main.screenPosition + this.position.ToVector2() * 16;
			Point16 pos = Utility.TileEntityTopLeft(this.position.X, this.position.Y);
			Color color = Lighting.GetColor(pos.X, pos.Y);
			spriteBatch.Draw(Potentia.Textures.cableTexture, position, new Rectangle(frameX, frameY, 16, 16), color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

			TileEntity te = TileEntity.ByPosition.ContainsKey(pos) ? TileEntity.ByPosition[pos] : null;
			if (te != null && (te is IEnergyReceiver || te is IEnergyProvider)) spriteBatch.Draw(Potentia.Textures.cableIOTexture, position + new Vector2(4), new Rectangle((int)IO * 8, 0, 8, 8), Color.White);
		}
	}
}