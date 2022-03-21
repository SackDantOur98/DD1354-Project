using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelpers {

    //Vector1 onto vector2
    public float ProjectOntoVector(Vector3 vector1, Vector3 vector2) {
        Vector3 projectedVector = (Vector3.Dot(vector1, vector2) / Mathf.Pow(vector2.magnitude, 2)) * vector2;

        //The sight should only be on one side of the fish, that is to say, one side of the vector.
        if (vector2.x > 0 && projectedVector.x < 0) return -1;
        else if (vector2.x < 0 && projectedVector.x > 0) return -1;
        else if (vector2.y > 0 && projectedVector.y < 0) return -1;
        else if (vector2.y < 0 && projectedVector.y > 0) return -1;
        else if (vector2.z > 0 && projectedVector.z < 0) return -1;
        else if (vector2.z < 0 && projectedVector.z > 0) return -1;

        return projectedVector.magnitude;
    }

    /*
     The original code for the following functions can be found here:
     https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/HandleUtility.cs
    */
    // Project /point/ onto a line.
    public Vector3 ProjectPointOntoLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        Vector3 relativePoint = point - lineStart;
        Vector3 lineDirection = lineEnd - lineStart;
        float length = lineDirection.magnitude;
        Vector3 normalizedLineDirection = lineDirection;

        if (length > 0.000001f)
            normalizedLineDirection /= length;

        float dot = Vector3.Dot(normalizedLineDirection, relativePoint);
        dot = Mathf.Clamp(dot, 0.0F, length);

        return lineStart + normalizedLineDirection * dot;
    }

    // Calculate distance between a point and a line.
    public float DistancePointToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        float lengthOfDistance = Vector3.Magnitude(ProjectPointOntoLine(point, lineStart, lineEnd) - point);
        return lengthOfDistance;
    }
}
