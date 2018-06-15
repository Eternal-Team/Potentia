using System.IO;

namespace PotentiaCore.Global
{
	public static class Net
	{
		internal enum MessageType : byte
		{
		}

		public static void HandlePacket(BinaryReader reader, int sender)
		{
			MessageType type = (MessageType)reader.ReadByte();
			switch (type)
			{

			}
		}
	}
}