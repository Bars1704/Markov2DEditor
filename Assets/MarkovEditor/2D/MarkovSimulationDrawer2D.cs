using MarkovTest;
using MarkovTest.TwoDimension;
using UnityEditor;
using UnityEngine;

namespace MarkovEditor._2D
{
    [CreateAssetMenu(menuName = "UnityMarkov/Simulation2D")]
    public class MarkovSimulationDrawer2D : ScriptableObject, IMarkovSimulationDrawer
    {
        [SerializeField]private ColorPalette _colorPaletteLink;
        public ColorPalette ColorPaletteLink => _colorPaletteLink;
        public MarkovSimulation<byte> MarkovSimulation { get; set; } = new MarkovSimulation<byte>();
        public IMarkovSimulation<byte> Simulation { get => MarkovSimulation; set => MarkovSimulation = value as MarkovSimulation<byte>; }

        public string SerializedSimulation{ get; set; }


        private SpriteRenderer[,] visualizationMatrix;

        public void Visualize(GameObject root)
        {
            if (visualizationMatrix == default)
                InitVisualizationMatrix(root, MarkovSimulation.Size.X, MarkovSimulation.Size.Y);

            var sizeX = visualizationMatrix.GetLength(0);
            var sizeY = visualizationMatrix.GetLength(1);

            if (sizeX != MarkovSimulation.Size.X ||
                sizeY != MarkovSimulation.Size.Y)
                InitVisualizationMatrix(root, MarkovSimulation.Size.X, MarkovSimulation.Size.Y);

            foreach (var x in visualizationMatrix)
            {
                if (x != default) continue;
                InitVisualizationMatrix(root, MarkovSimulation.Size.X, MarkovSimulation.Size.Y);
                break;
            }

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    visualizationMatrix[x, y].color = ColorPaletteLink.GetColor(MarkovSimulation[x, y]);
                }
            }
        }

        private void InitVisualizationMatrix(GameObject root, int sizeX, int sizeY)
        {
            if (visualizationMatrix != default)
                foreach (var spriteRenderer in visualizationMatrix)
                    if (spriteRenderer != default)
                        DestroyImmediate(spriteRenderer.gameObject);

            visualizationMatrix = new SpriteRenderer[sizeX, sizeY];
            Sprite s = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Editor/Resources/cell.png", typeof(Sprite));

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    var obj = new GameObject($"cell {x},{y}");
                    obj.transform.localPosition = new Vector3(x, sizeY - y, 0);
                    obj.transform.SetParent(root.transform);
                    var renderer = obj.AddComponent<SpriteRenderer>();
                    renderer.sprite = s;
                    visualizationMatrix[x, y] = renderer;
                }
            }
        }
    }
}