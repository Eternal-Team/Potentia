using PotentiaCore.TileEntities.Generators;
using Terraria;
using Terraria.ModLoader;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utils;

namespace PotentiaCore.UI.Generators
{
	public class WaterTurbineUI : BaseUI, ITileEntityUI
	{
		public TEWaterTurbine turbine;

		public UIText textLabel = new UIText(() => "Wind Turbine");
		public UIEnergyBar barEnergy;

		public UIPanel panelInfo = new UIPanel();

		public UIText textGeneration;
		//public UIText textEfficiency;

		public UITextButton button = new UITextButton("Reset", 6f);

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = Main.screenWidth / 7f;
			panelMain.Height.Precent = 0.25f;
			panelMain.Center();
			panelMain.SetPadding(0);
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel.Top.Pixels = 8;
			textLabel.HAlign = 0.5f;
			panelMain.Append(textLabel);

			barEnergy = new UIEnergyBar(turbine);
			barEnergy.Width.Pixels = 32;
			barEnergy.Height.Set(-16, 1);
			barEnergy.Left.Set(-barEnergy.Width.Pixels - 8, 1);
			barEnergy.Top.Pixels = 8;
			panelMain.Append(barEnergy);

			panelInfo.Width.Set(-56, 1);
			panelInfo.Height.Set(-44, 1);
			panelInfo.Left.Pixels = 8;
			panelInfo.Top.Pixels = 36;
			panelInfo.SetPadding(0);
			panelMain.Append(panelInfo);

			textGeneration = new UIText(() => $"Generating: {barEnergy.delta.AsPower(true)}");
			textGeneration.Left.Pixels = 8;
			textGeneration.Top.Pixels = 8;
			panelInfo.Append(textGeneration);

			//textEfficiency = new UIText(() => $"Efficiency: {turbine.efficiency:F2}%");
			//textEfficiency.Left.Pixels = 8;
			//textEfficiency.Top.Pixels = 36;
			//panelInfo.Append(textEfficiency);

			button.Left.Pixels = 8;
			button.Top.Pixels = 64;
			button.Height.Pixels = 40;
			button.Width.Set(-16, 1);
			button.OnClick += (a, b) => turbine.energy.SetValue("energy", 0);
			panelInfo.Append(button);
		}

		public void SetTileEntity(ModTileEntity tileEntity) => turbine = (TEWaterTurbine)tileEntity;
	}
}