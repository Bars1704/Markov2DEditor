using Markov.MarkovTest.TwoDimension.Rules;
using MarkovEditor._2D;
using UnityEditor;
using UnityEngine;

namespace Editor._2D.EditorElementDrawers
{
    public class AllRuleDrawer : IEditorElementDrawer<AllRule<byte>, MarkovSimulationDrawer2D>
    {
        private const float INTEND_SPACE = 15;

        public AllRule<byte> Draw(AllRule<byte> elem, MarkovSimulationDrawer2D sim)
        {
            EditorGUILayout.LabelField("All Rule");
            new ResizableDrawer().Draw(elem, sim);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE, false);
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