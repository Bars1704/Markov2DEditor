using Markov.MarkovTest.TwoDimension.Rules;
using MarkovEditor._2D;
using UnityEditor;
using UnityEngine;

namespace Editor._2D.EditorElementDrawers
{
    public class RotationSettingsDrawer : IEditorElementDrawer<RotationSettingsFlags, MarkovSimulationDrawer2D>
    {
        public RotationSettingsFlags Draw(RotationSettingsFlags elem, MarkovSimulationDrawer2D sim)
        {
            RotationSettingsFlags resultFlag = RotationSettingsFlags.None;
            EditorGUILayout.BeginHorizontal(GUILayout.Width(350));
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.Rotate), "Rotate"))
                resultFlag |= RotationSettingsFlags.Rotate;
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipX), "FlipX"))
                resultFlag |= RotationSettingsFlags.FlipX;
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipY), "FlipY"))
                resultFlag |= RotationSettingsFlags.FlipY;
            EditorGUILayout.EndHorizontal();
            return resultFlag;
        }
    }
}