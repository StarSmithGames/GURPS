using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.UI.Windows;

using UnityEngine;

namespace Game
{
	[System.Serializable]
	public class GlobalSettings
	{
		public CameraVision.Settings cameraVisionMap;
		public CameraVision.Settings cameraVisionLocation;
		[Space]
		public BattleSystem.Settings battleSettings;

		[Space]
		public WindowInfinityLoading.Settings infinityLoadingSettings;
	}
}