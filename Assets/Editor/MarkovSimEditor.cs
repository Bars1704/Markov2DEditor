using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarkovTest;
using MarkovTest.Serialization;
using MarkovTest.TwoDimension;
using MarkovTest.TwoDimension.Patterns;
using MarkovTest.TwoDimension.Rules;
using MarkovTest.TwoDimension.Sequences;
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

        DrawSim(sim.Simulation, sim);
    }


    private static void DrawSim(MarkovSimulationTwoDim<byte> sim2dim, MarkovSimulation sim)
    {
        if (sim2dim.Seed != null)
            EditorGUILayout.IntField("Seed", sim2dim.Seed.Value);
        else
            GUILayout.Label("Random Seed");
        
        EditorGUILayout.Vector2Field("Size", new Vector2(sim2dim.Size.X, sim2dim.Size.Y));
        DrawPlayablesList(sim2dim.Playables, sim);
    }

    private static void DrawPattern(Pattern<Byte> pattern, MarkovSimulation sim)
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for (var x = 0; x < pattern.PatternForm.GetLength(0); x++)
        {
            GUILayout.BeginHorizontal();
            for (var y = 0; y < pattern.PatternForm.GetLength(1); y++)
            {
                DrawPatternElement(pattern.PatternForm[x, y], sim);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.Space(100);
    }


    private static void DrawRotationSettings(RotationSettingsFlags settings)
    {
        GUILayout.BeginVertical();
        GUILayout.Toggle(settings.HasFlag(RotationSettingsFlags.Rotate), "Rotate");
        GUILayout.Toggle(settings.HasFlag(RotationSettingsFlags.FlipX), "FlipX");
        GUILayout.Toggle(settings.HasFlag(RotationSettingsFlags.FlipY), "FlipY");
        GUILayout.EndVertical();
    }

    private static void DrawStamp(byte[,] stamp, MarkovSimulation sim)
    {
        GUILayout.BeginVertical();
        for (var x = 0; x < stamp.GetLength(0); x++)
        {
            GUILayout.BeginHorizontal();
            for (var y = 0; y < stamp.GetLength(1); y++)
            {
                DrawStampElement(stamp[x, y], sim);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        GUILayout.Space(100);
    }


    private static void DrawSequence(SequenceBase<byte> sequence, MarkovSimulation sim)
    {
        if (sequence is CycleSequence<byte> cycle)
            DrawCycleSequence(cycle, sim);
        else if (sequence is MarkovSequence<byte> markov)
            DrawMarkovSequence(markov, sim);
    }

    private static void DrawCycleSequence(CycleSequence<byte> cycleSequence, MarkovSimulation sim)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Cycle");
        EditorGUILayout.FloatField(cycleSequence.Cycles);
        GUILayout.EndHorizontal();
        DrawPlayablesList(cycleSequence.Playables, sim);
    }

    private static void DrawMarkovSequence(MarkovSequence<byte> markovSequence, MarkovSimulation sim)
    {
        GUILayout.Label("Markov");
        DrawPlayablesList(markovSequence.Playables, sim);
    }


    private static void DrawPlayablesList(List<ISequencePlayable<byte>> playables, MarkovSimulation sim)
    {
        GUILayout.BeginVertical();
        playables.ForEach(x =>
        {
            DrawPlayable(x, sim);
            GUILayout.Space(10);
        });
        GUILayout.EndVertical();
    }

    private static void DrawPlayable(ISequencePlayable<byte> playable, MarkovSimulation sim)
    {
        if (playable is RuleBase<byte> rule)
            DrawRule(rule, sim);
        else if (playable is SequenceBase<byte> sequence)
            DrawSequence(sequence, sim);
    }


    private static void DrawRule(RuleBase<byte> rule, MarkovSimulation sim)
    {
        if (rule is RandomRule<byte> randRule)
            DrawRandomRule(randRule, sim);
        else if (rule is AllRule<byte> allRule)
            DrawAllRule(allRule, sim);
    }

    private static void DrawAllRule(AllRule<byte> rule, MarkovSimulation sim)
    {
        GUILayout.Label("All");
        GUILayout.BeginHorizontal();

        DrawPattern(rule.MainPattern, sim);
        DrawRotationSettings(rule.RotationSettings);
        GUILayout.Label("->");
        DrawStamp(rule.Stamp, sim);

        GUILayout.EndHorizontal();
    }

    private static void DrawRandomRule(RandomRule<byte> rule, MarkovSimulation sim)
    {
        GUILayout.Label("Random");
        GUILayout.BeginHorizontal();

        DrawPattern(rule.MainPattern, sim);
        DrawRotationSettings(rule.RotationSettings);
        GUILayout.Label("->");
        DrawStamp(rule.Stamp, sim);

        GUILayout.EndHorizontal();
    }

    private static void DrawStampElement(byte stampElement, MarkovSimulation sim)
    {
        var style = new GUIStyle();
        style.normal.background = Texture2D.whiteTexture;
        style.margin = new RectOffset(5, 5, 0, 0);

        var defaultColor = GUI.backgroundColor;


        GUI.backgroundColor = sim.ColorPaletteLink.GetColor(stampElement);
        GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20));
        GUI.backgroundColor = defaultColor;
    }

    private static void DrawPatternElement(IEquatable<byte> patternElement, MarkovSimulation sim)
    {
        var style = new GUIStyle();
        style.normal.background = Texture2D.whiteTexture;
        style.margin = new RectOffset(5, 5, 0, 0);

        var defaultColor = GUI.backgroundColor;

        if (!(patternElement is byte))
        {
            Debug.LogError($"{patternElement} doesnt has drawer!");
            return;
        }

        GUI.backgroundColor = sim.ColorPaletteLink.GetColor(((byte)patternElement));
        GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20));
        GUI.backgroundColor = defaultColor;
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