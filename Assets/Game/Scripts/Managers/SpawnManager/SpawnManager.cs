namespace Game.Systems.SpawnManager
{
	public class SpawnManager : Registrator<SpawnPoint>
	{
		public bool IsSpawnProcess => registers.Count > 0;

		public void Spawn()
		{
			for (int i = 0; i < registers.Count; i++)
			{
				registers[i].Execute();
			}
		}
	}
}