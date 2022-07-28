using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSetter : ObjectSetter
{
    private void Awake()
    {
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        for(int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
        {
            ZeroToOneBlendShapes.Add(i);
        }
    }

    protected override void SetOffset()
    {
        base.SetOffset();
        transform.rotation = Random.rotation;
    }
}
