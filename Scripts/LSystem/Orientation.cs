using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public struct Orientation
    {
        public readonly Vector3 forward;
        public readonly Vector3 left;
        public readonly Vector3 up;

        public Orientation(Vector3 f, Vector3 l, Vector3 u)
        {
            forward = f;
            left = l;
            up = u;
        }

        public static Orientation Set(Vector3 u)
        {
            if (Mathf.Approximately(Vector3.Cross(u, Vector3.up).magnitude, 0))
                return Idle;
            var left = Vector3.Cross(Vector3.up, u).normalized;
            var forward = Vector3.Cross(left, u);
            return new Orientation(forward, left, u);
        }

        public static Orientation Idle => new Orientation(Vector3.forward, -Vector3.right, Vector3.up);

        public Orientation Roll(float angle)
        {
            var rotor = Quaternion.AngleAxis(angle, up);
            return new Orientation(rotor * forward, rotor * left, up);
        }

        public Orientation Fall(float angle, float minGroundAngle = 0)
        {
            if (Mathf.Approximately(Vector3.Cross(up, Vector3.up).magnitude, 0))
                return this;
            var norm = Vector3.Cross(up, Vector3.up).normalized;
            var target = Quaternion.AngleAxis(minGroundAngle, norm) * Vector3.Cross(Vector3.up, norm).normalized;
            var realAngle = Vector3.SignedAngle(up, target, norm);
            var rotor = Quaternion.AngleAxis(Mathf.Max(-angle, realAngle), norm);
            return new Orientation(rotor * forward, rotor * left, rotor * up);
        }

        public Orientation Pitch(float angle)
        {
            var rotor = Quaternion.AngleAxis(angle, left);
            return new Orientation(rotor * forward, left, rotor * up);
        }

        public Orientation Turn(float angle)
        {
            var rotor = Quaternion.AngleAxis(angle, forward);
            return new Orientation(forward, rotor * left, rotor * up);
        }

        public Orientation Flat()
        {
            if (Mathf.Approximately(Vector3.Cross(up, Vector3.up).magnitude, 0))
                return this;
            var targetDir = Vector3.Cross(Vector3.up, up);
            var angle = Vector3.SignedAngle(left, targetDir, up);
            return Roll(angle);
        }
    }
}
