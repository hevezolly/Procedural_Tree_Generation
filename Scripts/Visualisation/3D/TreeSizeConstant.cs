using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSizeConstant : MonoBehaviour
{
    [SerializeField]
    private TreeScaler scaler;
    [SerializeField]
    private Transform treeOrigin;

    private Vector3 initialScale;

    private Vector3 originPosition;

    private void Awake()
    {
        initialScale = transform.localScale;
        originPosition = treeOrigin.position;
    }

    public void Scale()
    {
        transform.localScale = initialScale / scaler.ScaleFactor;
        var normalCenter = scaler.GetBranches()[0].start;
        var normalOffset = normalCenter - Vector3.one / 2;
        transform.position = Vector3.Scale(originPosition - normalOffset / scaler.ScaleFactor, initialScale);
    }
}
