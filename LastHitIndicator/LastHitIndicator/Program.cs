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
	class Program
	{
		static void Main(string[] args)
		{
			Game.PrintMessage("Not WOrking!!!!!!!!!!!!!!!!!!\n", MessageType.ChatMessage);
			LastHitIndicatorUI.Init();
			LastHitIndicator.Init();
		}
	}
}
