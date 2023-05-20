using System;
using System.Collections.Generic;
using MarkovEditor;
using MarkovEditor._3D;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class StampDrawer : IEditorElementDrawer<byte[,,], MarkovSimulationDrawer3D>
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

        public byte[,,] Draw(byte[,,] elem, MarkovSimulationDrawer3D sim)
        {
            if (_foldoutsMasks.Length < elem.GetLength(1))
                _foldoutsMasks = Resize(_foldoutsMasks, elem.GetLength(1));
            EditorGUILayout.BeginVertical();

            EditorGUI.indentLevel++;
            for (var y = 0; y < elem.GetLength(1); y++)
            {
                _foldoutsMasks[y] = EditorGUILayout.Foldout(_foldoutsMasks[y], $"Y Layer: {y.ToString()}");
                if (!_foldoutsMasks[y]) continue;
                
                for (var z = 0; z < elem.GetLength(2); z++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (var x = 0; x < elem.GetLength(0); x++)
                    {
                        var x1 = x;
                        var z1 = z;
                        DrawStampElement(elem[x, y, z], sim,
                            () => elem[x1, y, z1
                            ] = ColorPalette.CurrentColorIndex);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            return elem;
        }

        private static void DrawStampElement(byte stampElement, MarkovSimulationDrawer3D sim, Action OnClicked)
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


            GUI.backgroundColor = sim.ColorPaletteLink.GetColor(stampElement);
            if (GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20)))
                OnClicked?.Invoke();
            GUI.backgroundColor = defaultColor;
        }
    }
}