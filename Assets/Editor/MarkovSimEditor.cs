using Editor.EditorElementDrawers;
using MarkovTest;
using MarkovTest.Serialization;
using MarkovTest.TwoDimension;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable AccessToModifiedClosure


namespace Editor
{
    [CustomEditor(typeof(MarkovSimulation))]
    public class MarkovSimEditor : UnityEditor.Editor
    {
        private static readonly SceneContext _sceneContext = new SceneContext();
        Vector2 scrollPosition;
        private bool _isShowDefaultState;
        private bool _isShowSerialized;

        private int seed;

        private void Awake()
        {
            Load();
        }

        private void OnDestroy()
        {
            Save();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var sim = (MarkovSimulation)target;

            if (sim.ColorPaletteLink == default)
            {
                EditorGUILayout.HelpBox("Palette is not set!", MessageType.Error);
            }
            else
                new PaletteDrawer().Draw(sim.ColorPaletteLink, sim);

            DrawSerializableField(sim);
            DrawSimulation(sim.Simulation, sim);


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
                Save();

            if (GUILayout.Button("Rewind"))
                Load();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Exit"))
                RunOneStep();

            if (GUILayout.Button("Run"))
                Run();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Init"))
            {
                sim.Simulation = SimulationFabric.Labirynth();
                Save();
            }
        }

        private void DrawSerializableField(MarkovSimulation sim)
        {
            _isShowSerialized = EditorGUILayout.Foldout(_isShowSerialized, "Serialized");
            if (_isShowSerialized)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
                EditorGUILayout.TextArea(sim.SerializedSimulation);
                EditorGUILayout.EndScrollView();
            }
        }


        private void DrawSimulation(MarkovSimulationTwoDim<byte> sim2dim, MarkovSimulation sim)
        {
            EditorGUILayout.BeginHorizontal();
            seed = EditorGUILayout.IntField("Seed", seed);
            if (GUILayout.Button("Random"))
                seed = Random.Range(0, int.MaxValue);
            EditorGUILayout.EndHorizontal();
            if (sim2dim.DefaultState != default)
                new ResizableDrawer().Draw(sim2dim, sim);

            _isShowDefaultState = EditorGUILayout.Foldout(_isShowDefaultState, "Initial state");
            if (_isShowDefaultState)
                new StampDrawer().Draw(sim2dim.DefaultState, sim);

            new PlayableListDrawer().Draw(sim2dim.Playables, sim);
        }


        private void Save()
        {
            MarkovSimulation sim = (MarkovSimulation)target;
            sim.SerializedSimulation = SimulationSerializer.SerializeSim(sim.Simulation);
            EditorUtility.SetDirty(sim);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Load()
        {
            MarkovSimulation sim = (MarkovSimulation)target;
            sim.Simulation = SimulationSerializer.DeserializeSim<byte>(sim.SerializedSimulation);
        }


        private void RunOneStep()
        {
            _sceneContext.Exit();
        }

        private void Run()
        {
            if (!_sceneContext.IsActive())
                _sceneContext.Enter();
            MarkovSimulation sim = (MarkovSimulation)target;
            sim.Simulation.Play(sim.Simulation, seed);
            sim.Visualize(_sceneContext.rootGameObject);
        }
    }
}