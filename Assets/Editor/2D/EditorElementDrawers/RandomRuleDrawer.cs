using MarkovEditor;
using MarkovEditor._2D;
using MarkovTest.TwoDimension.Rules;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorElementDrawers
{
    public class RandomRuleDrawer : IEditorElementDrawer<RandomRule<byte>, MarkovSimulationDrawer2D>
    {
        public RandomRule<byte> Draw(RandomRule<byte> elem, MarkovSimulationDrawer2D sim)
        {
            new ResizableDrawer().Draw(elem, sim);
            EditorGUILayout.LabelField("Random");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * 20);
            new Pattern2DDrawer().Draw(elem.MainPattern, sim);
            GUILayout.Label("->");
            new StampDrawer().Draw(elem.Stamp, sim);

            EditorGUILayout.EndHorizontal();
            return elem;
        }
    }
}