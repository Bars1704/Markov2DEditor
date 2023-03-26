using System;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class StampDrawer : IEditorElementDrawer<byte[,,], MarkovSimulationDrawer3D>
    {
        private int _currentZIndex;

        public byte[,,] Draw(byte[,,] elem, MarkovSimulationDrawer3D sim)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Z index");
            _currentZIndex = EditorGUILayout.IntField(_currentZIndex, GUILayout.Width(400));
            _currentZIndex = Mathf.Clamp(_currentZIndex, 0, elem.GetLength(2) - 1);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            for (var y = 0; y < elem.GetLength(1); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < elem.GetLength(0); x++)
                    DrawStampElement(elem[x, y, _currentZIndex], sim,
                        () => elem[x, y, _currentZIndex] = ColorPalette.CurrentColorIndex);

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