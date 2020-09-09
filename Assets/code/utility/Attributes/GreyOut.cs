using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Multiplayer.Utilies.Attribute
{
    //this will not allow us to change value from the insecpector
    // this is an advances thing for making clean inspector
    public class GreyOut : PropertyAttribute
    {
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GreyOut))]
    public class GreyOutDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}