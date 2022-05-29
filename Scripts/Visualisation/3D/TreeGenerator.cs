using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField]
    private SinglePartGenerator branchGen;
    [SerializeField]
    private SinglePartGenerator leafGen;

    private Coroutine generation;

    [SerializeField]
    private bool instantGeneration;

    public void Generate()
    {
        if (generation != null) 
        {
            StopCoroutine(generation);
        }
        generation = StartCoroutine(StartGenerating());
    }

    private IEnumerator StartGenerating()
    {
        yield return branchGen.StartGenerating();
        if (!instantGeneration)
            yield return new WaitForEndOfFrame();
        yield return leafGen.StartGenerating();
        generation = null;
    }
}
