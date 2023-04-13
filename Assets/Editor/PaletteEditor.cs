using MarkovEditor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ColorPalette))]
    public class PaletteEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var palette = (ColorPalette)target;
            var buttonStyle = new GUIStyle
            {
                normal =
                {
                    background = Texture2D.whiteTexture
                },
                margin = new RectOffset(5, 5, 0, 0)
            };

            EditorGUILayout.LabelField("Current Selected Color:");
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = palette.GetColor(ColorPalette.CurrentColorIndex);
            GUILayout.Button("", buttonStyle);
            GUI.backgroundColor = defaultColor;
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Palette:");
            EditorGUILayout.BeginHorizontal();
            defaultColor = GUI.backgroundColor;
            for (byte i = 0; i < palette.Length; i++)
            {
                GUI.backgroundColor = palette.GetColor(i);
                if (GUILayout.Button(i.ToString(), buttonStyle, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    ColorPalette.CurrentColorIndex = i;
                }

            }
            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = defaultColor;
        
        
            DrawDefaultInspector();
        }
    }
}