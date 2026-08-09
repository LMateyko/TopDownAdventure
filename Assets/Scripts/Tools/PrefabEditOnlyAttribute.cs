using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PrefabEditOnlyAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(PrefabEditOnlyAttribute))]
public class PrefabEditOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        PrefabEditOnlyAttribute conditionalAttribute = (PrefabEditOnlyAttribute)attribute;
        bool shouldBeReadOnly = !IsInPrefabMode(conditionalAttribute);

        EditorGUI.BeginDisabledGroup(shouldBeReadOnly);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }

    private bool IsInPrefabMode(PrefabEditOnlyAttribute attribute)
    {
#if UNITY_EDITOR
        var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
        return currentStage != null;
#else
        return true;
#endif
    }
}