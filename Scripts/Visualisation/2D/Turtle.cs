using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TurtleState
{
    public readonly Vector3 position;
    public readonly Vector3 heading;

    private float GetAnge()
    {
        return Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetHeading(float angle)
    {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public TurtleState(Vector3 position, Vector3 heading)
    {
        this.position = position;
        this.heading = heading.normalized;
    }

    public TurtleState Step(float distance)
    {
        return new TurtleState(position + heading * distance, heading);
    }

    public TurtleState Rotate(float angle)
    {
        return new TurtleState(position, GetHeading(GetAnge() + angle));
    }

    public TurtleState SetAngle(float angle, Vector3 initialDir)
    {
        return new TurtleState(position, 
            GetHeading(Mathf.Atan2(initialDir.y, initialDir.x) * Mathf.Rad2Deg + angle));
    }

    public TurtleState SetAngle(float angle)
    {
        return SetAngle(angle, Vector3.up);
    }
}
