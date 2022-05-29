using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public static class Tools
    {
        public static Vector3 RotateVector(Vector3 input, float angleWithDir, float angleAlongDir)
        {
            var otherDir = new Vector3(0, 1, 0);
            if (Mathf.Approximately(Vector3.Cross(input, otherDir).magnitude, 0))
                otherDir = new Vector3(1, 0, 0);
            var perp = Vector3.Cross(input, otherDir).normalized;
            var rotation = Quaternion.AngleAxis(angleAlongDir, input) * Quaternion.AngleAxis(angleWithDir, perp);
            return rotation * input;
        }

        public static float Cbrt(float value)
        {
            return (float)System.Math.Pow(value, (double)1 / 3);
        }
    }
}
