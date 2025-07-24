using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (EnemyFOV))]
public class FOV_Editor : Editor
{
    void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRad);

        Vector3 viewAng_A = fov.angleDir(-fov.viewAng / 2, false);
        Vector3 viewAng_B = fov.angleDir(fov.viewAng / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAng_A * fov.viewRad);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAng_B * fov.viewRad);

        Handles.color = Color.red;
        foreach (Transform playersFound in fov.foundPlayer) { Handles.DrawLine(fov.transform.position, playersFound.position); }
    }
}
