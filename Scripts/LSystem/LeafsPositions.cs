using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LSystem
{
    public class LeafsPositions
    {
        private Dictionary<int, Vector3> positions;

        public LeafsPositions()
        {
            positions = new Dictionary<int, Vector3>();
        }

        public LeafsPositions Update(int index, Vector3 position)
        {
            positions[index] = position;
            return this;
        }

        public LeafsPositions Remove(int index)
        {
            positions.Remove(index);
            return this;
        }

        public Vector3 Avg()
        {
            if (positions.Count == 0)
                return Vector3.up;
            var value = Vector3.up;
            foreach (var leaf in positions.Values)
            {
                value += leaf;
            }
            return value / positions.Count;
        }

        public Vector3 DirToAvg(Vector3 from, float seed, float randomnes)
        {
            var s = (int)(seed * 10000000000);
            var random = new System.Random(s);
            var phi = ((float)random.NextDouble()) * Mathf.PI * 2;
            var costheta = (float)((random.NextDouble() - 0.5) * 2);

            var theta = Mathf.Acos(costheta);
            var x = Mathf.Sin(theta) * Mathf.Cos(phi);
            var y = Mathf.Sin(theta) * Mathf.Sin(phi);
            var z = Mathf.Cos(theta);
            var rand = new Vector3(x, y, z);
            if (positions.Count == 0)
            {
                return rand;
            }
            return Vector3.Lerp((Avg() - from).normalized, rand, randomnes).normalized;
        }
    }
}
