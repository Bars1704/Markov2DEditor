using MarkovTest.TwoDimension.Patterns;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Pattern<byte>))] 
public class SimplePatternEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var x = target.GetType().ToString();
        GUILayout.Space(20f);
        GUILayout.Label($"Custom Editor Elements {x}", EditorStyles.boldLabel); //3
    }
}