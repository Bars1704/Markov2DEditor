using Markov.MarkovTest;
using Markov.MarkovTest.TwoDimension;
using UnityEditor;
using UnityEngine;

namespace MarkovEditor._2D
{
    [CreateAssetMenu(menuName = "UnityMarkov/Simulations/Simulation2D", order = 1)]
    public class MarkovSimulationDrawer2D : ScriptableObject, IMarkovSimulationDrawer
    {
        [SerializeField] private ColorPalette _colorPaletteLink;
        public ColorPalette ColorPaletteLink => _colorPaletteLink;
        public MarkovSimulation<byte> MarkovSimulation { get; set; } = new MarkovSimulation<byte>();

        public IMarkovSimulation<byte> Simulation
        {
            get => MarkovSimulation;
            set => MarkovSimulation = value as MarkovSimulation<byte>;
        }

        public string SerializedSimulation { get; set; }
    }
}