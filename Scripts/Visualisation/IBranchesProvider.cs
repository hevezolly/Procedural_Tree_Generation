using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IBranchProvider
{
    Branch[] GetBranches();
}

public interface ILeafProvider
{
    Leaf[] GetLeafs();
}

public interface ITreePartsProvider: IBranchProvider, ILeafProvider { }
