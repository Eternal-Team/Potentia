using System.Collections.Generic;
using TheOneLibrary.Energy.Energy;

namespace Potentia.Grid
{
	public class CableGrid
	{
		public List<Cable> tiles = new List<Cable>();
		public EnergyStorage energy = new EnergyStorage();

		public long GetEnergySharePerNode
		{
			get { return energy.GetEnergy() / tiles.Count; }
		}

		public void AddTile(Cable tile)
		{
			if (!tiles.Contains(tile))
			{
				energy.AddCapacity(tile.maxIO * 2);
				energy.ModifyEnergyStored(tile.grid.GetEnergySharePerNode);
				tile.grid = this;
				tiles.Add(tile);
			}
		}

		public void MergeGrids(CableGrid wireGrid)
		{
			for (int i = 0; i < wireGrid.tiles.Count; i++) AddTile(wireGrid.tiles[i]);
		}

		public void ReformGrid()
		{
			for (int i = 0; i < tiles.Count; i++)
			{
				tiles[i].grid = new CableGrid
				{
					energy = new EnergyStorage(tiles[i].maxIO * 2, tiles[i].maxIO).ModifyEnergyStored(GetEnergySharePerNode),
					tiles = new List<Cable> { tiles[i] }
				};
			}

			for (int i = 0; i < tiles.Count; i++) tiles[i].Merge();
		}
	}
}