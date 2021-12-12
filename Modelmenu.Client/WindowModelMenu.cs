using System.Threading.Tasks;
using CitizenFX.Core;
using Gaston11276.SimpleUi;
using Gaston11276.Fivemui;

namespace Gaston11276.Modelmenu.Client
{
	public class WindowModelMenu: Window
	{
		PanelModel panelModel = new PanelModel();
		
		public WindowModelMenu()
		{
			defaultPadding = 0.0025f;	
		}

		protected override void OnOpen()
		{
			UiCamera.SetMode(CameraMode.Front);
			Game.Player.CanControlCharacter = false;
			panelModel.SetCurrent(panelModel.FindModelIndex(Game.Player.Character.Model));
			base.OnOpen();
		}

		protected override void OnClose()
		{
			UiCamera.SetMode(CameraMode.Game);
			Game.Player.CanControlCharacter = true;
			base.OnClose();
		}

		public override void OnMouseButton(int state, int button, float CursorX, float CursorY)
		{
			if (IsOpen())
			{
				base.OnMouseButton(state, button, CursorX, CursorY);

				if (!UiCamera.rotatingCamera && state == 1 && button == 2)
				{
					UiCamera.rotatingCamera = true;
					UiCamera.lastCursorX = CursorX;
					UiCamera.lastCursorY = CursorY;
				}
				else if (UiCamera.rotatingCamera && state == 3 && button == 2)
				{
					UiCamera.rotatingCamera = false;
				}
			}
		}

		public async Task SetUi()
		{
			await panelModel.SetUi();
		}

		protected void CreateColumn(UiElementFiveM uiPanel, HGravity gravity, UiElementFiveM uiColumn, string label = null)
		{
			uiColumn.SetOrientation(Orientation.Vertical);
			uiColumn.SetPadding(new UiRectangle(defaultPadding));
			uiColumn.SetGravity(gravity);
			uiColumn.SetFlags(TRANSPARENT);
			uiPanel.AddElement(uiColumn);

			if (label != null)
			{
				Textbox header = new Textbox();
				header.SetPadding(new UiRectangle(defaultPadding));
				header.SetText(label);
				header.SetFlags(TRANSPARENT);
				if (label.Length == 0)
				{
					header.SetTextFlags(TRANSPARENT);
				}
				uiColumn.AddElement(header);
			}
		}

		public override void CreateUi()
		{
			base.CreateUi();

			SetFlags(HIDDEN);
			SetPadding(new UiRectangle(defaultPadding));
			SetAlignment(HAlignment.Right);
			SetProperties(FLOATING | MOVABLE | COLLISION_PARENT);
			SetOrientation(Orientation.Vertical);

			Textbox header = new Textbox();
			header.SetText("Model");
			header.SetFlags(TRANSPARENT);
			header.SetPadding(new UiRectangle(defaultPadding));
			AddElement(header);

			panelModel.CreateUi();
			AddElement(panelModel);

			UiElementFiveM panelButtons = new UiElementFiveM();
			panelButtons.SetHDimension(Dimension.Fill);
			panelButtons.SetPadding(new UiRectangle(0f));
			panelButtons.SetFlags(TRANSPARENT);
			panelButtons.SetMargin(new UiRectangle(0f, -defaultPadding, 0f, 0f));
			AddElement(panelButtons);

			UiElementFiveM panelButtonsLeft = new UiElementFiveM();
			panelButtonsLeft.SetGravity(HGravity.Left);
			panelButtonsLeft.SetPadding(new UiRectangle(0f));
			panelButtonsLeft.SetFlags(TRANSPARENT);
			panelButtonsLeft.SetHDimension(Dimension.Fill);
			panelButtons.AddElement(panelButtonsLeft);

			UiElementFiveM panelButtonsRight = new UiElementFiveM();
			panelButtonsRight.SetPadding(new UiRectangle(0f));
			panelButtonsRight.SetFlags(TRANSPARENT);
			panelButtonsRight.SetGravity(HGravity.Right);
			panelButtonsRight.SetHDimension(Dimension.Fill);
			panelButtons.AddElement(panelButtonsRight);

			Textbox uiButtonClose= new Textbox();
			uiButtonClose.SetText("Close");
			uiButtonClose.SetPadding(new UiRectangle(defaultPadding));
			uiButtonClose.SetProperties(CANFOCUS);
			uiButtonClose.RegisterOnLMBRelease(Close);
			WindowManager.RegisterOnMouseButtonCallback(uiButtonClose.OnMouseButton);
			WindowManager.RegisterOnMouseMoveCallback(uiButtonClose.OnMouseMove);
			panelButtonsLeft.AddElement(uiButtonClose);

			Textbox uiButtonRevert = new Textbox();
			uiButtonRevert.SetText("Revert");
			uiButtonRevert.SetPadding(new UiRectangle(defaultPadding));
			uiButtonRevert.SetProperties(CANFOCUS);
			uiButtonRevert.RegisterOnLMBRelease(Revert);
			WindowManager.RegisterOnMouseButtonCallback(uiButtonRevert.OnMouseButton);
			WindowManager.RegisterOnMouseMoveCallback(uiButtonRevert.OnMouseMove);
			panelButtonsRight.AddElement(uiButtonRevert);

			Refresh();
		}

		private void Revert()
		{
			panelModel.Revert();
			Close();
		}
	}
}
