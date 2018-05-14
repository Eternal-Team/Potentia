using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Potentia.Global;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.Utils;

namespace Potentia
{
	public class Potentia : Mod
	{
		public GUI<TestUI> ui;

		public override void Load()
		{
			ui = Utility.SetupGUI<TestUI>();
		}

		public override void Unload()
		{
			Utility.UnloadNullableTypes();
		}

		public override void PostUpdateInput()
		{
			if (Keys.X.IsKeyDown())
			{
				ui.@interface.IsVisible = !ui.@interface.IsVisible;
				if (ui.@interface.IsVisible) ui.ui.Load();
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			layers.Add(ui.InterfaceLayer);
		}
	}

	public class TestUI : BaseUI
	{
		public override void Load()
		{
			RemoveAllChildren();
			
			Texture2D texture = new Texture2D(Main.instance.GraphicsDevice, PWorld.Instance.oil.GetLength(0), PWorld.Instance.oil.GetLength(1));
			Color[,] colors = new Color[texture.Width, texture.Height];
			Main.NewText(PWorld.Instance.oil.Cast<int>().Count(x=>x>1E6));
			for (int i = 0; i < texture.Width; i++)
			{
				for (int j = 0; j < texture.Height; j++)
				{
					colors[i, j] = Utility.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, PWorld.Instance.oil[i, j] / (float)PWorld.Instance.oil.Cast<int>().Max());
				}
			}

			texture.SetData(colors.Cast<Color>().ToArray());

			UITexture t = new UITexture(texture);
			t.Width.Set(texture.Width*10, 0);
			t.Height.Set(texture.Height*10, 0);
			t.Center();
			Append(t);
		}
	}
}