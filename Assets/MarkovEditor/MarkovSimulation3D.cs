using MarkovTest.TwoDimension;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityMarkov/Simulation3D")]
public class MarkovSimulation3D : ScriptableObject
{
    public ColorPalette ColorPaletteLink;
    public MarkovSimulationTwoDim<byte> Simulation = new MarkovSimulationTwoDim<byte>();
    [HideInInspector] public string SerializedSimulation;

}