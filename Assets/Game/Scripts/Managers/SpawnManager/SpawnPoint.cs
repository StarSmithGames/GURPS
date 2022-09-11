using Game.Managers.PartyManager;

using System.Collections;

using UnityEngine;

using Zenject;

namespace Game.Systems.SpawnManager
{
	[ExecuteInEditMode]
	public class SpawnPoint : MonoBehaviour
	{
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

			partyManager.PlayerParty.Characters.ForEach((x) =>
			{
				container.InstantiatePrefab(x.CharacterData.prefab, pos, rot, null);
			});

			GameObject.DestroyImmediate(gameObject);

			yield return null;

			spawnManager.UnRegistrate(this);
			spawnCoroutine = null;
		}

		private void Draw()
		{
			if (GlobalDatabase.Instance.HumanoidMesh && GlobalDatabase.Instance.GreenMaterial)
			{
				Graphics.DrawMesh(GlobalDatabase.Instance.HumanoidMesh, transform.position, transform.rotation * Quaternion.Euler(-90f, 0, 0), GlobalDatabase.Instance.GreenMaterial, gameObject.layer);
			}
		}
	}
}