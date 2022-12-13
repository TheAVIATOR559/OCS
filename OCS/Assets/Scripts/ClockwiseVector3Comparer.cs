using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockwiseVector3Comparer : IComparer<Vector3>
{
    private Vector3 m_Origin;
    public Vector3 origin { get { return m_Origin; } set { m_Origin = value; } }

    public ClockwiseVector3Comparer(Vector3 origin)
    {
        m_Origin = origin;
    }

    public int Compare(Vector3 a, Vector3 b)
    {
        return IsClockwise(a, b, m_Origin);
    }

    public static int IsClockwise(Vector3 a, Vector3 b, Vector3 origin)
    {
        if(a == b)
        {
            return 0;
        }

        Vector3 firstOffset = a - origin;
        Vector3 secondOffset = b - origin;

        float angleA = Mathf.Atan2(firstOffset.x, firstOffset.y);
        float angleB = Mathf.Atan2(secondOffset.x, secondOffset.y);

        if(angleA < angleB)
        {
            return -1;
        }

        if(angleA > angleB)
        {
            return 1;
        }

        return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? -1 : 1;
    }
}
