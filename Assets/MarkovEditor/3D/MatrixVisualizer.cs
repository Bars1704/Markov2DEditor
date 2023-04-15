using System;
using System.Collections.Generic;
using MarkovEditor.Settings;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3Int = Markov.MarkovTest.ThreeDimension.Vector3Int;

namespace MarkovEditor._3D
{
    public static class MatrixVisualizer3D
    {
        private static MeshRenderer[,,] visualizationMatrix;
        private static ColorPalette ColorPaletteLink;
        private static readonly Dictionary<byte, Material> _cachedMaterials = new Dictionary<byte, Material>();

        public static void CacheMaterials()
        {
            _cachedMaterials.Clear();

            var defaultMat = EditorResources.Instance.DefaultMaterial;
            for (byte i = 0; i < ColorPaletteLink.Length; i++)
            {
                var newMat = Object.Instantiate(defaultMat);
                newMat.SetColor("_Color", ColorPaletteLink.GetColor(i));
                _cachedMaterials.Add(i, newMat);
            }
        }

        private static void InitVisualizationMatrix(GameObject root, int sizeX, int sizeY, int sizeZ)
        {
            if (visualizationMatrix != default)
                foreach (var spriteRenderer in visualizationMatrix)
                    if (spriteRenderer != default)
                        Object.DestroyImmediate(spriteRenderer.gameObject);

            visualizationMatrix = new MeshRenderer[sizeX, sizeY, sizeZ];

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    for (var z = 0; z < sizeZ; z++)
                    {
                        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.name = $"cell {x.ToString()},{y.ToString()},{z.ToString()}";
                        obj.transform.localPosition = new Vector3(x, sizeY - y, z);
                        obj.transform.SetParent(root.transform);
                        var renderer = obj.GetComponent<MeshRenderer>();
                        renderer.material = _cachedMaterials[0];
                        visualizationMatrix[x, y, z] = renderer;
                    }
                }
            }
        }

        public static void Visualize(Func<int, int, int, byte> getElem, Vector3Int size,ColorPalette colorPalette, GameObject rootGameObject)
        {
            ColorPaletteLink = colorPalette;
            CacheMaterials();

            if (visualizationMatrix == default)
                InitVisualizationMatrix(rootGameObject, size.X, size.Y, size.Z);

            var sizeX = visualizationMatrix.GetLength(0);
            var sizeY = visualizationMatrix.GetLength(1);
            var sizeZ = visualizationMatrix.GetLength(2);

            if (sizeX != size.X ||
                sizeY != size.Y ||
                sizeZ != size.Z)
                InitVisualizationMatrix(rootGameObject, size.X, size.Y, size.Z);

            foreach (var x in visualizationMatrix)
            {
                if (x != default) continue;
                InitVisualizationMatrix(rootGameObject, size.X, size.Y, size.Z);
                break;
            }

            for (var x = 0; x < sizeX; x++)
            for (var y = 0; y < sizeY; y++)
            for (var z = 0; z < sizeZ; z++)
            {
                visualizationMatrix[x, y, z].gameObject.SetActive(getElem.Invoke(x, y, z) != 0);
                visualizationMatrix[x, y, z].material = _cachedMaterials[getElem.Invoke(x, y, z)];
            }
        }
    }
}