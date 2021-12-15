using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Gaston11276.SimpleUi;
using Gaston11276.Fivemui;

namespace Gaston11276.Modelmenu.Client
{
	class ModelData
	{
		public PedHash pedHash;
		public ModelData(PedHash pedHash)
		{
			this.pedHash = pedHash;
		}
	}

	public class PanelModel : UiElementFiveM
	{
		EntryModel entryModel = new EntryModel();

		UiElementFiveM uiColumnLabels = new UiElementFiveM();
		UiElementFiveM uiColumnNames = new UiElementFiveM();
		UiElementFiveM uiColumnIndex = new UiElementFiveM();
		UiElementFiveM uiColumnDecrease = new UiElementFiveM();
		UiElementFiveM uiColumnIncrease = new UiElementFiveM();

		private int originalModelIndex = 0;
		private int currentModelIndex = 0;
		private List<ModelData> modelData = new List<ModelData>();

		float defaultPadding = 0.0025f;

		List<fpPedHash> onModelChangeCallbacks = new List<fpPedHash>();

		public PanelModel()
		{
			CreateModelData(modelData);
		}

		static void CreateModelData(List<ModelData> ModelData)
		{
			foreach (PedHash thisEnum in PedHash.GetValues(typeof(PedHash)))
			{
				ModelData.Add(new ModelData(thisEnum));
			}
		}

		public async Task SetUi()
		{
			await entryModel.SetUi();
		}

		public void SetCurrent(int index)
		{
			currentModelIndex = index;
			originalModelIndex = currentModelIndex;
		}

		protected void CreateColumn(UiElementFiveM uiPanel, HGravity gravity, UiElementFiveM uiColumn, string label = null)
		{
			uiColumn.SetOrientation(Orientation.Vertical);
			uiColumn.SetPadding(new UiRectangle(defaultPadding));
			uiColumn.SetGravity(gravity);
			uiColumn.SetFlags(UiElement.TRANSPARENT);
			uiPanel.AddElement(uiColumn);

			if (label != null)
			{
				Textbox header = new Textbox();
				header.SetPadding(new UiRectangle(defaultPadding));
				header.SetText(label);
				header.SetFlags(UiElement.TRANSPARENT);
				if (label.Length == 0)
				{
					header.SetTextFlags(UiElement.TRANSPARENT);
				}
				uiColumn.AddElement(header);
			}
		}

		protected void CreateColumns()
		{
			UiElementFiveM uiCenterPanel = new UiElementFiveM();
			uiCenterPanel.SetPadding(new UiRectangle(0f));
			AddElement(uiCenterPanel);

			CreateColumn(uiCenterPanel, HGravity.Left, uiColumnLabels);
			CreateColumn(uiCenterPanel, HGravity.Left, uiColumnNames);
			CreateColumn(uiCenterPanel, HGravity.Right, uiColumnIndex);
			CreateColumn(uiCenterPanel, HGravity.Center, uiColumnDecrease);
			CreateColumn(uiCenterPanel, HGravity.Center, uiColumnIncrease);
		}

		public void CreateUi()
		{
			SetPadding(new UiRectangle(0f));
			SetFlags(TRANSPARENT);

			CreateColumns();
			entryModel = CreateEntry("Model");
		}

		private EntryModel CreateEntry(string label)
		{
			float defaultPadding = 0.0025f;

			EntryModel entry = new EntryModel();

			entry.uiLabel.SetPadding(new UiRectangle(defaultPadding));
			entry.uiLabel.SetText(label);
			entry.uiLabel.SetFont(Font.CharletComprimeColonge);
			entry.uiLabel.SetFlags(UiElement.TRANSPARENT);
			uiColumnLabels.AddElement(entry.uiLabel);

			entry.uiName.SetPadding(new UiRectangle(defaultPadding));
			entry.uiName.SetText(label);
			entry.uiName.SetFont(Font.CharletComprimeColonge);
			entry.uiName.SetFlags(UiElement.TRANSPARENT);
			uiColumnNames.AddElement(entry.uiName);

			entry.uiIndex.SetText(label);
			entry.uiIndex.SetFont(Font.CharletComprimeColonge);
			entry.uiIndex.SetPadding(new UiRectangle(defaultPadding));
			entry.uiIndex.SetProperties(UiElement.CANFOCUS);
			entry.uiIndex.SetFlags(UiElement.TRANSPARENT);
			uiColumnIndex.AddElement(entry.uiIndex);

			entry.btnDecrease.SetText("-");
			entry.btnDecrease.SetPadding(new UiRectangle(defaultPadding));
			entry.btnDecrease.SetProperties(UiElement.CANFOCUS);
			entry.btnDecrease.RegisterOnLMBRelease(entry.Decrease);
			WindowManager.RegisterOnMouseButtonCallback(entry.btnDecrease.OnMouseButton);
			WindowManager.RegisterOnMouseMoveCallback(entry.btnDecrease.OnMouseMove);
			uiColumnDecrease.AddElement(entry.btnDecrease);

			entry.btnIncrease.SetText("+");
			entry.btnIncrease.SetPadding(new UiRectangle(defaultPadding));
			entry.btnIncrease.SetProperties(UiElement.CANFOCUS);
			entry.btnIncrease.RegisterOnLMBRelease(entry.Increase);
			WindowManager.RegisterOnMouseButtonCallback(entry.btnIncrease.OnMouseButton);
			WindowManager.RegisterOnMouseMoveCallback(entry.btnIncrease.OnMouseMove);
			uiColumnIncrease.AddElement(entry.btnIncrease);

			entry.GetIndexMax = GetModelIndexMax;
			entry.GetIndex = GetModelIndex;
			entry.SetIndex = SetModelIndex;
			entry.GetName = GetModelName;

			return entry;
		}

		private int GetModelIndexMax()
		{
			return modelData.Count;
		}

		private int GetModelIndex()
		{
			return currentModelIndex;
		}

		public PedHash GetModelHash()
		{
			return modelData[currentModelIndex].pedHash;
		}

		private string GetModelName()
		{
			return modelData[currentModelIndex].pedHash.ToString();
		}

		public int FindModelIndex(PedHash pedHash)
		{
			int cnt = 0;
			foreach (ModelData modelData in modelData)
			{
				if (modelData.pedHash == pedHash)
				{
					return cnt;
				}
				cnt++;
			}
			return -1;
		}

		public async void SetModelIndex(int index)
		{
			currentModelIndex = index;
			if (index >= 0 && index < modelData.Count)
			{
				await ApplyToPed();
			}

			foreach (fpPedHash OnModelChange in onModelChangeCallbacks)
			{
				OnModelChange(modelData[currentModelIndex].pedHash);
			}
		}

		public async Task ApplyToPed()
		{
			await ApplyModel();
		}

		public void Revert()
		{
			SetModelIndex(originalModelIndex);
		}

		public async Task ApplyModel()
		{

			API.RequestModel((uint)modelData[currentModelIndex].pedHash);

			Vehicle current_vehicle = Game.PlayerPed.CurrentVehicle;
			bool in_vehicle = false;
			if (Game.PlayerPed.IsInVehicle())
			{
				in_vehicle = true;
			}

			await Game.Player.ChangeModel(new Model(modelData[currentModelIndex].pedHash));
			API.SetPedDefaultComponentVariation(Game.PlayerPed.Handle);

			if (in_vehicle)
			{
				Game.PlayerPed.SetIntoVehicle(current_vehicle, VehicleSeat.Driver);
			}
		}

		public void RegisterOnModelChangeCallback(fpPedHash OnModelChange)
		{
			onModelChangeCallbacks.Add(OnModelChange);
		}
	}
}
