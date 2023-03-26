using MarkovTest.ThreeDimension.Rules;
using UnityEditor;
using UnityEngine;

namespace Editor._3D.EditorElementDrawers
{
    public class RotationSettingsDrawer : IEditorElementDrawer<RotationSettingsFlags, MarkovSimulationDrawer3D>
    {
        public RotationSettingsFlags Draw(RotationSettingsFlags elem, MarkovSimulationDrawer3D sim)
        {
            RotationSettingsFlags resultFlag = RotationSettingsFlags.None;
            EditorGUILayout.BeginVertical();
            //  if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.Rotate), "Rotate"))
            //      resultFlag = resultFlag | RotationSettingsFlags.Rotate;
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipX), "FlipX"))
                resultFlag = resultFlag | RotationSettingsFlags.FlipX;
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipY), "FlipY"))
                resultFlag = resultFlag | RotationSettingsFlags.FlipY;
            if (GUILayout.Toggle(elem.HasFlag(RotationSettingsFlags.FlipZ), "FlipZ"))
                resultFlag = resultFlag | RotationSettingsFlags.FlipZ;
            EditorGUILayout.EndVertical();
            return resultFlag;
        }
    }
}