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
        private Vector2 _serializedScrollPosition;
        private Vector2 _defaultStateScrollPosition;
        private bool _isShowDefaultState;
        private bool _isShowSerialized;

        private static readonly SceneContext SceneContext = new SceneContext();

        private int _defaultStateZIndexCoord;
        private int _seed;

        private readonly PlayableListDrawer<MarkovSimulation<byte>> _listDrawer =
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

            _serializedScrollPosition =
                GUILayout.BeginScrollView(_serializedScrollPosition, GUILayout.MaxHeight(500));
            EditorGUILayout.TextArea(sim.SerializedSimulation);
            EditorGUILayout.EndScrollView();
        }


        private void DrawSimulation(MarkovSimulation<byte> simulation, MarkovSimulationDrawer3D drawer)
        {
            EditorGUILayout.BeginHorizontal();
            _seed = EditorGUILayout.IntField("Seed", _seed);
            if (GUILayout.Button("Random"))
                _seed = Random.Range(0, int.MaxValue);
            EditorGUILayout.EndHorizontal();
            if (simulation.DefaultState != default)
                new ResizableDrawer().Draw(simulation, drawer);

            _isShowDefaultState = EditorGUILayout.Foldout(_isShowDefaultState, "Initial state");
            if (_isShowDefaultState)
            {
                if (GUILayout.Button("Visualize"))
                {
                    if (!SceneContext.IsActive())
                        SceneContext.Enter();
                    MatrixVisualizer3D.Visualize((x,y,z)=>drawer.MarkovSimulation.DefaultState[x,y,z], drawer.MarkovSimulation.Size, drawer.ColorPaletteLink,SceneContext.rootGameObject);
                }
                Drawer.Draw(simulation.DefaultState, drawer);
            }

            _listDrawer.Draw(simulation.Playables, drawer);
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
            SceneContext.Exit();
        }

        private void Run()
        {
            if (!SceneContext.IsActive())
                SceneContext.Enter();
            MarkovSimulationDrawer3D sim = (MarkovSimulationDrawer3D)target;
            sim.Simulation.Play(_seed);
            
            MatrixVisualizer3D.Visualize(
                (x, y, z) => sim.MarkovSimulation[x, y, z],
                sim.MarkovSimulation.Size,
                sim.ColorPaletteLink,
                SceneContext.rootGameObject);
        }
    }
}