using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI()
    {
        var fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.range);

        var viewAngleA = fov.DirectionFromAngle(-fov.angle / 2, false);
        var viewAngleB = fov.DirectionFromAngle(fov.angle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.range);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.range);

        Handles.color = Color.red;
        foreach (var target in fov.visibleTargets)
        {

            Handles.DrawLine(fov.transform.position, target.position);
        }
    }
}
