// C# example.
using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(Dungeon.Inventory))]
class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //target.lookAtPoint = EditorGUILayout.Vector3Field("Look At Point", target.lookAtPoint);
        base.OnInspectorGUI();

        EditorGUILayout.PrefixLabel("And now some buttons...");
        if (GUILayout.Button("Add Item"))
        {
            Debug.Log("Add Inventory Item button clicked");
        }
        if (GUILayout.Button("Remove Item"))
        {
            Debug.Log("Remove Inventory Item button clicked");
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);

    }
}
