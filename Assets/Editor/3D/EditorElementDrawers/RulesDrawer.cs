using Markov.MarkovTest.ThreeDimension.Rules;
using MarkovEditor._3D;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class RulesDrawer: IEditorElementDrawer<AllRule<byte>, MarkovSimulationDrawer3D>,IEditorElementDrawer<RandomRule<byte>, MarkovSimulationDrawer3D>
    {
        private const float INTEND_SPACE = 20;

        public AllRule<byte> Draw(AllRule<byte> elem, MarkovSimulationDrawer3D sim)
        {
            new ResizableDrawer().Draw(elem, sim);
            EditorGUILayout.LabelField("All");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE);
            Drawer.Draw(elem.MainPattern, sim);
            GUILayout.Label("->");
            Drawer.Draw(elem.Stamp, sim);
            EditorGUILayout.EndHorizontal();
            return elem;
        }

        public RandomRule<byte> Draw(RandomRule<byte> elem, MarkovSimulationDrawer3D sim)
        {
            new ResizableDrawer().Draw(elem, sim);
            EditorGUILayout.LabelField("Random");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * 20);
            Drawer.Draw(elem.MainPattern, sim);
            GUILayout.Label("->");
            Drawer.Draw(elem.Stamp, sim);

            EditorGUILayout.EndHorizontal();
            return elem;
        }
    }
}