using System.Collections.Generic;

using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
    public interface IModifiable<M> where M : Modifier<float>
    {
        event UnityAction onModifiersChanged;

        float TotalValue { get; }
        float ModifyAddValue { get; }
        float ModifyPercentValue { get; }

        List<M> Modifiers { get; }

        void AddModifier(M modifier);
        void RemoveModifier(M modifier);

        bool Contains(M modifier);
    }
}