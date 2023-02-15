using System;
using MarkovTest.TwoDimension.Patterns;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class PatternDrawer : IEditorElementDrawer<Pattern<byte>>
    {
        public Pattern<byte> Draw(Pattern<byte> elem, MarkovSimulation sim)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            for (var y = 0; y < elem.PatternForm.GetLength(1); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < elem.PatternForm.GetLength(0); x++)
                    DrawPatternElement(elem.PatternForm[x, y], sim,
                        () => elem.PatternForm[x, y] = ColorPalette.CurrentColorIndex);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            return elem;
        }

        private static void DrawPatternElement(IEquatable<byte> patternElement, MarkovSimulation sim, Action OnClicked)
        {
            var style = new GUIStyle
            {
                normal =
                {
                    background = Texture2D.whiteTexture
                },
                margin = new RectOffset(5, 5, 0, 0)
            };

            var defaultColor = GUI.backgroundColor;

            patternElement ??= (byte)0;

            if (!(patternElement is byte b))
            {
                Debug.LogError($"{patternElement} doesnt has drawer!");
                return;
            }

            GUI.backgroundColor = sim.ColorPaletteLink.GetColor(b);
            if (GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20)))
                OnClicked?.Invoke();
            GUI.backgroundColor = defaultColor;
        }
    }
}