using Game.Managers.SceneManager;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIButtonSwitchScene : MonoBehaviour
{
	[field: SerializeField] public Scenes GoTo { get; private set; }
	[field: SerializeField] public Button Button { get; private set; }

	private SceneManager sceneManager;

	[Inject]
	private void Construct(SceneManager sceneManager)
	{
		this.sceneManager = sceneManager;
	}

	private void Start()
	{
		Button.onClick.AddListener(Click);
	}

	private void OnDestroy()
	{
		Button.onClick.RemoveAllListeners();
	}

	private void Click()
	{
		sceneManager.SwitchScene(GoTo);
	}
}
