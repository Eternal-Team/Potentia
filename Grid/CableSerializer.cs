﻿using System.Collections.Generic;
using System.Linq;
using Potentia.Global;
using Potentia.Items.Cables;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using TheOneLibrary.Energy.Energy;

namespace Potentia.Grid
{
	public class CableSerializer : TagSerializer<Cable, TagCompound>
	{
		public override TagCompound Serialize(Cable value) => new TagCompound
		{
			["Position"] = value.position,
			["Name"] = value.name,
			["IO"] = (int)value.IO,
			["Connections"] = value.connections,
			["Share"] = value.Share
		};

		public override Cable Deserialize(TagCompound tag)
		{
			Cable cable = new Cable();
			cable.position = tag.Get<Point16>("Position");
			cable.name = tag.GetString("Name");
			cable.IO = (IO)tag.GetInt("IO");
			cable.connections = tag.GetList<bool>("Connections").ToList();
			cable.MaxIO = ((BaseCable)Potentia.Instance.GetItem(cable.name)).MaxIO;
			cable.layer = PWorld.Instance.layer;
			cable.grid = new CableGrid
			{
				energy = new EnergyStorage(cable.MaxIO * 2, cable.MaxIO),
				tiles = new List<Cable> { cable }
			};
			cable.Share = tag.GetFloat("Share");

			return cable;
		}
	}
}