﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Potentia.Global;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TheOneLibrary.Base;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.Utils;

namespace Potentia
{
	public class Potentia : Mod
	{
		[Texture]
		public struct Textures
		{
			public const string Path = "Potentia/Textures/";
			public const string TilePath = Path + "Tiles/";
		}

		[Null] public static Potentia Instance;

		public Dictionary<object, GUI> UIs = new Dictionary<object, GUI>();
		public LegacyGameInterfaceLayer InventoryLayer;

		public override void Load()
		{
			Instance = this;

			InventoryLayer = new LegacyGameInterfaceLayer("Potentia: Inventory", () => UIs.Values.Draw(), InterfaceScaleType.UI);
		}

		public override void Unload()
		{
			Utility.UnloadNullableTypes();
		}

		public override void PreSaveAndQuit()
		{
			UIs.Clear();
		}

		public override void UpdateUI(GameTime gameTime)
		{
			List<BaseTE> list = UIs.Where(x => x.Key is BaseTE).Select(x => (BaseTE)x.Key).ToList();
			for (int i = 0; i < list.Count; i++) list[i].HandleUIFar();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int Hotbar = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
			if (Hotbar != -1) layers.Insert(Hotbar, InventoryLayer);
		}
	}

	public class TestUI : BaseUI
	{
		public override void Load()
		{
			RemoveAllChildren();

			Texture2D texture = new Texture2D(Main.instance.GraphicsDevice, PWorld.Instance.oil.GetLength(0), PWorld.Instance.oil.GetLength(1));
			Color[,] colors = new Color[texture.Width, texture.Height];

			for (int i = 0; i < texture.Width; i++)
			{
				for (int j = 0; j < texture.Height; j++)
				{
					colors[i, j] = Utility.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, PWorld.Instance.oil[i, j] / (float)PWorld.Instance.oil.Cast<int>().Max());
				}
			}

			texture.SetData(colors.Cast<Color>().ToArray());

			UITexture t = new UITexture(texture);
			t.Width.Set(texture.Width * 10, 0);
			t.Height.Set(texture.Height * 10, 0);
			t.HAlign = 0.5f;
			t.VAlign = 0.33f;
			Append(t);

			Texture2D texture1 = new Texture2D(Main.instance.GraphicsDevice, PWorld.Instance.gas.GetLength(0), PWorld.Instance.gas.GetLength(1));
			Color[,] colors1 = new Color[texture1.Width, texture1.Height];

			for (int i = 0; i < texture1.Width; i++)
			{
				for (int j = 0; j < texture1.Height; j++)
				{
					colors1[i, j] = Utility.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, PWorld.Instance.gas[i, j] / (float)PWorld.Instance.gas.Cast<int>().Max());
				}
			}

			texture1.SetData(colors1.Cast<Color>().ToArray());

			UITexture t1 = new UITexture(texture1);
			t1.Width.Set(texture1.Width * 10, 0);
			t1.Height.Set(texture1.Height * 10, 0);
			t1.HAlign = 0.5f;
			t1.VAlign = 0.66f;
			Append(t1);
		}
	}
}