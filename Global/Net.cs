using System.IO;

namespace Potentia.Global
{
	public static class Net
	{
		internal enum MessageType : byte
		{
		}

		public static void HandlePacket(BinaryReader reader, int sender)
		{
			MessageType type = (MessageType)reader.ReadByte();
			//switch (type)
			//{
			//	case MessageType.SyncQEItem:
			//		ReceiveQEItem(reader, sender);
			//		break;
			//	case MessageType.SyncQEFluid:
			//		ReceiveQEFluid(reader, sender);
			//		break;
			//}
		}

		//#region Items
		//public static void ReceiveQEItem(BinaryReader reader, int sender)
		//{
		//	TagCompound tag = TagIO.Read(reader);

		//	Frequency frequency = tag.Get<Frequency>("Frequency");
		//	int slot = tag.GetInt("Slot");
		//	Item item = ItemIO.Load(tag.GetCompound("Item"));

		//	if (!PSWorld.Instance.enderItems.ContainsKey(frequency)) PSWorld.Instance.enderItems.Add(frequency, Enumerable.Repeat(new Item(), 27).ToList());
		//	PSWorld.Instance.enderItems[frequency][slot] = item;

		//	if (Main.netMode == NetmodeID.Server) SendQEItem(frequency, slot, sender);
		//}

		//public static void SendQEItem(Frequency frequency, int slot, int excludedPlayer = -1)
		//{
		//	if (Main.netMode == NetmodeID.SinglePlayer) return;

		//	ModPacket packet = PortableStorage.Instance.GetPacket();
		//	packet.Write((byte)MessageType.SyncQEItem);
		//	TagIO.Write(new TagCompound
		//	{
		//		["Frequency"] = frequency,
		//		["Slot"] = slot,
		//		["Item"] = ItemIO.Save(PSWorld.Instance.enderItems[frequency][slot])
		//	}, packet);
		//	packet.Send(ignoreClient: excludedPlayer);
		//}
		//#endregion
	}
}