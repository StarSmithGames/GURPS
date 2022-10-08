namespace Game.Systems.SpawnManager
{
	public class SpawnManager : Registrator<SpawnPoint>
	{
		public bool IsSpawnProcess
		{
			get
			{
				for (int i = 0; i < registers.Count; i++)
				{
					if (registers[i].IsSpawnProcess)
					{
						return true;
					}
				}

				return false;
			}
		}

		public void Spawn()
		{
			for (int i = 0; i < registers.Count; i++)
			{
				registers[i].Execute();
			}

			for (int i = registers.Count - 1 ; i >= 0 ; i--)
			{
				UnRegistrate(registers[i]);
			}
		}
	}
}