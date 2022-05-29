using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class BranchesGenerator : SinglePartVolumeGenerator<Branch>
{
    [SerializeField]
    private GameObject branchesProviderObj;

    private IBranchProvider branchProvider;

    protected override void Awake()
    {
        base.Awake();
        branchProvider = branchesProviderObj.GetComponent<IBranchProvider>();
    }

    protected override Branch[] GetParts()
    {
        return branchProvider.GetBranches();
    }

    protected override ComputeBuffer GetPartsBuffer(Branch[] parts, ComputeBufferType type)
    {
        var b = new ComputeBuffer(parts.Length, Branch.Size, type);
        b.SetData(parts);
        return b;
    }

    
}
