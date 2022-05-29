using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using LSystem;

public class DerivationTreeVisualiser : MonoBehaviour, ITreePartsProvider, IStepReciver
{
    private IModule axiome;

    [SerializeField]
    private string Nopstring;
    [SerializeField]
    private string Directionstring;
    [SerializeField]
    private string Lengthstring;
    [SerializeField]
    private string StartWidth;
    [SerializeField]
    private string EndWidth;
    [SerializeField]
    private string ColorSymb;
    [SerializeField]
    private string leafPowerSymb;
    [SerializeField]
    private string IsLeafstring;
    [SerializeField]
    private bool clampBranchWidth;

    [SerializeField]
    private float length;

    private List<Branch> branches = new List<Branch>();
    private List<Leaf> leafs = new List<Leaf>();

    private void Awake()
    {
        axiome = null;
    }

    public void Step(List<IModule> newAxiome)
    {
        if (axiome == null)
            axiome = newAxiome.First();
        branches = new List<Branch>();
        leafs = new List<Leaf>();
        var direction = axiome.GetParameterValue(Directionstring, Orientation.Idle);
        var initialState = new TurtleState(Vector3.zero, direction.up);
        var stateStack = new Stack<Tuple<TurtleState, IModule>>();
        stateStack.Push(new Tuple<TurtleState, IModule>(initialState, axiome));

        while (stateStack.Count > 0)
        {
            var t = stateStack.Pop();
            var startState = t.Item1;
            var currentIModule = t.Item2;
            var currentState = startState.Step(length * currentIModule.GetParameterValue(Lengthstring, 0f));
            var isLeaf = currentIModule.GetParameterValue<bool>(IsLeafstring, false);
            if (isLeaf)
            {
                leafs.Add(new Leaf(currentState.position,
                    currentIModule.GetParameterValue<float>(StartWidth),
                    currentIModule.GetParameterValue(ColorSymb, Color.green),
                    currentIModule.GetParameterValue(leafPowerSymb, 2f)));
            }
            else
            {
                branches.Add(new Branch(startState.position, currentState.position,
                    currentIModule.GetParameterValue(ColorSymb, Color.red),
                    currentIModule.GetParameterValue<float>(StartWidth),
                    currentIModule.GetParameterValue<float>(EndWidth), clampBranchWidth));
            }
            
            foreach (var successor in currentIModule.Successors)
            {
                if (successor.Id == Nopstring)
                {
                    continue;
                }
                var angle = successor.GetParameterValue(Directionstring, Orientation.Idle).up;
                var newState = new TurtleState(currentState.position, angle);
                stateStack.Push(new Tuple<TurtleState, IModule>(newState, successor));
            }
        }
    }

    public Branch[] GetBranches()
    {
        return branches.ToArray();
    }

    public Leaf[] GetLeafs()
    {
        //return new Leaf[0];
        return leafs.ToArray();
    }
}
