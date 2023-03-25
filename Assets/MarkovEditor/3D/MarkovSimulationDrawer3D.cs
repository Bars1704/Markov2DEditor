using MarkovEditor;
using MarkovTest;
using MarkovTest.ThreeDimension;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityMarkov/Simulation3D")]
public class MarkovSimulationDrawer3D : ScriptableObject, IMarkovSimulationDrawer
{
    public ColorPalette ColorPaletteLink;
    public string SerializedSimulation { get; set; }

    public MarkovSimulation<byte> MarkovSimulation{ get; set; }
    public IMarkovSimulation<byte> Simulation { get => MarkovSimulation; set => MarkovSimulation = value as MarkovSimulation<byte>; }

    public void Visualize(GameObject root)
    {
        throw new System.NotImplementedException();
    }
}