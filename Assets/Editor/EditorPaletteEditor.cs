using MarkovEditor;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(EditorPalette))]
    public class EditorPaletteEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            var palette = target as EditorPalette;
            palette.Fill();
        }

        public override void OnInspectorGUI()
        {
            var palette = target as EditorPalette;

            EditorGUILayout.LabelField("Colors");
            EditorGUILayout.BeginVertical();
            for (var i = 0; i < palette.Colors.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(palette.Colors[i].TypeName);
                palette.Colors[i].Color = EditorGUILayout.ColorField(palette.Colors[i].Color);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}