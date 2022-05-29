using LSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStepReciver
{
    void Step(List<IModule> state);
}
