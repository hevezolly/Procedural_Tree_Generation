using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SinglePartGenerator : MonoBehaviour
{
    public abstract IEnumerator StartGenerating();

    public abstract void GeneratePartsMesh();

    public abstract void ClearMesh();
}
