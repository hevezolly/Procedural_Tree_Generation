using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class LeafsGenerator : SinglePartVolumeGenerator<Leaf>
{

    [SerializeField]
    private GameObject leafsProviderObject;

    private ILeafProvider leafProvider;

    protected override void Awake()
    {
        base.Awake();
        leafProvider = leafsProviderObject.GetComponent<ILeafProvider>();
    }

    protected override Leaf[] GetParts()
    {
        return leafProvider.GetLeafs();
    }

    protected override ComputeBuffer GetPartsBuffer(Leaf[] parts, ComputeBufferType type = ComputeBufferType.Default)
    {
        var b = new ComputeBuffer(parts.Length, Leaf.Size, type);
        b.SetData(parts);
        return b;
    }
}
