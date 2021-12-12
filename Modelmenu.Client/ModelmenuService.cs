using System.Threading.Tasks;
using JetBrains.Annotations;
using NFive.SDK.Client.Commands;
using NFive.SDK.Client.Communications;
using NFive.SDK.Client.Events;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Services;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Core.Input;
using Gaston11276.Fivemui;

namespace Gaston11276.Modelmenu.Client
{
	[PublicAPI]
	public class ModelMenuService : Service
	{
		WindowModelMenu windowModelChanger = new WindowModelMenu();

		public ModelMenuService(ILogger logger, ITickManager ticks, ICommunicationManager comms, ICommandManager commands, IOverlayManager overlay, User user) : base(logger, ticks, comms, commands, overlay, user) { }

		public override async Task Started()
		{
			WindowManager.AddWindow(windowModelChanger);
			windowModelChanger.CreateUi();
			await windowModelChanger.SetUi();
			windowModelChanger.SetHotkey(InputControl.SelectCharacterFranklin);
			
			await Delay(10);
		}
	}
}
