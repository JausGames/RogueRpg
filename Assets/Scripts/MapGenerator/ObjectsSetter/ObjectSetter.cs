using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSetter : MonoBehaviour
{
    [SerializeField] protected SkinnedMeshRenderer renderer;
    [SerializeField] protected List<int> ZeroToOneBlendShapes;
    protected float mapSizeFactor = 12f;

    public void SetUpObject()
    {
        if (ZeroToOneBlendShapes.Count == 0) SetComponents();
        SetOffset();
        foreach (int index in ZeroToOneBlendShapes)
        {
            SetKeyFrameValue(index, Random.Range(0, 100));
        }
    }
    virtual protected void SetOffset()
    {
        transform.position = transform.position + Vector3.up * -0.2f
                                + Vector3.right * Random.Range(-0.33f, 0.33f) * mapSizeFactor
                                + Vector3.forward * Random.Range(-0.33f, 0.33f) * mapSizeFactor;

        transform.rotation = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);
    }
    private void SetComponents()
    {
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
        {
            ZeroToOneBlendShapes.Add(i);
        }
    }

    protected void SetKeyFrameValue(int keyIndex, float value)
    {
        renderer.SetBlendShapeWeight(keyIndex, value);
    }
}
