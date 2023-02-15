using System;
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
using Random = UnityEngine.Random;
using Vector2Int = MarkovTest.TwoDimension.Vector2Int;

// ReSharper disable AccessToModifiedClosure


namespace Editor
{
    [CustomEditor(typeof(MarkovSimulation))]
    public class MarkovSimEditor : UnityEditor.Editor
    {
        private static readonly SceneContext _sceneContext = new SceneContext();
        Vector2 scrollPosition;
        private const float INTEND_SPACE = 20;
        private List<Type> Types = new List<Type>();
        private static GUIStyle _boxStyle;
        private bool _isShowDefaultState;
        private bool _isShowSerialized;

        private int seed;

        private void Awake()
        {
            Load();
            Types = GetAllPlayables().ToList();
        }

        private void OnDestroy()
        {
            Save();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var sim = (MarkovSimulation)target;

            DrawPalette(sim);
            DrawSerializableFiel(sim);
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

        private void DrawSerializableFiel(MarkovSimulation sim)
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
                DrawResizable(sim2dim);

            _isShowDefaultState = EditorGUILayout.Foldout(_isShowDefaultState, "Initial state");
            if (_isShowDefaultState)
                DrawStamp(sim2dim.DefaultState, sim);

            DrawPlayablesList(sim2dim.Playables, "Simulation", sim);
        }


        void AddMenuItem(GenericMenu menu, string menuPath, Action action = default)
        {
            menu.AddItem(new GUIContent(menuPath), false, () => action?.Invoke());
        }

        private string GetName(Type t) => t.Name.Split('`').First();

        private Type CreateGenericType(Type t) => t.MakeGenericType(typeof(byte));

        private void DrawAddNewElemDropList(List<ISequencePlayable<byte>> playables)
        {
            GenericMenu menu = new GenericMenu();

            var TypesParents = Types.Where(x => x.BaseType.IsGenericType).ToList();

            var seekableRule = typeof(RuleBase<byte>).GetGenericTypeDefinition();
            var rules = TypesParents.Where(x => seekableRule.IsAssignableFrom(x.BaseType?.GetGenericTypeDefinition()))
                .ToList();
            var seekableSeq = typeof(SequenceBase<byte>).GetGenericTypeDefinition();
            var sequences = TypesParents
                .Where(x => seekableSeq.IsAssignableFrom(x.BaseType?.GetGenericTypeDefinition())).ToList();
            var other = Types.Except(rules).Except(sequences);

            foreach (var rule in rules)
                AddMenuItem(menu, $"Rule/{GetName(rule)}",
                    () => playables.Add(Activator.CreateInstance(CreateGenericType(rule)) as ISequencePlayable<byte>));

            menu.AddSeparator("Sequences/");

            foreach (var seq in sequences)
                AddMenuItem(menu, $"Sequences/{GetName(seq)}",
                    () => playables.Add(Activator.CreateInstance(CreateGenericType(seq)) as ISequencePlayable<byte>));

            menu.AddSeparator("Other/");
            foreach (var oth in other)
                AddMenuItem(menu, $"Other/{GetName(oth)}",
                    () => playables.Add(Activator.CreateInstance(CreateGenericType(oth)) as ISequencePlayable<byte>));

            menu.ShowAsContext();
        }

        private void DrawResizable(IResizable resizable)
        {
            EditorGUILayout.BeginHorizontal();
            var size = resizable.Size;
            var newSize = EditorGUILayout.Vector2IntField("Size", new UnityEngine.Vector2Int(size.X, size.Y));

            if (newSize.x != size.X || newSize.y != size.Y)
                resizable.Resize(new Vector2Int(newSize.x, newSize.y));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPattern(Pattern<Byte> pattern, MarkovSimulation sim)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            for (var y = 0; y < pattern.PatternForm.GetLength(1); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < pattern.PatternForm.GetLength(0); x++)
                    DrawPatternElement(pattern.PatternForm[x, y], sim,
                        () => pattern.PatternForm[x, y] = ColorPalette.CurrentColorIndex);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
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
            for (var y = 0; y < stamp.GetLength(1); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var x = 0; x < stamp.GetLength(0); x++)
                    DrawStampElement(stamp[x, y], sim, () => stamp[x, y] = ColorPalette.CurrentColorIndex);

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
            var style = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft
            };

            cycleSequence.Cycles = Math.Max(EditorGUILayout.IntField("Cycle", cycleSequence.Cycles, style), 1);
            DrawPlayablesList(cycleSequence.Playables, "", sim);
        }

        private void DrawMarkovSequence(MarkovSequence<byte> markovSequence, MarkovSimulation sim)
        {
            DrawPlayablesList(markovSequence.Playables, "Markov", sim);
        }


        private static Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void DrawPlayablesList(List<ISequencePlayable<byte>> playables, string labelName, MarkovSimulation sim)
        {
            _boxStyle ??= new GUIStyle(GUI.skin.box)
            {
                normal =
                {
                    background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.15f))
                }
            };

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(_boxStyle);
            EditorGUILayout.LabelField(labelName);
            for (var i = 0; i < playables.Count; i++)
            {
                DrawPlayable(playables[i], sim, () => playables.Remove(playables[i]));
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();
            if (GUILayout.Button("+"))
                DrawAddNewElemDropList(playables);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPlayable(ISequencePlayable<byte> playable, MarkovSimulation sim, Action OnDeleteButtonClicked)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            if (playable is RuleBase<byte> rule)
                DrawRule(rule, sim);
            else if (playable is SequenceBase<byte> sequence)
                DrawSequence(sequence, sim);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
                OnDeleteButtonClicked?.Invoke();

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }


        private static IEnumerable<Type> GetAllPlayables()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => !t.IsInterface && !t.IsAbstract);

            var seekedType = typeof(ISequencePlayable<byte>).GetGenericTypeDefinition();


            var result = allTypes.Where(type => type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(x => seekedType.IsAssignableFrom(x))
            );
            return result;
        }

        private void DrawRule(RuleBase<byte> rule, MarkovSimulation sim)
        {
            DrawResizable(rule);
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
            var style = new GUIStyle
            {
                normal =
                {
                    background = Texture2D.whiteTexture
                },
                margin = new RectOffset(5, 5, 0, 0)
            };

            var defaultColor = GUI.backgroundColor;


            GUI.backgroundColor = sim.ColorPaletteLink.GetColor(stampElement);
            if (GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20)))
                OnClicked?.Invoke();
            GUI.backgroundColor = defaultColor;
        }

        private static void DrawPatternElement(IEquatable<byte> patternElement, MarkovSimulation sim, Action OnClicked)
        {
            var style = new GUIStyle
            {
                normal =
                {
                    background = Texture2D.whiteTexture
                },
                margin = new RectOffset(5, 5, 0, 0)
            };

            var defaultColor = GUI.backgroundColor;

            patternElement ??= (byte)0;

            if (!(patternElement is byte b))
            {
                Debug.LogError($"{patternElement} doesnt has drawer!");
                return;
            }

            GUI.backgroundColor = sim.ColorPaletteLink.GetColor(b);
            if (GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20)))
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

            var buttonStyle = new GUIStyle
            {
                normal =
                {
                    background = Texture2D.whiteTexture
                },
                margin = new RectOffset(5, 5, 0, 0)
            };

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Palette");
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = sim.ColorPaletteLink.CurrentColor;
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
                    ColorPalette.CurrentColorIndex = i;
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
            sim.Simulation.Play(sim.Simulation, seed);
            sim.Visualize(_sceneContext.rootGameObject);
        }
    }
}