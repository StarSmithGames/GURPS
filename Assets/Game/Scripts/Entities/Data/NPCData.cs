using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
    [CreateAssetMenu(fileName = "NPC", menuName = "Game/Characters/NPC")]
    public class NPCData : EntityData
    {
        public NPCInformation information;
    }
}