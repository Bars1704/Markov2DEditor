using MarkovTest.TwoDimension;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityMarkov/Simulation")]
public class MarkovSimulation : ScriptableObject
{
    public ColorPalette ColorPaletteLink;
    public MarkovSimulationTwoDim<byte> Simulation = new MarkovSimulationTwoDim<byte>();
    [HideInInspector] public string SerializedSimulation;


    private SpriteRenderer[,] visualizationMatrix;

    public void Visualize(GameObject root)
    {
        if (visualizationMatrix == default)
            InitVisualizationMatrix(root, Simulation.Size.X, Simulation.Size.Y);

        var sizeX = visualizationMatrix.GetLength(0);
        var sizeY = visualizationMatrix.GetLength(1);

        if (sizeX != Simulation.Size.X ||
            sizeY != Simulation.Size.Y)
            InitVisualizationMatrix(root, Simulation.Size.X, Simulation.Size.Y);

        foreach (var x in visualizationMatrix)
        {
            if (x != default) continue;
            InitVisualizationMatrix(root, Simulation.Size.X, Simulation.Size.Y);
            break;
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                visualizationMatrix[x, y].color = ColorPaletteLink.GetColor(Simulation[x, y]);
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