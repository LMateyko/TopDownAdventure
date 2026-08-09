using UnityEngine;
using UnityEditor;

public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public bool InverseCondition;

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        ConditionalSourceField = conditionalSourceField;
        InverseCondition = true;
    }
}

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute conditionalAttribute = (ConditionalHideAttribute)attribute;
        bool shouldHide = EvaluateCondition(property, conditionalAttribute);

        // Return zero height if hidden, default otherwise
        return shouldHide ? 0f : EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute conditionalAttribute = (ConditionalHideAttribute)attribute;
        if (!EvaluateCondition(property, conditionalAttribute))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    private bool EvaluateCondition(SerializedProperty property, ConditionalHideAttribute attribute)
    {
        SerializedProperty sourceProperty = property.serializedObject.FindProperty(attribute.ConditionalSourceField);

        if (sourceProperty == null)
        {
            Debug.LogWarning($"ConditionalHide: The field '{attribute.ConditionalSourceField}' was not found.");
            return false;
        }

        if (sourceProperty.propertyType != SerializedPropertyType.Boolean)
        {
            Debug.LogWarning($"ConditionalHide: The field '{attribute.ConditionalSourceField}' must be of type bool.");
            return false;
        }

        bool conditionMet = sourceProperty.boolValue;
        return attribute.InverseCondition ? !conditionMet : conditionMet;
    }
}