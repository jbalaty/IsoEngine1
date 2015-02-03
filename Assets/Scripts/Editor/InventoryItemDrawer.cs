using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomPropertyDrawer(typeof(Dungeon.InventoryItem))]
//[ExecuteInEditMode]
public class InventoryItemDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        EditorGUI.indentLevel = 0;
        var width = contentPosition.width;
        contentPosition.width *= 0.1f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("Amount"), GUIContent.none);
        contentPosition.x += contentPosition.width;
        contentPosition.width = width - contentPosition.width;
        var serializedItem = property.FindPropertyRelative("Item");
        EditorGUI.LabelField(contentPosition, serializedItem.FindPropertyRelative("Name").stringValue);
        //EditorGUI.PropertyField(contentPosition, serializedItem.FindPropertyRelative("Name"), GUIContent.none);
        EditorGUI.EndProperty();
    }
}
