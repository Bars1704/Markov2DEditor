using System;
using MarkovEditor.Settings;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector2Int = Markov.MarkovTest.TwoDimension.Vector2Int;

namespace MarkovEditor._2D
{
    public static class MatrixVisualizer2D
    {
        private static SpriteRenderer[,] visualizationMatrix;
        private static ColorPalette ColorPaletteLink;

        public static void Visualize(Func<int, int, byte> getter, Vector2Int size, ColorPalette colorPalette, GameObject root)
        {
            ColorPaletteLink = colorPalette;
            if (visualizationMatrix == default)
                InitVisualizationMatrix(root, size.X, size.Y);

            var sizeX = visualizationMatrix.GetLength(0);
            var sizeY = visualizationMatrix.GetLength(1);

            if (sizeX != size.X ||
                sizeY != size.Y)
                InitVisualizationMatrix(root, size.X, size.Y);

            foreach (var x in visualizationMatrix)
            {
                if (x != default) continue;
                InitVisualizationMatrix(root, size.X, size.Y);
                break;
            }

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    visualizationMatrix[x, y].color = ColorPaletteLink.GetColor(getter.Invoke(x, y));
                }
            }
        }

        private static void InitVisualizationMatrix(GameObject root, int sizeX, int sizeY)
        {
            if (visualizationMatrix != default)
                foreach (var spriteRenderer in visualizationMatrix)
                    if (spriteRenderer != default)
                        Object.DestroyImmediate(spriteRenderer.gameObject);

            visualizationMatrix = new SpriteRenderer[sizeX, sizeY];
            var s = EditorResources.Instance.DefaultSprite;

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    var obj = new GameObject($"cell {x.ToString()},{y.ToString()}")
                    {
                        transform =
                        {
                            localPosition = new Vector3(x, sizeY - y, 0)
                        }
                    };
                    
                    obj.transform.SetParent(root.transform);
                    var renderer = obj.AddComponent<SpriteRenderer>();
                    renderer.sprite = s;
                    visualizationMatrix[x, y] = renderer;
                }
            }
        }
    }
}