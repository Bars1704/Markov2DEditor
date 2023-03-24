using System;
using System.Collections.Generic;
using MarkovTest.TwoDimension.Sequences;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class SequenceDrawer : IEditorElementDrawer<SelectRandomSequence<byte>>,
        IEditorElementDrawer<MarkovSequence<byte>>, IEditorElementDrawer<CycleSequence<byte>>
    {
        public SelectRandomSequence<byte> Draw(SelectRandomSequence<byte> elem, MarkovSimulation2D sim)
        {
            EditorGUILayout.LabelField("Random");
            new PlayableListDrawer().Draw(elem.Playables, sim);
            return elem;
        }

        public MarkovSequence<byte> Draw(MarkovSequence<byte> elem, MarkovSimulation2D sim)
        {
            EditorGUILayout.LabelField("Markov");
            new PlayableListDrawer().Draw(elem.Playables, sim);
            return elem;
        }

        public CycleSequence<byte> Draw(CycleSequence<byte> elem, MarkovSimulation2D sim)
        {
            var style = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft
            };

            elem.Cycles = Math.Max(EditorGUILayout.IntField("Cycle", elem.Cycles, style), 1);
            new PlayableListDrawer().Draw(elem.Playables, sim);
            return elem;
        }
    }
}