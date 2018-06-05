using System;
using Potentia.Grid;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			CableModification,
			GridEnergy
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
				case MessageType.GridEnergy:
					ReceiveGridEnergy(reader, sender);
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
			Cable cable = PWorld.Instance.layer[TagIO.Read(reader).Get<Point16>("Position")];

			cable.grid.RemoveTile(cable);
			PWorld.Instance.layer.Remove(cable.position);

			foreach (Point16 point in Cable.sides.Select(x => x + cable.position).Where(x => PWorld.Instance.layer.ContainsKey(x))) PWorld.Instance.layer[point].Frame();

			if (Main.netMode == NetmodeID.Server) SendCableRemovement(cable.position, sender);
		}

		public static void SendCableRemovement(Point16 position, int excludedPlayer = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = Potentia.Instance.GetPacket();
			packet.Write((byte)MessageType.CableRemovement);
			TagIO.Write(new TagCompound
			{
				["Position"] = position
			}, packet);
			packet.Send(ignoreClient: excludedPlayer);
		}

		public static void ReceiveCableModification(BinaryReader reader, int sender)
		{
		}

		public static void SendCableModification()
		{
		}

		public static void ReceiveGridEnergy(BinaryReader reader, int sender)
		{
			TagCompound tag = TagIO.Read(reader);
			Point16 position = tag.Get<Point16>("Position");
			long delta = tag.GetLong("Energy");

			if (PWorld.Instance.layer.ContainsKey(position))
			{
				PWorld.Instance.layer[position].grid.energy.ModifyEnergyStored(delta);

				if (Main.netMode == NetmodeID.Server) SendGridEnergy(position, delta, sender);
			}
		}

		public static void SendGridEnergy(Point16 position, long delta, int excludedPlayer = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = Potentia.Instance.GetPacket();
			packet.Write((byte)MessageType.GridEnergy);
			TagIO.Write(new TagCompound
			{
				["Position"] = position,
				["Energy"] = delta
			}, packet);
			packet.Send(ignoreClient: excludedPlayer);
		}
	}
}