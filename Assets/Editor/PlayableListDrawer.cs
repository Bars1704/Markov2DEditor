using System;
using System.Collections.Generic;
using System.Linq;
using Editor._2D;
using MarkovEditor;
using MarkovTest;
using MarkovTest.Sequences;
using MarkovTest.TwoDimension.Rules;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class PlayableListDrawer<T> : IEditorElementDrawer<List<ISequencePlayable<byte, T>>,
        IMarkovSimulationDrawer> where T : IMarkovSimulation<byte>
    {
        private static List<Type> PlayableTypes = new List<Type>();
        private static GUIStyle _boxStyle;

        static PlayableListDrawer()
        {
            PlayableTypes = GetAllPlayables().ToList();
        }

        public List<ISequencePlayable<byte, T>> Draw(
            List<ISequencePlayable<byte, T>> elem, IMarkovSimulationDrawer sim)
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
            EditorGUILayout.LabelField(GetName(elem.GetType()));
            for (var i = 0; i < elem.Count; i++)
            {
                void OnDeleteButtonClicked() => elem.Remove(elem[i]);

                var index = i;

                void MoveUp()
                {
                    var newIndex = index <= 0 ? index : index - 1;
                    (elem[index], elem[newIndex]) = (elem[newIndex], elem[index]);
                }

                void MoveDown()
                {
                    var newIndex = index >= elem.Count ? index : index + 1;
                    (elem[i], elem[newIndex]) = (elem[newIndex], elem[i]);
                }

                DrawPlayable(elem[i], sim, OnDeleteButtonClicked, MoveUp, MoveDown);
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();
            if (GUILayout.Button("+"))
                DrawAddNewElemDropList(elem, sim);
            EditorGUILayout.EndHorizontal();
            return elem;
        }


        private void DrawAddNewElemDropList(List<ISequencePlayable<byte, T>> playables, IMarkovSimulationDrawer sim)
        {
            GenericMenu menu = new GenericMenu();

            var TypesParents = PlayableTypes.Where(x => x.BaseType.IsGenericType).ToList();

            var seekableRule = typeof(RuleBase<byte>).GetGenericTypeDefinition();
            var rules = TypesParents.Where(x => seekableRule.IsAssignableFrom(x.BaseType?.GetGenericTypeDefinition()))
                .ToList();
            var seekableSeq = typeof(SequenceBase<byte, IMarkovSimulation<byte>>).GetGenericTypeDefinition();
            var sequences = TypesParents
                .Where(x => seekableSeq.IsAssignableFrom(x.BaseType?.GetGenericTypeDefinition())).ToList();
            var other = PlayableTypes.Except(rules).Except(sequences);

            foreach (var rule in rules)
                AddMenuItem(menu, $"Rule/{GetName(rule)}",
                    () => playables.Add(
                        Activator.CreateInstance(CreateGenericType(rule)) as
                            ISequencePlayable<byte, T>));

            menu.AddSeparator("Sequences/");

            foreach (var seq in sequences)
                AddMenuItem(menu, $"Sequences/{GetName(seq)}",
                    () => playables.Add(
                        Activator.CreateInstance(CreateGenericType(seq)) as
                            ISequencePlayable<byte, T>));

            menu.AddSeparator("3D rules/");
            foreach (var oth in other)
                AddMenuItem(menu, $"3D rules/{GetName(oth)}",
                    () => playables.Add(
                        Activator.CreateInstance(CreateGenericType(oth)) as
                            ISequencePlayable<byte, T>));

            menu.ShowAsContext();
        }

        private void DrawPlayable(ISequencePlayable<byte, T> playable, IMarkovSimulationDrawer sim,
            Action OnDeleteButtonClicked,
            Action moveUp, Action moveDown)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            Drawer.Draw(playable, sim);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
                OnDeleteButtonClicked?.Invoke();
            EditorGUILayout.Separator();
            if (GUILayout.Button("^\n|", GUILayout.Width(20), GUILayout.Height(30)))
                moveUp?.Invoke();
            if (GUILayout.Button("|\nv", GUILayout.Width(20), GUILayout.Height(30)))
                moveDown?.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void AddMenuItem(GenericMenu menu, string menuPath, Action action = default)
        {
            menu.AddItem(new GUIContent(menuPath), false, () => action?.Invoke());
        }

        private string GetName(Type t) => t.Name.Split('`').First();

        private Type CreateGenericType(Type t) => t.GetGenericArguments().Length == 1
            ? t.MakeGenericType(typeof(byte))
            : t.MakeGenericType(typeof(byte), typeof(T));

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

        private static IEnumerable<Type> GetAllPlayables()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => !t.IsInterface && !t.IsAbstract);

            var seekedType = typeof(ISequencePlayable<byte, IMarkovSimulation<byte>>).GetGenericTypeDefinition();


            var result = allTypes.Where(type => type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(x => seekedType.IsAssignableFrom(x))
            );
            return result;
        }
    }
}