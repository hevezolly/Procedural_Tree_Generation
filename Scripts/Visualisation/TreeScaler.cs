using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeScaler : MonoBehaviour, ITreePartsProvider
{
    [SerializeField]
    private GameObject partsProvider;
    [SerializeField]
    private float margin;
    [SerializeField]
    private bool accountLeafs;

    private ITreePartsProvider provider;

    private IEnumerable<Branch> branches;
    private IEnumerable<Leaf> leafs;

    private Vector3 max;
    private Vector3 min;
    private float scale;
    public float ScaleFactor => scale;

    private void Awake()
    {
        provider = partsProvider.GetComponent<ITreePartsProvider>();
    }

    public void Scale()
    {
        var branchUnnormalised = provider.GetBranches().ToList();
        var leafsUnnormalised = provider.GetLeafs().ToList();
        max = Vector3.negativeInfinity;
        min = Vector3.positiveInfinity;
        branchUnnormalised.ForEach(b =>
        {
            max.x = Mathf.Max(b.start.x + margin, b.end.x + margin, max.x);
            max.y = Mathf.Max(b.start.y + margin, b.end.y + margin, max.y);
            max.z = Mathf.Max(b.start.z + margin, b.end.z + margin, max.z);
            min.x = Mathf.Min(b.start.x - margin, b.end.x - margin, min.x);
            min.y = Mathf.Min(b.start.y - margin, b.end.y - margin, min.y);
            min.z = Mathf.Min(b.start.z - margin, b.end.z - margin, min.z);
        });
        if (accountLeafs)
        {
            leafsUnnormalised.ForEach(b =>
            {
                max.x = Mathf.Max(b.position.x + b.radius + margin, max.x);
                max.y = Mathf.Max(b.position.y + b.radius + margin, max.y);
                max.z = Mathf.Max(b.position.z + b.radius + margin, max.z);
                min.x = Mathf.Min(b.position.x - b.radius - margin, min.x);
                min.y = Mathf.Min(b.position.y - b.radius - margin, min.y);
                min.z = Mathf.Min(b.position.z - b.radius - margin, min.z);
            });
        }
        scale = 1 / Mathf.Max(max.x - min.x, max.y - min.y, max.z - min.z);
        var one = new Vector3(1, margin * scale * 2, 1);
        var center = (max + min) * scale / 2 + new Vector3(1, margin * scale * 2, 1) / 2;
        var offset = Vector3.one / 2 - center;
        branches = branchUnnormalised.Select(b => new Branch(
            (b.start * scale + one / 2 + offset), (b.end * scale + one / 2 + offset), b.color, b.startWidth * scale, b.endWidth * scale)
        );
        leafs = leafsUnnormalised.Select(b => new Leaf(
            (b.position * scale + one / 2 + offset), b.radius * scale, b.color, b.power)
        );
    }

    public Branch[] GetBranches()
    {
        return branches.ToArray();
    }

    public Leaf[] GetLeafs()
    {
        return leafs.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        var center = (max + min) * scale / 2;
        var size = (max - min) * scale;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.one / 2, size);
    }
}
