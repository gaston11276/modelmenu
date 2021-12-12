using NFive.SDK.Core.Controllers;
using NFive.SDK.Core.Input;

namespace Gaston11276.Modelmenu.Shared
{
	public class Configuration : ControllerConfiguration
	{
		public InputControl HotkeyToggle { get; set; } = InputControl.SelectCharacterFranklin; // Default to F5
	}
}
