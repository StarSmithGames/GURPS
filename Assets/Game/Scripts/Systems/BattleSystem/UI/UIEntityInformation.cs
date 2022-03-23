using UnityEngine;

namespace Game.Systems.BattleSystem
{
    public class UIEntityInformation : MonoBehaviour
    {
        [field: SerializeField] public TMPro.TextMeshProUGUI Name { get; private set; }
        [field: SerializeField] public UIBar HealthBar { get; private set; }
        [field: SerializeField] public TMPro.TextMeshProUGUI Level { get; private set; }

        public void SetEntity(IEntity entity)
		{
            Name.text = entity?.EntityData.CharacterName ?? "";
            Level.text = "Level 999";
        }
    }
}