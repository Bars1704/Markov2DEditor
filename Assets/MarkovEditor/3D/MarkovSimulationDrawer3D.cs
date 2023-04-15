using Markov.MarkovTest;
using Markov.MarkovTest.ThreeDimension;
using UnityEngine;

namespace MarkovEditor._3D
{
    [CreateAssetMenu(menuName = "UnityMarkov/Simulations/Simulation3D", order = 2)]
    public class MarkovSimulationDrawer3D : ScriptableObject, IMarkovSimulationDrawer
    {
        [SerializeField] private ColorPalette _colorPaletteLink;
        public ColorPalette ColorPaletteLink => _colorPaletteLink;
        public string SerializedSimulation { get; set; }
        public MarkovSimulation<byte> MarkovSimulation { get; private set; } = new MarkovSimulation<byte>();

        public IMarkovSimulation<byte> Simulation
        {
            get => MarkovSimulation;
            set => MarkovSimulation = value as MarkovSimulation<byte>;
        }
    }
}