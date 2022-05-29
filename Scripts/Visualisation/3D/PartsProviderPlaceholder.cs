using LSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsProviderPlaceholder : MonoBehaviour, ITreePartsProvider, IStepReciver
{
    [SerializeField]
    private GameObject linearProviderObject;
    [SerializeField]
    private GameObject derrivationProviderObject;
    [SerializeField]
    private LSystemTest systemActivator;

    private ITreePartsProvider mainProvider;
    private IStepReciver mainStepper;

    private void Awake()
    {
        switch (systemActivator.visualiseType) 
        {
            case LSystemVisualiseType.Linear:
                mainProvider = linearProviderObject.GetComponent<ITreePartsProvider>();
                mainStepper = linearProviderObject.GetComponent<IStepReciver>();
                break;
            case LSystemVisualiseType.DerrivationTree:
                mainProvider = derrivationProviderObject.GetComponent<ITreePartsProvider>();
                mainStepper = derrivationProviderObject.GetComponent<IStepReciver>();
                break;
            default:
                throw new System.ArgumentException("incorrect l-system visualisation type");
        }
    }

    public Branch[] GetBranches()
    {
        return mainProvider.GetBranches();
    }

    public Leaf[] GetLeafs()
    {
        return mainProvider.GetLeafs();
    }

    public void Step(List<IModule> state)
    {
        mainStepper.Step(state);
    }
}
