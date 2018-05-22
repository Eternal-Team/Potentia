using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
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
		public int type; // just keep Name
		public short frameX;
		public short frameY;
		public long share;
		public long maxIO;

		public Connection IO = Connection.Both;

		public FacingIndexedArray<bool> connections = new FacingIndexedArray<bool> { true, true, true, true };

		public void SetDefaults(int type)
		{
			this.type = type;
			Item item = new Item();
			item.SetDefaults(type);
			Name = item.modItem.GetType().Name;
			maxIO = ((BasicCable)item.modItem).maxIO;
		}

		public void Frame()
		{
			P16Dictionary<Cable> wires = layer.elements;

			frameX = 0;
			frameY = 0;

			if (wires.ContainsKey(position.X - 1, position.Y) && wires[position.X - 1, position.Y].type == type && connections[Left]) frameX += 18;
			if (wires.ContainsKey(position.X + 1, position.Y) && wires[position.X + 1, position.Y].type == type && connections[Right]) frameX += 36;
			if (wires.ContainsKey(position.X, position.Y - 1) && wires[position.X, position.Y - 1].type == type && connections[Up]) frameY += 18;
			if (wires.ContainsKey(position.X, position.Y + 1) && wires[position.X, position.Y + 1].type == type && connections[Down]) frameY += 36;
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
			Vector2 position = -Main.screenPosition + new Vector2(this.position.X, this.position.Y) * 16;
			spriteBatch.Draw(Potentia.Textures.cableTexture, position, new Rectangle(frameX, frameY, 16, 16), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

			//Vector2 tePosVec = (position + Main.screenPosition) / 16;
			//Point16 tePos = TheOneLibrary.Utils.Utility.TileEntityTopLeft((int)tePosVec.X, (int)tePosVec.Y);
			//TileEntity tileEntity = TileEntity.ByPosition.ContainsKey(tePos) ? TileEntity.ByPosition[tePos] : null;
			//if (tileEntity != null && (tileEntity is IEnergyReceiver || tileEntity is IEnergyProvider))
			//{
			//	switch (IO)
			//	{
			//		case Connection.In:
			//			Main.spriteBatch.Draw(DawnOfIndustryCore.inTexture, position + new Vector2(4), Color.White);
			//			break;
			//		case Connection.Out:
			//			Main.spriteBatch.Draw(DawnOfIndustryCore.outTexture, position + new Vector2(4), Color.White);
			//			break;
			//		case Connection.Both:
			//			Main.spriteBatch.Draw(DawnOfIndustryCore.bothTexture, position + new Vector2(4), Color.White);
			//			break;
			//		case Connection.Blocked:
			//			Main.spriteBatch.Draw(DawnOfIndustryCore.blockedTexture, position + new Vector2(4), Color.White);
			//			break;
			//	}
			//}
		}
	}
}