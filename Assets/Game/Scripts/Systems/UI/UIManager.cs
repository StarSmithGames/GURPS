using CMF;

using Game.Managers.CharacterManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class UIManager : MonoBehaviour
{
	public UIVirtualSpace CurrentVirtualSpace { get; private set; }
	public UIWindowsManager WindowsManager { get; private set; }

	[SerializeField] private UIVirtualSpace originalVirtualSpace;
	[SerializeField] private List<UIAvatar> avatars = new List<UIAvatar>();

	private List<UIVirtualSpace> virtualSpaces = new List<UIVirtualSpace>();

	private SignalBus signalBus;
	private CharacterManager characterManager;

	[Inject]
	private void Construct(SignalBus signalBus, UIWindowsManager windowsManager, CharacterManager characterManager)
	{
		this.signalBus = signalBus;
		this.characterManager = characterManager;

		WindowsManager = windowsManager;

		avatars.ForEach((x) => x.onClicked += OnAvatarClicked);

		signalBus?.Subscribe<SignalCharacterChanged>(OnCharacterChanged);
	}

	private void OnDestroy()
	{
		avatars.ForEach((x) => x.onClicked -= OnAvatarClicked);

		signalBus?.Unsubscribe<SignalCharacterChanged>(OnCharacterChanged);
	}

	private void Awake()
	{
		for (int i = 0; i < 2; i++)
		{
			var space = Instantiate(originalVirtualSpace, originalVirtualSpace.transform.parent);
			virtualSpaces.Add(space);
			space.gameObject.SetActive(false);
		}

		originalVirtualSpace.gameObject.SetActive(false);
	}

	private void SetVirtualSpace(int index)
	{
		virtualSpaces.ForEach((x) => x.gameObject.SetActive(false));

		virtualSpaces[index].gameObject.SetActive(true);
		CurrentVirtualSpace = virtualSpaces[index];
	}

	private void SetAvatarFrame(int index)
	{
		avatars.ForEach((x) => x.SetFrame(false));
		avatars[index].SetFrame(true);
	}


	private void OnAvatarClicked(UIAvatar avatar)
	{
		int index = avatars.IndexOf(avatar);
		characterManager.SetCharacter(index);
	}

	private void OnCharacterChanged(SignalCharacterChanged signal)
	{
		int index = characterManager.CurrentCharacterIndex;

		SetVirtualSpace(index);
		SetAvatarFrame(index);
	}
}