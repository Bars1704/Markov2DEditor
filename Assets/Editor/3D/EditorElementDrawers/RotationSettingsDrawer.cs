using Markov.MarkovTest.ThreeDimension.Rules;
using MarkovEditor._3D;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class RotationSettingsDrawer : IEditorElementDrawer<RotationSettingsFlags, MarkovSimulationDrawer3D>
    {
        public RotationSettingsFlags Draw(RotationSettingsFlags elem, MarkovSimulationDrawer3D sim)
        {
            RotationSettingsFlags resultFlag = RotationSettingsFlags.None;
            EditorGUILayout.BeginHorizontal(GUILayout.Width(350));
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipX), "FlipX"))
                resultFlag |= RotationSettingsFlags.FlipX;
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipY), "FlipY"))
                resultFlag |= RotationSettingsFlags.FlipY;
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipZ), "FlipZ"))
                resultFlag |= RotationSettingsFlags.FlipZ;
            EditorGUILayout.EndHorizontal();
            return resultFlag;
        }
    }
}