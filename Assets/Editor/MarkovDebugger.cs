using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

public class MarkovDebugger : EditorWindow
{
    private static ColorPalette obj;
    public static TextAsset _asset;
    public SimplePatternContainer _Container;

    [MenuItem("Window/MarkowDebugger")]
    public static void OpenWindow()
    {
        obj = ScriptableObject.CreateInstance<ColorPalette>();

        GetWindow<MarkovDebugger>();
    }

    private void OnGUI()
    {
        SerializedObject serializedObject = new SerializedObject(obj);
        SerializedProperty serializedPropertyMyInt = serializedObject.FindProperty("Colors");

        //EditorGUI.PropertyField(new Rect(Vector2.one, Vector2.one * 1000), serializedPropertyMyInt);
        _asset = (TextAsset)EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Find Dependency", _asset, typeof(TextAsset));
    }
}
#endif