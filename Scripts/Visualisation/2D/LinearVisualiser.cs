using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LSystem;

public class LinearVisualiser : MonoBehaviour, ITreePartsProvider, IStepReciver
{

    [SerializeField]
    private string directionParamId;
    [SerializeField]
    private string lengthParamId;
    [SerializeField]
    private string startWidthId;
    [SerializeField]
    private string endWidthId;
    [SerializeField]
    private string leafPowerSymb;
    [SerializeField]
    private string IsLeafParamId;
    [SerializeField]
    private string branchStart;
    [SerializeField]
    private string branchEnd;
    [SerializeField]
    private string colorParam;
    [SerializeField]
    private bool clampBranchWidth;

    private List<Branch> branches;
    private List<Leaf> leafs;


    public void Step(List<IModule> currentState)
    {
        branches = new List<Branch>();
        leafs = new List<Leaf>();
        var turtle = new TurtleState(Vector3.zero, Vector3.up);
        var storedPozs = new Stack<TurtleState>();
        foreach (var module in currentState)
        {
            if (module.Id == branchStart)
            {
                storedPozs.Push(turtle);
                continue;
            }
            if (module.Id == branchEnd)
            {
                turtle = storedPozs.Pop();
                continue;
            }
            var length = module.GetParameterValue(lengthParamId, 0f);
            var position = turtle.position;
            var direction = module.GetParameterValue(directionParamId, Orientation.Idle).up;
            var color = module.GetParameterValue(colorParam, Color.red);
            var startWidth = module.GetParameterValue(startWidthId, 0f);
            var endWidth = module.GetParameterValue(endWidthId, 0f);
            var isLeaf = module.GetParameterValue(IsLeafParamId, false);
            var endPos = position + direction * length;
            if (isLeaf)
            {
                var power = module.GetParameterValue(leafPowerSymb, 3f);
                leafs.Add(new Leaf(position, startWidth, color, power));
            }
            else
            {
                branches.Add(new Branch(position, endPos, color, startWidth, endWidth, clampBranchWidth));
            }
            turtle = new TurtleState(endPos, direction);
        }
    }

    public Branch[] GetBranches()
    {
        return branches.ToArray();
    }

    public Leaf[] GetLeafs()
    {
        return leafs.ToArray();
    }
}
