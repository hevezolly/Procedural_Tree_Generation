using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LSystem {
    public static class LSystemExtensions
    {
        public static void Display(this ILSystem system, bool displayParameters = true)
        {
            Debug.Log(string.Join(" ", system.CurrentState.Select(m => displayParameters ? m.ToString() : m.Id)));
        }
    }
}
