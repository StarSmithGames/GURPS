using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModel : MonoBehaviour
{
    [ReadOnly] public int materialIndex = 0;

    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private List<Material> materials = new List<Material>();

    private void Refresh()
    {
        renderer.material = materials[materialIndex];
    }

    [ButtonGroup("Body")]
    private void PrevBody()
    {
        if (renderer == null) renderer = GetComponent<MeshRenderer>();

        if (materials.Count == 0) return;

        materialIndex = materialIndex - 1 >= 0 ? materialIndex - 1 : materialIndex;

        Refresh();
    }
    [ButtonGroup("Body")]
    private void NextBody()
    {
        if (renderer == null) renderer = GetComponent<MeshRenderer>();

        if (materials.Count == 0) return;

        materialIndex = materialIndex + 1 < materials.Count ? materialIndex + 1 : materialIndex;

        Refresh();
    }
}