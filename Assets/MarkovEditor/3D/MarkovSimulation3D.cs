using MarkovTest.ThreeDimension;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityMarkov/Simulation3D")]
public class MarkovSimulation3D : ScriptableObject
{
    public ColorPalette ColorPaletteLink;
    public MarkovSimulation<byte> Simulation = new MarkovSimulation<byte>();
    [HideInInspector] public string SerializedSimulation;

}