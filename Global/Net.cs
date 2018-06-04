using System.Collections.Generic;
using System.IO;
using System.Linq;
using Potentia.Grid;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Energy.Energy;

namespace Potentia.Global
{
	public static class Net
	{
		internal enum MessageType : byte
		{
			CablePlacement,
			CableRemovement,
			CableModification
		}

		public static void HandlePacket(BinaryReader reader, int sender)
		{
			MessageType type = (MessageType)reader.ReadByte();
			switch (type)
			{
				case MessageType.CablePlacement:
					ReceiveCablePlacement(reader, sender);
					break;
				case MessageType.CableRemovement:
					ReceiveCableRemovement(reader, sender);
					break;
				case MessageType.CableModification:
					ReceiveCableModification(reader, sender);
					break;
			}
		}

		public static void ReceiveCablePlacement(BinaryReader reader, int sender)
		{
			TagCompound tag = TagIO.Read(reader);
			Point16 position = tag.Get<Point16>("Position");
			string name = tag.GetString("Name");

			Cable cable = new Cable();
			cable.SetDefaults(name);
			cable.position = position;
			cable.layer = PWorld.Instance.layer;
			cable.grid = new CableGrid
			{
				energy = new EnergyStorage(cable.maxIO * 2, cable.maxIO),
				tiles = new List<Cable> { cable }
			};
			PWorld.Instance.layer.Add(position, cable);

			cable.Merge();
			cable.Frame();

			foreach (Cable merge in Cable.sides.Select(x => x + position).Where(PWorld.Instance.layer.ContainsKey).Select(x => PWorld.Instance.layer[x]).Where(x => x.name == name)) merge.Frame();

			if (Main.netMode == NetmodeID.Server) SendCablePlacement(position, name, sender);
		}

		public static void SendCablePlacement(Point16 position, string name, int excludedPlayer = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = Potentia.Instance.GetPacket();
			packet.Write((byte)MessageType.CablePlacement);
			TagIO.Write(new TagCompound
			{
				["Position"] = position,
				["Name"] = name
			}, packet);
			packet.Send(ignoreClient: excludedPlayer);
		}

		public static void ReceiveCableRemovement(BinaryReader reader, int sender)
		{
			Cable cable = PWorld.Instance.layer[TagIO.Read(reader).Get<Point16>("Cable")];

			cable.grid.tiles.Remove(cable.grid.tiles.First(x => x.position == cable.position));
			cable.grid.ReformGrid();
			PWorld.Instance.layer.Remove(cable.position);

			foreach (Point16 point in Cable.sides.Select(x => x + cable.position).Where(x => PWorld.Instance.layer.ContainsKey(x))) PWorld.Instance.layer[point].Frame();

			if (Main.netMode == NetmodeID.Server) SendCableRemovement(cable, sender);
		}

		public static void SendCableRemovement(Cable cable, int excludedPlayer = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = Potentia.Instance.GetPacket();
			packet.Write((byte)MessageType.CableRemovement);
			TagIO.Write(new TagCompound
			{
				["Cable"] = cable.position
			}, packet);
			packet.Send(ignoreClient: excludedPlayer);
		}

		public static void ReceiveCableModification(BinaryReader reader, int sender)
		{
		}

		public static void SendCableModification()
		{
		}
	}
}