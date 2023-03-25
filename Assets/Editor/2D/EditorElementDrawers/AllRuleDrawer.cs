using MarkovEditor;
using MarkovEditor._2D;
using MarkovTest;
using MarkovTest.TwoDimension.Rules;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class AllRuleDrawer : IEditorElementDrawer<AllRule<byte>, MarkovSimulationDrawer2D>
    {
        private const float INTEND_SPACE = 20;

        public AllRule<byte> Draw(AllRule<byte> elem, MarkovSimulationDrawer2D sim)
        {
            new ResizableDrawer().Draw(elem, sim);
            EditorGUILayout.LabelField("All");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE);
            new Pattern2DDrawer().Draw(elem.MainPattern, sim);
            GUILayout.Label("->");
            new StampDrawer().Draw(elem.Stamp, sim);
            EditorGUILayout.EndHorizontal();
            return elem;
        }
    }
}