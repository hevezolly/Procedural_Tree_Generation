using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Events
{
    [System.Serializable]
    public class ParametrisedEvent<T>: UnityEvent<T> { }

    [System.Serializable]
    public class ModuleListEvent: ParametrisedEvent<List<LSystem.IModule>> { }
}
