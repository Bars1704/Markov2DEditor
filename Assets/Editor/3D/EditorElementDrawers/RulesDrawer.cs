using Markov.MarkovTest.ThreeDimension.Rules;
using MarkovEditor._3D;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class RulesDrawer: IEditorElementDrawer<AllRule<byte>, MarkovSimulationDrawer3D>,IEditorElementDrawer<RandomRule<byte>, MarkovSimulationDrawer3D>
    {
        private const float INTEND_SPACE = 15;

        public AllRule<byte> Draw(AllRule<byte> elem, MarkovSimulationDrawer3D sim)
        {
            EditorGUILayout.LabelField("All rule");
            new ResizableDrawer().Draw(elem, sim);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE , false);
            elem.MainPattern.RotationSettings = new RotationSettingsDrawer().Draw(elem.MainPattern.RotationSettings, sim);
            EditorGUILayout.EndHorizontal();
            
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
            EditorGUILayout.LabelField("Random rule");
            new ResizableDrawer().Draw(elem, sim);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE , false);
            elem.MainPattern.RotationSettings = new RotationSettingsDrawer().Draw(elem.MainPattern.RotationSettings, sim);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(EditorGUI.indentLevel * INTEND_SPACE);
            Drawer.Draw(elem.MainPattern, sim);
            GUILayout.Label("->");
            Drawer.Draw(elem.Stamp, sim);

            EditorGUILayout.EndHorizontal();
            return elem;
        }
    }
}