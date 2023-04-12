using System;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class StampDrawer : IEditorElementDrawer<byte[,,], MarkovSimulationDrawer3D>
    {
        private int _currentYIndex;
        private bool[] _foldoutsMasks;
        public byte[,,] Draw(byte[,,] elem, MarkovSimulationDrawer3D sim)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Y index");
            _currentYIndex = EditorGUILayout.IntField(_currentYIndex, GUILayout.Width(400));
            _currentYIndex = Mathf.Clamp(_currentYIndex, 0, elem.GetLength(1) - 1);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            for (var z = 0; z < elem.GetLength(2); z++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < elem.GetLength(0); x++)
                {
                    var x1 = x;
                    var z1 = z;
                    DrawStampElement(elem[x, _currentYIndex, z], sim,
                        () => elem[x1, _currentYIndex, z1
                        ] = ColorPalette.CurrentColorIndex);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }

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