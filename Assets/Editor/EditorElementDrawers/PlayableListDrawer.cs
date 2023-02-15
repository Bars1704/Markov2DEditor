using System;
using System.Collections.Generic;
using System.Linq;
using MarkovTest.TwoDimension.Rules;
using MarkovTest.TwoDimension.Sequences;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class PlayableListDrawer : IEditorElementDrawer<List<ISequencePlayable<byte>>>
    {
        private static List<Type> PlayableTypes = new List<Type>();
        private static GUIStyle _boxStyle;

        static PlayableListDrawer()
        {
            PlayableTypes = GetAllPlayables().ToList();
        }

        public List<ISequencePlayable<byte>> Draw(List<ISequencePlayable<byte>> elem, MarkovSimulation sim)
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
                DrawPlayable(elem[i], sim, () => elem.Remove(elem[i]));
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();
            if (GUILayout.Button("+"))
                DrawAddNewElemDropList(elem);
            EditorGUILayout.EndHorizontal();
            return elem;
        }


        private void DrawAddNewElemDropList(List<ISequencePlayable<byte>> playables)
        {
            GenericMenu menu = new GenericMenu();

            var TypesParents = PlayableTypes.Where(x => x.BaseType.IsGenericType).ToList();

            var seekableRule = typeof(RuleBase<byte>).GetGenericTypeDefinition();
            var rules = TypesParents.Where(x => seekableRule.IsAssignableFrom(x.BaseType?.GetGenericTypeDefinition()))
                .ToList();
            var seekableSeq = typeof(SequenceBase<byte>).GetGenericTypeDefinition();
            var sequences = TypesParents
                .Where(x => seekableSeq.IsAssignableFrom(x.BaseType?.GetGenericTypeDefinition())).ToList();
            var other = PlayableTypes.Except(rules).Except(sequences);

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

        private void DrawPlayable(ISequencePlayable<byte> playable, MarkovSimulation sim, Action OnDeleteButtonClicked)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            Drawer.Draw(playable, sim);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
                OnDeleteButtonClicked?.Invoke();

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void AddMenuItem(GenericMenu menu, string menuPath, Action action = default)
        {
            menu.AddItem(new GUIContent(menuPath), false, () => action?.Invoke());
        }

        private string GetName(Type t) => t.Name.Split('`').First();

        private Type CreateGenericType(Type t) => t.MakeGenericType(typeof(byte));

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

            var seekedType = typeof(ISequencePlayable<byte>).GetGenericTypeDefinition();


            var result = allTypes.Where(type => type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(x => seekedType.IsAssignableFrom(x))
            );
            return result;
        }
    }
}