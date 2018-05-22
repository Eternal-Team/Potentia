using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Potentia.Cable
{
	public class CableSerializer : TagSerializer<Cable, TagCompound>
	{
		public override TagCompound Serialize(Cable value) => new TagCompound
		{
			["Type"] = value.Name,
			["Position"] = value.position,
			["frameX"] = value.frameX,
			["frameY"] = value.frameY,
			["IO"] = (int)value.IO,
			//["Facing"] = value..Select(x => (int)x).ToList(),
			["Connection"] = value.connections.Values.ToList(),
			["Energy"] = value.grid.GetEnergySharePerNode()
		};

		public override Cable Deserialize(TagCompound tag)
		{
			Cable wire = new Cable();
			wire.Name = tag.GetString("Type");
			wire.SetDefaults(Potentia.Instance.ItemType(wire.Name));
			wire.position = tag.Get<Point16>("Position");
			wire.frameX = tag.GetShort("frameX");
			wire.frameY = tag.GetShort("frameY");
			wire.IO = (Connection)tag.GetInt("IO");
			wire.connections.Values = tag.GetList<bool>("Connection").ToArray();
			wire.share = tag.GetLong("Energy");
			return wire;
		}
	}
}