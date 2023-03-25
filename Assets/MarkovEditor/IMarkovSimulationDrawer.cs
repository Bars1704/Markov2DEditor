using MarkovTest;
using UnityEngine;

namespace MarkovEditor
{
    public interface IMarkovSimulationDrawer
    {
        IMarkovSimulation<byte> Simulation { get; set; }
        [HideInInspector] public string SerializedSimulation { get; set; }
        void Visualize(GameObject root);
    }
}