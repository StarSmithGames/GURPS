using Game.Systems.QuestSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class QuestTriggerInRange : MonoBehaviour
{
	[SerializeField] private Quest quest;
	[SerializeField] private Settings settings;

	private Data data;

	private QuestManager questManager;

	[Inject]
	private void Construct(QuestManager questManager)
	{
		this.questManager = questManager;
	}

	private void Start()
	{
		if(data == null)
		{
			data = new Data();
		}

		if (data.isVisited)
		{
			gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (!data.isVisited)
		{
			var colliders = Physics.OverlapSphere(transform.position, settings.range, settings.characterMask);

			if(colliders.Length > 0)
			{
				data.isVisited = true;

				quest.Initialize();
				questManager.AddQuest(quest);

				gameObject.SetActive(false);
			}
		}
	}

	public Data GetData() => data;

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, settings.range);
	}

	[System.Serializable]
	public class Settings
	{
		public float range = 5f;
		public LayerMask characterMask;
	}

	public class Data
	{
		public bool isVisited;
	}
}