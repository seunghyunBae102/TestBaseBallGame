# if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

#region bool -> Invisiable!
[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
    public string boolVariableName;
    public bool inverse;

    public ConditionalFieldAttribute(string boolVariableName, bool inverse = false)
    {
        this.boolVariableName = boolVariableName;
        this.inverse = inverse;
    }
}

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return 0f;

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return;

        EditorGUI.PropertyField(position, property, label, true);
    }

    private bool ShouldShow(SerializedProperty property)
    {
        var cond = attribute as ConditionalFieldAttribute;
        if (cond == null) return true;

        // ???
        string path = property.propertyPath;
        string conditionPath = path.Replace(property.name, cond.boolVariableName);
        var boolProp = property.serializedObject.FindProperty(conditionPath);

        if (boolProp != null && boolProp.propertyType == SerializedPropertyType.Boolean)
        {
            bool val = boolProp.boolValue;
            return cond.inverse ? !val : val;
        }

        return true; // ?? ????? ?????
    }
}
#endregion


#endif