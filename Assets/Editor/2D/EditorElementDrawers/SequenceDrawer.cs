using System;
using MarkovEditor;
using MarkovTest;
using MarkovTest.Sequences;
using MarkovTest.TwoDimension;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class SequenceDrawer :
        IEditorElementDrawer<SelectRandomSequence<byte, MarkovSimulation<byte>>, IMarkovSimulationDrawer>,
        IEditorElementDrawer<MarkovSequence<byte, MarkovSimulation<byte>>, IMarkovSimulationDrawer>,
        IEditorElementDrawer<CycleSequence<byte, MarkovSimulation<byte>>, IMarkovSimulationDrawer>
    {
        public SelectRandomSequence<byte, MarkovSimulation<byte>> Draw(
            SelectRandomSequence<byte, MarkovSimulation<byte>> elem,
            IMarkovSimulationDrawer sim)
        {
            EditorGUILayout.LabelField("Random");
            new PlayableListDrawer<MarkovSimulation<byte>>().Draw(elem.Playables, sim);
            return elem;
        }

        public MarkovSequence<byte, MarkovSimulation<byte>> Draw(MarkovSequence<byte, MarkovSimulation<byte>> elem,
            IMarkovSimulationDrawer sim)
        {
            EditorGUILayout.LabelField("Markov");
            new PlayableListDrawer<MarkovSimulation<byte>>().Draw(elem.Playables, sim);
            return elem;
        }

        public CycleSequence<byte, MarkovSimulation<byte>> Draw(CycleSequence<byte, MarkovSimulation<byte>> elem, IMarkovSimulationDrawer sim)
        {
            var style = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft
            };

            elem.Cycles = Math.Max(EditorGUILayout.IntField("Cycle", elem.Cycles, style), 1);
            new PlayableListDrawer<MarkovSimulation<byte>>().Draw(elem.Playables, sim);
            return elem;
        }
    }
}