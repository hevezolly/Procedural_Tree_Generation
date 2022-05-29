using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGeneratorConfiguration : MonoBehaviour
{
    [SerializeField]
    private LSystemProvider lSystem;

    [SerializeField]
    private GameObject treePartsProvider;

    public LSystemProvider LSystem => lSystem;

    public ITreePartsProvider TreePArtsProvider => treePartsProvider.GetComponent<ITreePartsProvider>();
}
