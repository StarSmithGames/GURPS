using UnityEngine;

using Zenject;

namespace Game.Systems.CommandCenter
{
	public class CommandCenter : MonoBehaviour
	{
		public static CommandCenter Instance
		{
			get
			{
				if(instance == null)
				{
					instance = FindObjectOfType<CommandCenter>();
				}

				return instance;
			}
		}
		private static CommandCenter instance = null;

		public Registrator<IExecutor> Registrator
		{
			get
			{
				if(registrator == null)
				{
					registrator = new Registrator<IExecutor>();
				}

				return registrator;
			}
		}
		private Registrator<IExecutor> registrator;

		private DiContainer container;

		[Inject]
		private void Construct(DiContainer container)
		{
			this.container = container;
		}
	}
}