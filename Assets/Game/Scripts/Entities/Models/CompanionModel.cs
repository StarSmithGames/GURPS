using Game.Managers.CharacterManager;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Entities.Models
{
    public interface ICompanionModel : ICharacterModel
	{
		
	}

    public class CompanionModel : CharacterModel, ICompanionModel
    {
		public CharacterData data;

		[Inject]
		private void Construct(CharacterManager characterManager)
		{
			if(characterManager.Contains(data, out ICharacter character))
			{
				Character = character;
			}
			else
			{
				Character = characterManager.CreateCharacter(data);
			}

			Character.SetModel(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			Character.SetModel(null);
		}
	}
}