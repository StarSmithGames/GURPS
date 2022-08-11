using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.UI.Windows;

using UnityEngine;

[System.Serializable]
public class GlobalSettings
{
	public CameraVisionMap.Settings cameraVisionMap;
	public CameraVisionLocation.Settings cameraVisionLocation;
	[Space]
	public BattleSystem.Settings battleSettings;

	[Space]
	public WindowInfinityLoading.Settings infinityLoadingSettings;
}