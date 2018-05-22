using System.Collections.Generic;
using TheOneLibrary.Energy.Energy;

namespace Potentia.Cable
{
	public class CableGrid
	{
		public List<Cable> tiles = new List<Cable>();
		public EnergyStorage energy = new EnergyStorage();

		public long GetCapacitySharePerNode() => energy.GetCapacity() / tiles.Count;

		public long GetEnergySharePerNode() => energy.GetEnergy() / tiles.Count;

		public void AddTile(Cable tile)
		{
			if (!tiles.Contains(tile))
			{
				energy.AddCapacity(tile.maxIO * 2);
				energy.ModifyEnergyStored(tile.grid.GetEnergySharePerNode());
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
				CableGrid newGrid = new CableGrid();
				newGrid.energy.SetMaxTransfer(tiles[i].maxIO);
				newGrid.energy.SetCapacity(tiles[i].maxIO * 2);
				newGrid.energy.ModifyEnergyStored(GetEnergySharePerNode());
				newGrid.tiles.Add(tiles[i]);
				tiles[i].grid = newGrid;
			}

			for (int i = 0; i < tiles.Count; i++) tiles[i].Merge();
		}
	}
}