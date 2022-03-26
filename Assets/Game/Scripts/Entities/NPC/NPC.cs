using Zenject;

namespace Game.Entities
{
	public class NPC : Entity
	{
		protected UIManager uiManager;
		protected FieldOfView fov;

		[Inject]
		private void Construct(UIManager uiManager, FieldOfView fov)
		{
			this.uiManager = uiManager;
			this.fov = fov;
		}

		public override void StartObserve()
		{
			base.StartObserve();
			uiManager.Battle.SetEntityInformation(this);
		}

		public override void EndObserve()
		{
			base.EndObserve();
			uiManager.Battle.SetEntityInformation(null);
		}
	}
}