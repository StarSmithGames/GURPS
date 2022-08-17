using Game.Managers.CharacterManager;
using Game.Systems.BattleSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;
using Game.Systems.TooltipSystem;
using Game.Systems.ContextMenu;

using System.Collections.Generic;
using UnityEngine;

using Zenject;
using Game.Systems.NotificationSystem;
using Game.Managers.PartyManager;

public class UIManager : MonoBehaviour
{
	public UIVirtualSpace CurrentVirtualSpace { get; private set; }

	[field: SerializeField] public WindowCharacterSheet CharacterSheet { get; private set; }
	[field: SerializeField] public UIBattle Battle { get; private set; }
	[SerializeField] private UIVirtualSpace originalVirtualSpace;

	private List<UIVirtualSpace> virtualSpaces = new List<UIVirtualSpace>();

	private void OnDestroy()
	{
	}

	private void Start()
	{
		CreateVirtualSpaces();

		//SetVirtualSpace(characterManager.CurrentParty.LeaderPartyIndex);
		//CharacterSheet.SetSheet(characterManager.CurrentParty.LeaderParty.Sheet as CharacterSheet);
	}

	public void SetVirtualSpace(int index)
	{
		virtualSpaces.ForEach((x) => x.gameObject.SetActive(false));

		virtualSpaces[index].gameObject.SetActive(true);
		CurrentVirtualSpace = virtualSpaces[index];
	}

	private void CreateVirtualSpaces()
	{
		//for (int i = 0; i < characterManager.CurrentParty.Characters.Count; i++)
		//{
		//	var space = Instantiate(originalVirtualSpace, originalVirtualSpace.transform.parent);
		//	virtualSpaces.Add(space);
		//	space.gameObject.SetActive(false);
		//}

		originalVirtualSpace.gameObject.SetActive(false);
	}

	private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
	{
		//int index = characterManager.CurrentParty.LeaderPartyIndex;

		//SetVirtualSpace(index);
		//CharacterSheet.SetSheet(signal.leader.Sheet as CharacterSheet);
	}
}