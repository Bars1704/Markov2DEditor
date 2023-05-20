using Markov.MarkovTest.TwoDimension.Rules;
using MarkovEditor._2D;
using UnityEditor;
using UnityEngine;

namespace Editor._2D.EditorElementDrawers
{
    public class RandomRuleDrawer : IEditorElementDrawer<RandomRule<byte>, MarkovSimulationDrawer2D>
    {
        private const int INTEND_SPACE = 16;
        public RandomRule<byte> Draw(RandomRule<byte> elem, MarkovSimulationDrawer2D sim)
        {
            EditorGUILayout.LabelField("Random rule");
            new ResizableDrawer().Draw(elem, sim);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE , false);
            elem.MainPattern.RotationSettings = new RotationSettingsDrawer().Draw(elem.MainPattern.RotationSettings, sim);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE, false);
            new PatternDrawer().Draw(elem.MainPattern, sim);
            GUILayout.Label("->");
            new StampDrawer().Draw(elem.Stamp, sim);
            EditorGUILayout.EndHorizontal();

            return elem;
        }
    }
}