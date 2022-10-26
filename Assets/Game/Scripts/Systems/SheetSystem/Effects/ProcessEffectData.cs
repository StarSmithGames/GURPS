using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Systems.SheetSystem
{
    [CreateAssetMenu(fileName = "ProcessEffect", menuName = "Game/Sheet/Effects/Process Effect")]
    public class ProcessEffectData : EffectData
    {
        [OnValueChanged("OnCustom")]
        public bool isCustom = false;
        [Min(1f)]
        [HideIf("isCustom")]
        [OnValueChanged("OnCurve")]
        [SuffixLabel("sec", true)]
        public float duration = 1f;
        [Range(2, 10)]
        [HideIf("isCustom")]
        [OnValueChanged("OnCurve")]
        public int amount = 2;
        
        public AnimationCurve curve = AnimationCurve.Linear(0, 0.5f, 1, 0.5f);

        [Space]
        public bool isBlinkOnEnd = true;

        private void OnCurve()
		{
            Keyframe[] keyframes = new Keyframe[amount];

			for (int i = 0; i < keyframes.Length; i++)
			{
                keyframes[i].value = 0.5f;

                if (i == keyframes.Length - 1)
				{
                    keyframes[i].time = duration;
                }
				else
				{
                    keyframes[i].time = (float)i * (duration / (amount - 1));
                }
            }

            curve = new AnimationCurve(keyframes);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void OnCustom()
		{
			if (isCustom)
			{
                curve = AnimationCurve.Linear(0, 0.5f, 1, 0.5f);
			}
			else
			{
                OnCurve();
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}