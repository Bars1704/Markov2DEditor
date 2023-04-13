using System.Collections.Generic;
using Editor._3D.EditorElementDrawers;
using Markov.MarkovTest.Serialization;
using Markov.MarkovTest.ThreeDimension;
using MarkovEditor._3D;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable AccessToModifiedClosure


namespace Editor._3D
{
    [CustomEditor(typeof(MarkovSimulationDrawer3D))]
    public class MarkovSimulation3DEditor : UnityEditor.Editor
    {
        private static readonly SceneContext _sceneContext = new SceneContext();
        private Vector2 serializedScrollPosition;
        private Vector2 defaultStateScrollPosition;
        private bool _isShowDefaultState;
        private bool _isShowSerialized;


        private int defaultStateZIndexCoord;
        private int seed;
        private readonly PlayableListDrawer<MarkovSimulation<byte>> listDrawer =
            new PlayableListDrawer<MarkovSimulation<byte>>();
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

            var sim = (MarkovSimulationDrawer3D)target;

            if (sim.ColorPaletteLink == default)
            {
                EditorGUILayout.HelpBox("Palette is not set!", MessageType.Error);
            }
            else
                new PaletteDrawer().Draw(sim.ColorPaletteLink, sim);

            DrawSimulation(sim.MarkovSimulation, sim);

            DrawSerializableField(sim);

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
            }
        }

        private void DrawSerializableField(MarkovSimulationDrawer3D sim)
        {
            _isShowSerialized = EditorGUILayout.Foldout(_isShowSerialized, "Serialized");
            if (!_isShowSerialized) return;

            serializedScrollPosition =
                GUILayout.BeginScrollView(serializedScrollPosition, GUILayout.MaxHeight(500));
            EditorGUILayout.TextArea(sim.SerializedSimulation);
            EditorGUILayout.EndScrollView();
        }


        private void DrawSimulation(MarkovSimulation<byte> simulation, MarkovSimulationDrawer3D drawer)
        {
            EditorGUILayout.BeginHorizontal();
            seed = EditorGUILayout.IntField("Seed", seed);
            if (GUILayout.Button("Random"))
                seed = Random.Range(0, int.MaxValue);
            EditorGUILayout.EndHorizontal();
            if (simulation.DefaultState != default)
                new ResizableDrawer().Draw(simulation, drawer);

            _isShowDefaultState = EditorGUILayout.Foldout(_isShowDefaultState, "Initial state");
            if (_isShowDefaultState)
            {
                Drawer.Draw(simulation.DefaultState, drawer);
            }

            listDrawer.Draw(simulation.Playables, drawer);
        }


        private void Save()
        {
            var sim = (MarkovSimulationDrawer3D)target;
            sim.SerializedSimulation = SimulationSerializer.SerializeSim(sim.MarkovSimulation);
            EditorUtility.SetDirty(sim);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Load()
        {
            var sim = (MarkovSimulationDrawer3D)target;
            sim.Simulation = SimulationSerializer.DeserializeSim3D(sim.SerializedSimulation);
        }


        private static void RunOneStep()
        {
            _sceneContext.Exit();
        }

        private void Run()
        {
            if (!_sceneContext.IsActive())
                _sceneContext.Enter();
            MarkovSimulationDrawer3D sim = (MarkovSimulationDrawer3D)target;
            sim.Simulation.Play(seed);
            sim.Visualize(_sceneContext.rootGameObject);
        }
    }
}