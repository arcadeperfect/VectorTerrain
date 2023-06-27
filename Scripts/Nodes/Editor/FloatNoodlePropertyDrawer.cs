// using Sirenix.OdinInspector.Editor;
// using UnityEditor;
// using UnityEngine;
// using VectorTerrain.Scripts.Types;
//
// namespace VectorTerrain.Scripts.Nodes.Editor
// {
//     // [CustomPropertyDrawer(typeof(FloatNoodle))]
//     // public class FloatNoodlePropertyDrawer : PropertyDrawer
//     // { 
//     //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     //     {
//     //         EditorGUI.BeginChangeCheck();
//     //         var v = property.FindPropertyRelative("value").floatValue;
//     //         
//     //         float newValue = EditorGUI.FloatField(position, label, v);
//     //         if (EditorGUI.EndChangeCheck())
//     //         {
//     //             property.floatValue = newValue;
//     //         }
//     //     }
//     // }
//     
//     // public class OdinFloatNoodleDrawer : OdinValueDrawer<FloatNoodle>
//     // {
//     //     protected override void DrawPropertyLayout(GUIContent label)
//     //     {
//     //         Rect rect = EditorGUILayout.GetControlRect();
//     //         FloatNoodle value = this.ValueEntry.SmartValue;
//     //         value.value = EditorGUI.FloatField(rect, label, value.value);
//     //         this.ValueEntry.SmartValue = value;
//     //     }
//     // }
// }