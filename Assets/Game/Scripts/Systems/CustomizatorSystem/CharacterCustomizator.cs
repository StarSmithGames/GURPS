using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;
using UnityEngine.XR;

namespace Game.Systems.CutomizationSystem
{
    public class CharacterCustomizator : MonoBehaviour
    {
        [HideLabel]
        [SerializeField] private Character character;
    }

    [System.Serializable]
    public class Character
    {
        [HideLabel]
        [BoxGroup("Head")]
        public Head head;

        [HideLabel]
        [BoxGroup("Body")]
        public Body body;

        [Button]
        private void Refresh()
        {
            //body.Refresh();
        }

        [Button]
        private void Reset()
        {
            head.Reset();
            body.Reset();
        }
    }

    [System.Serializable]
    public class CharacterCollection
    {
        public bool IsEnabled
        {
            get => isEnable;
            set
            {
                isEnable = value;

                renderer.enabled = isEnable;

            }
        }
        private bool isEnable = true;


        [SerializeField] private SkinnedMeshRenderer renderer;
        [SerializeField] private List<Material> materials = new List<Material>();

        public CharacterCollection(SkinnedMeshRenderer renderer)
        {
            this.renderer = renderer;
        }

        public bool SetMaterial(int index)
        {
            if (renderer == null) return false;

            if (index >= 0 && index < materials.Count)
            {
                renderer.material = materials[index];

                return true;
            }

            return false;
        }
    }

    [System.Serializable]
    public class CharacterCollectionHandler
    {
        [OnValueChanged("RefreshColor")]
        public HairColor color = HairColor.Grey;
        [ReadOnly] public int index = -1;
        public List<CharacterCollection> collection = new List<CharacterCollection>();

        public void Reset()
        {
            index = -1;
            color = HairColor.Grey;

            Refresh();
            RefreshColor();
        }

        private void Refresh()
        {
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i].IsEnabled = false;
            }

            if (index != -1)
            {
                collection[index].IsEnabled = true;
            }
        }

        private void RefreshColor()
        {
            if (index != -1 && index < collection.Count)
            {
                Assert.IsTrue(collection[index].SetMaterial((int)color), $"CharacterCollection index {index} doesn't contain color {color} or Renderer == null");
            }
        }

        [ButtonGroup("Collection")]
        private void Prev()
        {
            if (collection.Count == 0)
            {
                index = -1;
                return;
            }
            index = index - 1 >= -1 ? index - 1 : index;

            RefreshColor();
            Refresh();
        }

        [ButtonGroup("Collection")]
        private void Next()
        {
            if (collection.Count == 0)
            {
                index = -1;
                return;
            }

            index = index + 1 < collection.Count ? index + 1 : index;

            RefreshColor();
            Refresh();
        }
    }



    [System.Serializable]
    public class Head
    {
        [OnValueChanged("RefreshEyesColor")]
        public EyeColor eyesColor = EyeColor.Blue;

        public CharacterCollectionHandler hairs;
        public CharacterCollectionHandler eyebrows;
        public CharacterCollection eyes;
        public CharacterCollectionHandler beard;

        [Button]
        public void Reset()
        {
            eyesColor = EyeColor.Blue;
            RefreshEyesColor();

            hairs.Reset();
            eyebrows.Reset();
            beard.Reset();
        }

        private void RefreshEyesColor()
        {
            Assert.IsTrue(eyes.SetMaterial((int)eyesColor), $"Head eyes doesn't contain color {eyesColor} or Renderer == null");
        }
    }


    [System.Serializable]
    public class Body
    {
        [OnValueChanged("RefreshBodyColor")]
        public BodyColor bodyColor = BodyColor.White;

        public CharacterCollection body;

        [Button]
        public void Reset()
        {
            bodyColor = BodyColor.White;
            RefreshBodyColor();
        }

        private void RefreshBodyColor()
        {
            Assert.IsTrue(body.SetMaterial((int)bodyColor), $"Body doesn't contain color {bodyColor} or Renderer == null");
        }
    }


    public enum BodyColor : int
    {
        White = 0,
        Black = 1,
        Yellow = 2,
    }

    public enum HairColor : int
    {
        Black = 0,
        Blond = 1,
        Brown = 2,
        Grey = 3,
    }

    public enum EyeColor : int
    {
        Blue = 0,
        Brown = 1,
        Green = 2,
        Purple = 3,
    }
}