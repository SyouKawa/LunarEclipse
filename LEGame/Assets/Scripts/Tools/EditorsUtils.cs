using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorsUtils : Editor
{
    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction, Color.blue, 10);
            if (Physics.Raycast(ray, out hit))
            {
                UnityEngine.Debug.Log(hit.collider.gameObject.transform.position);
            }

            UnityEngine.Debug.Log("Left-Mouse Down");
        }
    }

}
