using System;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class StampDrawer : IEditorElementDrawer<byte[,]>
    {
        public byte[,] Draw(byte[,] elem, MarkovSimulation sim)
        {
            EditorGUILayout.BeginVertical();
            for (var y = 0; y < elem.GetLength(1); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < elem.GetLength(0); x++)
                    DrawStampElement(elem[x, y], sim, () => elem[x, y] = ColorPalette.CurrentColorIndex);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();
            return elem;
        }

        private static void DrawStampElement(byte stampElement, MarkovSimulation sim, Action OnClicked)
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