using System;
using System.Collections;
using System.Collections.Generic;
using MarkovTest;
using MarkovTest.Serialization;
using MarkovTest.TwoDimension;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MarkovSimulation))]
public class MarkovSimEditor : Editor
{
    private static SceneContext _sceneContext = new SceneContext();
    Vector2 scrollPosition;

    public override void OnInspectorGUI()
    {
        var sim = (MarkovSimulation)target;

        DrawPalette(sim);

        DrawDefaultInspector();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
        GUILayout.TextArea(sim.SerializedSimulation);
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
            Save();

        if (GUILayout.Button("Load"))
            Load();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Exit"))
            RunOneStep();

        if (GUILayout.Button("Run"))
            Run();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Init"))
        {
            sim.Simulation = SimulationFabric.Labirynth();
            Save();
        }
    }

    private static void DrawPalette(MarkovSimulation sim)
    {
        if (sim.ColorPaletteLink == default)
        {
            EditorGUILayout.HelpBox("Palette is not set!", MessageType.Error);
            return;
        }

        var buttonStyle = new GUIStyle();
        buttonStyle.normal.background = Texture2D.whiteTexture;
        buttonStyle.margin = new RectOffset(5, 5, 0, 0);

        GUILayout.Label("Palette");
        GUILayout.BeginHorizontal();
        var defaultColor = GUI.backgroundColor;
        for (var i = 0; i < sim.ColorPaletteLink.Length; i++)
        {
            GUI.backgroundColor = sim.ColorPaletteLink.GetColor(i);
            GUILayout.Button("", buttonStyle);
        }

        GUI.backgroundColor = defaultColor;

        GUILayout.EndHorizontal();
    }

    private void Save()
    {
        MarkovSimulation sim = (MarkovSimulation)target;
        sim.SerializedSimulation = SimulationSerializer.SerializeSim(sim.Simulation);
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
        
        sim.Visualize(_sceneContext.rootGameObject);

        void OnSimulationOnOnSimulationChanged(byte[,] bytes) => sim.Visualize(_sceneContext.rootGameObject);

        sim.Simulation.OnSimulationChanged += OnSimulationOnOnSimulationChanged;
        
        sim.Simulation.Play(sim.Simulation);
        
        sim.Simulation.OnSimulationChanged -= OnSimulationOnOnSimulationChanged;
    }
}