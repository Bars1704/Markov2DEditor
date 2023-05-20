using System;
using System.Collections.Generic;
using Markov.MarkovTest.ThreeDimension.Patterns;
using MarkovEditor;
using MarkovEditor._3D;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class PatternDrawer : IEditorElementDrawer<Pattern<byte>, MarkovSimulationDrawer3D>
    {
        private bool[] _foldoutsMasks = Array.Empty<bool>();

        private static bool[] Resize(IReadOnlyList<bool> source, int newSize)
        {
            var result = new bool[newSize];
            var minSize = Math.Min(source.Count, newSize);
            for (var i = 0; i < minSize; i++)
                result[i] = source[i];
            return result;
        }

        public Pattern<byte> Draw(Pattern<byte> elem, MarkovSimulationDrawer3D sim)
        {
            if (_foldoutsMasks.Length < elem.PatternForm.GetLength(1))
                _foldoutsMasks = Resize(_foldoutsMasks, elem.PatternForm.GetLength(1));

            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel++;
            for (var y = 0; y < elem.PatternForm.GetLength(1); y++)
            {
                _foldoutsMasks[y] = EditorGUILayout.Foldout(_foldoutsMasks[y], $"Y Layer: {y.ToString()}");
                if (!_foldoutsMasks[y]) continue;
                for (var z = 0; z < elem.PatternForm.GetLength(2); z++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (var x = 0; x < elem.PatternForm.GetLength(0); x++)
                    {
                        var x1 = x;
                        var z1 = z;
                        DrawPatternElement(elem.PatternForm[x, y, z], sim,
                            () => elem.PatternForm[x1, y, z1] = ColorPalette.CurrentColorIndex);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                }
            }

            EditorGUILayout.EndVertical();
            return elem;
        }

        private static void DrawPatternElement(IEquatable<byte> patternElement, MarkovSimulationDrawer3D sim,
            Action OnClicked)
        {
            if (patternElement == default) return;

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