using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStructureVisualiser : MonoBehaviour
{
    [SerializeField]
    private GameObject providerObject;

    private ITreePartsProvider provider;

    [SerializeField]
    private Vector3 originPosition;
    [SerializeField]
    private Vector3 startDirection;
    [SerializeField]
    private float length;
    [SerializeField]
    private bool showBranches = true;
    [SerializeField]
    private bool showLeafs = true;

    private Branch[] branches;
    private Leaf[] leafs;

    private void Awake()
    {
        provider = providerObject.GetComponent<ITreePartsProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        branches = provider.GetBranches();
        leafs = provider.GetLeafs();
    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(originPosition + Vector3.one / 2, Vector3.one);
        Gizmos.color = Color.red;
        if (!Application.isPlaying)
        {
            Gizmos.DrawLine(originPosition, originPosition + startDirection.normalized * length);
        }
        else
        {
            if (showBranches && branches != null)
            {
                foreach (var startEnd in branches)
                {
                    Gizmos.color = startEnd.color;
                    Gizmos.DrawLine(originPosition + startEnd.start,originPosition + startEnd.end);
                }
            }
            if (showLeafs && leafs != null)
            {
                foreach (var leaf in leafs)
                {
                    Gizmos.color = leaf.color;
                    Gizmos.DrawWireSphere(originPosition + leaf.position, leaf.radius);
                }
            }
        }
    }
}
