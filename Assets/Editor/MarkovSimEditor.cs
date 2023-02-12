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
    private byte _currentColorIndex;

    private const float INTEND_SPACE = 20;

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
        var sim = (MarkovSimulation)target;

        DrawPalette(sim);

        DrawDefaultInspector();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
        EditorGUILayout.TextArea(sim.SerializedSimulation);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
            Save();

        if (GUILayout.Button("Load"))
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

        var i = EditorGUI.indentLevel;
        DrawSim(sim.Simulation, sim);
    }


    private void DrawSim(MarkovSimulationTwoDim<byte> sim2dim, MarkovSimulation sim)
    {
        if (sim2dim.Seed != null)
            EditorGUILayout.IntField("Seed", sim2dim.Seed.Value);
        else
            EditorGUILayout.LabelField("Random Seed");
        EditorGUILayout.Vector2Field("Size", new Vector2(sim2dim.Size.X, sim2dim.Size.Y));
        DrawStamp(sim2dim.DefaultState, sim);
        DrawPlayablesList(sim2dim.Playables, "", sim);
    }

    private void DrawPattern(Pattern<Byte> pattern, MarkovSimulation sim)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        for (var x = 0; x < pattern.PatternForm.GetLength(0); x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var y = 0; y < pattern.PatternForm.GetLength(1); y++)
            {
                DrawPatternElement(pattern.PatternForm[x, y], sim, () => pattern.PatternForm[x,y] = _currentColorIndex);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }


    private static RotationSettingsFlags DrawRotationSettings(RotationSettingsFlags settings)
    {
        RotationSettingsFlags resultFlag = RotationSettingsFlags.None;
        EditorGUILayout.BeginVertical();
        if (GUILayout.Toggle(settings.HasFlag(RotationSettingsFlags.Rotate), "Rotate"))
            resultFlag = resultFlag | RotationSettingsFlags.Rotate;
        if (GUILayout.Toggle(settings.HasFlag(RotationSettingsFlags.FlipX), "FlipX"))
            resultFlag = resultFlag | RotationSettingsFlags.FlipX;
        if (GUILayout.Toggle(settings.HasFlag(RotationSettingsFlags.FlipY), "FlipY"))
            resultFlag = resultFlag | RotationSettingsFlags.FlipY;
        EditorGUILayout.EndVertical();
        return resultFlag;
    }

    private void DrawStamp(byte[,] stamp, MarkovSimulation sim)
    {
        EditorGUILayout.BeginVertical();
        for (var x = 0; x < stamp.GetLength(0); x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var y = 0; y < stamp.GetLength(1); y++)
            {
                DrawStampElement(stamp[x, y], sim, () => stamp[x, y] = _currentColorIndex);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }

        EditorGUILayout.EndVertical();
    }


    private void DrawSequence(SequenceBase<byte> sequence, MarkovSimulation sim)
    {
        if (sequence is CycleSequence<byte> cycle)
            DrawCycleSequence(cycle, sim);
        else if (sequence is MarkovSequence<byte> markov)
            DrawMarkovSequence(markov, sim);
    }

    private void DrawCycleSequence(CycleSequence<byte> cycleSequence, MarkovSimulation sim)
    {
        EditorGUILayout.BeginHorizontal();
        cycleSequence.Cycles = EditorGUILayout.IntField(cycleSequence.Cycles);
        EditorGUILayout.EndHorizontal();
        DrawPlayablesList(cycleSequence.Playables, "Cycle", sim);
    }

    private void DrawMarkovSequence(MarkovSequence<byte> markovSequence, MarkovSimulation sim)
    {
        DrawPlayablesList(markovSequence.Playables, "Markov", sim);
    }


    private void DrawPlayablesList(List<ISequencePlayable<byte>> playables, string labelName,
        MarkovSimulation sim)
    {
        EditorGUILayout.LabelField(labelName);
        EditorGUILayout.BeginVertical();
        playables.ForEach(x =>
        {
            DrawPlayable(x, sim);
            EditorGUILayout.Separator();
        });
        EditorGUILayout.EndVertical();
    }

    private void DrawPlayable(ISequencePlayable<byte> playable, MarkovSimulation sim)
    {
        EditorGUI.indentLevel++;
        if (playable is RuleBase<byte> rule)
            DrawRule(rule, sim);
        else if (playable is SequenceBase<byte> sequence)
            DrawSequence(sequence, sim);
        EditorGUI.indentLevel--;
    }


    private void DrawRule(RuleBase<byte> rule, MarkovSimulation sim)
    {
        if (rule is RandomRule<byte> randRule)
            DrawRandomRule(randRule, sim);
        else if (rule is AllRule<byte> allRule)
            DrawAllRule(allRule, sim);
    }

    private void DrawAllRule(AllRule<byte> rule, MarkovSimulation sim)
    {
        EditorGUILayout.LabelField("All");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE);
        DrawPattern(rule.MainPattern, sim);
        rule.RotationSettings = DrawRotationSettings(rule.RotationSettings);
        GUILayout.Label("->");
        DrawStamp(rule.Stamp, sim);

        EditorGUILayout.EndHorizontal();
    }

    private void DrawRandomRule(RandomRule<byte> rule, MarkovSimulation sim)
    {
        EditorGUILayout.LabelField("Random");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE);
        DrawPattern(rule.MainPattern, sim);
        rule.RotationSettings = DrawRotationSettings(rule.RotationSettings);
        GUILayout.Label("->");
        DrawStamp(rule.Stamp, sim);

        EditorGUILayout.EndHorizontal();
    }

    private static void DrawStampElement(byte stampElement, MarkovSimulation sim, Action OnClicked)
    {
        var style = new GUIStyle();
        style.normal.background = Texture2D.whiteTexture;
        style.margin = new RectOffset(5, 5, 0, 0);

        var defaultColor = GUI.backgroundColor;


        GUI.backgroundColor = sim.ColorPaletteLink.GetColor(stampElement);
        if (GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20)))
            OnClicked?.Invoke();
        GUI.backgroundColor = defaultColor;
    }

    private static void DrawPatternElement(IEquatable<byte> patternElement, MarkovSimulation sim, Action OnClicked)
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
        if(GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20)))
            OnClicked?.Invoke();
        GUI.backgroundColor = defaultColor;
    }

    private void DrawPalette(MarkovSimulation sim)
    {
        if (sim.ColorPaletteLink == default)
        {
            EditorGUILayout.HelpBox("Palette is not set!", MessageType.Error);
            return;
        }

        var buttonStyle = new GUIStyle();
        buttonStyle.normal.background = Texture2D.whiteTexture;
        buttonStyle.margin = new RectOffset(5, 5, 0, 0);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Palette");
        var defaultColor = GUI.backgroundColor;
        GUI.backgroundColor = sim.ColorPaletteLink.GetColor(_currentColorIndex);
        GUILayout.Button("", buttonStyle);
        GUI.backgroundColor = defaultColor;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        defaultColor = GUI.backgroundColor;
        for (byte i = 0; i < sim.ColorPaletteLink.Length; i++)
        {
            GUI.backgroundColor = sim.ColorPaletteLink.GetColor(i);
            if (GUILayout.Button("", buttonStyle))
            {
                _currentColorIndex = i;
            }
        }

        GUI.backgroundColor = defaultColor;

        EditorGUILayout.EndHorizontal();
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

        sim.Visualize(_sceneContext.rootGameObject);

        void OnSimulationOnOnSimulationChanged(byte[,] bytes) => sim.Visualize(_sceneContext.rootGameObject);

        sim.Simulation.OnSimulationChanged += OnSimulationOnOnSimulationChanged;

        sim.Simulation.Play(sim.Simulation);

        sim.Simulation.OnSimulationChanged -= OnSimulationOnOnSimulationChanged;
    }
}