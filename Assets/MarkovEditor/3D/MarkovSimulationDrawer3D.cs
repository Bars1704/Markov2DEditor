using System.Collections.Generic;
using MarkovEditor;
using MarkovTest;
using MarkovTest.ThreeDimension;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityMarkov/Simulations/Simulation3D", order = 2)]
public class MarkovSimulationDrawer3D : ScriptableObject, IMarkovSimulationDrawer
{
    [SerializeField] private ColorPalette _colorPaletteLink;
    public ColorPalette ColorPaletteLink => _colorPaletteLink;
    public string SerializedSimulation { get; set; }

    private readonly Dictionary<byte, Material> _cachedMaterials = new Dictionary<byte, Material>();

    private MeshRenderer[,,] visualizationMatrix;

    public MarkovSimulation<byte> MarkovSimulation { get; set; } = new MarkovSimulation<byte>();

    public IMarkovSimulation<byte> Simulation
    {
        get => MarkovSimulation;
        set => MarkovSimulation = value as MarkovSimulation<byte>;
    }

    public void CacheMaterials()
    {
        _cachedMaterials.Clear();

        var defaultMat =
            (Material)AssetDatabase.LoadAssetAtPath("Assets/Editor/Resources/DefaultMaterial.mat", typeof(Material));
        for (byte i = 0; i < ColorPaletteLink.Length; i++)
        {
            var newMat = Instantiate(defaultMat);
            newMat.SetColor("_Color", ColorPaletteLink.GetColor(i));
            _cachedMaterials.Add(i, newMat);
        }
    }

    public void Visualize(GameObject root)
    {
            CacheMaterials();

        if (visualizationMatrix == default)
            InitVisualizationMatrix(root, MarkovSimulation.Size.X, MarkovSimulation.Size.Y, MarkovSimulation.Size.Z);

        var sizeX = visualizationMatrix.GetLength(0);
        var sizeY = visualizationMatrix.GetLength(1);
        var sizeZ = visualizationMatrix.GetLength(2);

        if (sizeX != MarkovSimulation.Size.X ||
            sizeY != MarkovSimulation.Size.Y ||
            sizeZ != MarkovSimulation.Size.Z)
            InitVisualizationMatrix(root, MarkovSimulation.Size.X, MarkovSimulation.Size.Y, MarkovSimulation.Size.Z);

        foreach (var x in visualizationMatrix)
        {
            if (x != default) continue;
            InitVisualizationMatrix(root, MarkovSimulation.Size.X, MarkovSimulation.Size.Y, MarkovSimulation.Size.Z);
            break;
        }

        for (var x = 0; x < sizeX; x++)
        for (var y = 0; y < sizeY; y++)
        for (var z = 0; z < sizeZ; z++)
        {
            visualizationMatrix[x, y, z].gameObject.SetActive(MarkovSimulation[x, y, z] != 0);
            visualizationMatrix[x, y, z].material = _cachedMaterials[MarkovSimulation[x, y, z]];
        }
    }


    private void InitVisualizationMatrix(GameObject root, int sizeX, int sizeY, int sizeZ)
    {
        if (visualizationMatrix != default)
            foreach (var spriteRenderer in visualizationMatrix)
                if (spriteRenderer != default)
                    DestroyImmediate(spriteRenderer.gameObject);

        visualizationMatrix = new MeshRenderer[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.name = $"cell {x},{y},{z}";
                    obj.transform.localPosition = new Vector3(x, sizeY - y, z);
                    obj.transform.SetParent(root.transform);
                    var renderer = obj.GetComponent<MeshRenderer>();
                    renderer.material = _cachedMaterials[0];
                    visualizationMatrix[x, y, z] = renderer;
                }
            }
        }
    }
}