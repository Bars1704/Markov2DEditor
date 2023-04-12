using System;
using MarkovTest.ThreeDimension.Patterns;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class PatternDrawer : IEditorElementDrawer<Pattern<byte>, MarkovSimulationDrawer3D>
    {
        private int _currentYIndex;

        public Pattern<byte> Draw(Pattern<byte> elem, MarkovSimulationDrawer3D sim)
        {
            elem.RotationSettings = new RotationSettingsDrawer().Draw(elem.RotationSettings, sim);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Y index");
            _currentYIndex = EditorGUILayout.IntField(_currentYIndex, GUILayout.Width(400));
            _currentYIndex = Mathf.Clamp(_currentYIndex, 0, elem.PatternForm.GetLength(1) - 1);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            for (var z = 0; z < elem.PatternForm.GetLength(2); z++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < elem.PatternForm.GetLength(0); x++)
                {
                    var x1 = x;
                    var z1 = z;
                    DrawPatternElement(elem.PatternForm[x, _currentYIndex, z], sim,
                        () => elem.PatternForm[x1, _currentYIndex, z1] = ColorPalette.CurrentColorIndex);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();
            return elem;
        }

        private static void DrawPatternElement(IEquatable<byte> patternElement, MarkovSimulationDrawer3D sim,
            Action OnClicked)
        {
            if(patternElement == default) return;
            
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