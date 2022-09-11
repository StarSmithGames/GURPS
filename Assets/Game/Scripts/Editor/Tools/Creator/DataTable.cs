using Game.Entities;
using Game.Entities.Models;
using Game.Systems.InventorySystem;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class DataTable<T, W>
        where T : ScriptableObject
        where W : DataWrapper<T>
	{
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        protected List<W> data = new List<W>();

        public T this[int index] => data[index].Data;
    }

	#region Wrappers
	public class DataWrapper<T> where T : ScriptableObject
    {
        public T Data { get; protected set; }

        public DataWrapper()
		{

		}
    }

    public class CharacterDataWrapper : DataWrapper<CharacterData>
    {
        public CharacterDataWrapper(CharacterData character)
        {
            Data = character;
        }

        [TableColumnWidth(50, false)]
        [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
        public Sprite Portrait { get => Data.information.portrait; set { Data.information.portrait = value; EditorUtility.SetDirty(Data); } }

        [TableColumnWidth(120)]
        [ShowInInspector]
        public string NameId { get => Data.information.nameId; set { Data.information.nameId = value; EditorUtility.SetDirty(Data); } }

        //[ShowInInspector, ProgressBar(0, 100)]
        //public float Shooting { get { return this.character.Skills.Shooting; } set { this.character.Skills.Shooting = value; EditorUtility.SetDirty(this.character); } }

        //[ShowInInspector, ProgressBar(0, 100)]
        //public float Melee { get { return this.character.Skills.Melee; } set { this.character.Skills.Melee = value; EditorUtility.SetDirty(this.character); } }

        //[ShowInInspector, ProgressBar(0, 100)]
        //public float Social { get { return this.character.Skills.Social; } set { this.character.Skills.Social = value; EditorUtility.SetDirty(this.character); } }

        //[ShowInInspector, ProgressBar(0, 100)]
        //public float Animals { get { return this.character.Skills.Animals; } set { this.character.Skills.Animals = value; EditorUtility.SetDirty(this.character); } }

        //[ShowInInspector, ProgressBar(0, 100)]
        //public float Medicine { get { return this.character.Skills.Medicine; } set { this.character.Skills.Medicine = value; EditorUtility.SetDirty(this.character); } }

        //[ShowInInspector, ProgressBar(0, 100)]
        //public float Crafting { get { return this.character.Skills.Crafting; } set { this.character.Skills.Crafting = value; EditorUtility.SetDirty(this.character); } }
    }

    public class ModelDataWrapper : DataWrapper<ModelData>
    {
        public ModelDataWrapper(ModelData model)
        {
            Data = model;
        }

        [TableColumnWidth(50, false)]
        [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
        public Sprite Portrait { get => Data.information.portrait; set { Data.information.portrait = value; EditorUtility.SetDirty(Data); } }

        [TableColumnWidth(120)]
        [ShowInInspector]
        public string NameId { get => Data.information.nameId; set { Data.information.nameId = value; EditorUtility.SetDirty(Data); } }
    }

	public class ContainerDataWrapper : DataWrapper<ContainerData>
    {
		public ContainerDataWrapper(ContainerData container)
        {
            Data = container;
        }

        [TableColumnWidth(50, false)]
        [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
        public Sprite Portrait { get => Data.information.portrait; set { Data.information.portrait = value; EditorUtility.SetDirty(Data); } }

        [TableColumnWidth(120)]
        [ShowInInspector]
        public string NameId { get => Data.information.nameId; set { Data.information.nameId = value; EditorUtility.SetDirty(Data); } }
    }

	#endregion

	public class CharacterDataTable : DataTable<CharacterData, CharacterDataWrapper>
	{
        public CharacterDataTable(IEnumerable<CharacterData> characters)
        {
            data = characters.Select((x) => new CharacterDataWrapper(x)).ToList();
        }
    }

    public class ModelDataTable : DataTable<ModelData, ModelDataWrapper> 
    {
        public ModelDataTable(IEnumerable<ModelData> models)
        {
            data = models.Select((x) => new ModelDataWrapper(x)).ToList();
        }
    }

    public class ContainerDataTable : DataTable<ContainerData, ContainerDataWrapper>
    {
        public ContainerDataTable(IEnumerable<ContainerData> models)
        {
            data = models.Select((x) => new ContainerDataWrapper(x)).ToList();
        }
    }
}