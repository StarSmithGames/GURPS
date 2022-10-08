using Game.Entities.Models;
using Game.Managers.PartyManager;

using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;

using Zenject;

namespace Game.Systems.SpawnManager
{
	[ExecuteInEditMode]
	public class SpawnPoint : MonoBehaviour
	{
		public Settings settings;

		public bool IsSpawnProcess => spawnCoroutine != null;
		private Coroutine spawnCoroutine = null;

		private DiContainer container;
		private SpawnManager spawnManager;
		private PartyManager partyManager;

	   [Inject]
		private void Construct(DiContainer container, SpawnManager spawnManager, PartyManager partyManager)
		{
			this.container = container;
			this.spawnManager = spawnManager;
			this.partyManager = partyManager;
		}

		private void Start()
		{
			if (Application.isPlaying)
			{
				spawnManager.Registrate(this);
			}
		}

		private void OnDestroy()
		{
			if (Application.isPlaying)
			{
				spawnManager.UnRegistrate(this);
			}
		}

		public void Update()
		{
			if (!IsSpawnProcess)
			{
				Draw();
			}
		}

		public void Execute()
		{
			if (!IsSpawnProcess)
			{
				spawnCoroutine = StartCoroutine(Spawn());
			}
		}

		private IEnumerator Spawn()
		{
			var pos = transform.position;
			var rot = transform.rotation;

			if (settings.spawnType == SpawnType.Party)
			{
				partyManager.PlayerParty.Characters.ForEach((x) =>
				{
					container.InstantiatePrefab(x.CharacterData.prefab, pos, rot, null);
				});
			}
			else if (settings.spawnType == SpawnType.Companion)
			{
				container.InstantiatePrefab(settings.model, pos, rot, null);
			}

			gameObject.SetActive(false);

			yield return null;

			spawnCoroutine = null;
		}

		private void Draw()
		{
			bool isValid = GlobalDatabase.Instance.HumanoidMesh && GlobalDatabase.Instance.Stand && GlobalDatabase.Instance.Material;

			if (isValid)
			{
				var material = new Material(GlobalDatabase.Instance.Material);

				if (settings.spawnType == SpawnType.Party)
				{
					var color = Color.green;
					color.a = 0.5f;
					material.SetColor("_BaseColor", color);

					DrawCharacter(material, new Vector3(0, 0, 0.5f));
					DrawCharacter(material, new Vector3(0.5f, 0, 0));
					DrawCharacter(material, new Vector3(-0.5f, 0, 0));
				}
				else if(settings.spawnType == SpawnType.Companion)
				{
					var color = Color.blue;
					color.a = 0.5f;
					material.SetColor("_BaseColor", color);

					DrawCharacter(material, Vector3.zero);
				}
			}

			void DrawCharacter(Material material, Vector3 offset)
			{
				Graphics.DrawMesh(GlobalDatabase.Instance.Stand, transform.position + offset, transform.rotation * Quaternion.Euler(-90f, 0, 0), material, gameObject.layer);
				Graphics.DrawMesh(GlobalDatabase.Instance.HumanoidMesh, transform.position + new Vector3(0, 0.05f, 0) + offset, transform.rotation * Quaternion.Euler(-90f, 0, 0), material, gameObject.layer);
			}
		}

		[System.Serializable]
		public class Settings
		{
			public SpawnType spawnType;

			[ShowIf("spawnType", SpawnType.Companion)]
			public CharacterModel model;
		}
	}

	public enum SpawnType
	{
		Party,
		Companion,
		Prefab,
	}
}