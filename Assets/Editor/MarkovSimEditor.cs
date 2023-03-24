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
    [CustomEditor(typeof(MarkovSimulation2D))]
    public class MarkovSimEditor : UnityEditor.Editor
    {
        private static readonly SceneContext _sceneContext = new SceneContext();
        private Vector2 serializedScrollPosition;
        private Vector2 defaultStateScrollPosition;
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

            var sim = (MarkovSimulation2D)target;

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

        private void DrawSerializableField(MarkovSimulation2D sim)
        {
            _isShowSerialized = EditorGUILayout.Foldout(_isShowSerialized, "Serialized");
            if (_isShowSerialized)
            {
                serializedScrollPosition =
                    GUILayout.BeginScrollView(serializedScrollPosition, GUILayout.MaxHeight(500));
                EditorGUILayout.TextArea(sim.SerializedSimulation);
                EditorGUILayout.EndScrollView();
            }
        }


        private void DrawSimulation(MarkovSimulationTwoDim<byte> sim2dim, MarkovSimulation2D sim)
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
            {
                defaultStateScrollPosition = GUILayout.BeginScrollView(defaultStateScrollPosition,
                    GUILayout.MaxHeight(500), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
                new StampDrawer().Draw(sim2dim.DefaultState, sim);
                EditorGUILayout.EndScrollView();
            }

            new PlayableListDrawer().Draw(sim2dim.Playables, sim);
        }


        private void Save()
        {
            MarkovSimulation2D sim = (MarkovSimulation2D)target;
            sim.SerializedSimulation = SimulationSerializer.SerializeSim(sim.Simulation);
            EditorUtility.SetDirty(sim);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Load()
        {
            MarkovSimulation2D sim = (MarkovSimulation2D)target;
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
            MarkovSimulation2D sim = (MarkovSimulation2D)target;
            sim.Simulation.Play(sim.Simulation, seed);
            sim.Visualize(_sceneContext.rootGameObject);
        }
    }
}