using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common.Menu;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using SharpDX.Direct3D9;

namespace LastHitIndicator
{
	internal class LastHitIndicatorUI
	{
		public static Font font = new Font(
			Drawing.Direct3DDevice9,
			new FontDescription
			{
				FaceName = "Tahoma",
				Height = 13,
				OutputPrecision = FontPrecision.Default,
				Quality = FontQuality.Default
			});

		public static string debugText1 = "1";
		public static string debugText2 = "2";
		public static string debugText3 = "3";

		private static readonly Menu Menu = new Menu("Last Hit Indicator", "lasthitindicator", true); //, "item_quelling_blade", true);

		public static void Init()
		{
			Menu.AddItem(new MenuItem("item1", "Test1").SetValue(new Slider(500, 100, 2000)));
			Menu.AddItem(new MenuItem("item2", "Test2").SetValue(false));

			Menu.AddToMainMenu();

			AppDomain.CurrentDomain.DomainUnload += CurrentDomainDomainUnload;
			Game.OnUpdate += OnUpdate;
			//Drawing.OnDraw += Drawing_OnDraw;
			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
		}

		private static void CurrentDomainDomainUnload(object sender, EventArgs e)
		{
			font.Dispose();
		}

		private static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame) return;

			font.DrawText(null, debugText1, 5, 60, Color.White);
			font.DrawText(null, debugText2, 5, 120, Color.White);
			font.DrawText(null, debugText3, 5, 180, Color.White);
		}

		private static void Drawing_OnPostReset(EventArgs args)
		{
			font.OnResetDevice();
		}
		private static void Drawing_OnPreReset(EventArgs args)
		{
			font.OnLostDevice();
		}

		public static void OnUpdate(EventArgs args)
		{
		}
	}
}
