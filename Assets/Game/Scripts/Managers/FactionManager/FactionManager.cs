using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Managers.FactionManager
{
    public class FactionManager
    {
        public readonly static string[] Factions = new string[]
        {
            "Player",
            "Dummy",
            "Beast",
            "Undead"
        };

        private List<Relation> relations;

        public FactionManager(List<Relation> relations)
		{
            this.relations = relations;
        }

        public bool CheckRelationShip(IFactionable factionA, RelationType type, IFactionable factionB)
		{
            var relation = relations.Find((x) => IsContains(x, factionA) && IsContains(x, factionB));

            return relation?.relation == type;
        }

        public List<Relation> FindRelations(IFactionable factionable)
        {
            return relations.FindAll((x) => (x.factionA.faction == factionable.Faction.faction) != (x.factionB.faction == factionable.Faction.faction));//!= => xor
        }

        private bool IsContains(Relation relation, IFactionable factionable)
		{
            return (relation.factionA.faction == factionable.Faction.faction) || (relation.factionB.faction == factionable.Faction.faction);
        }
    }
}