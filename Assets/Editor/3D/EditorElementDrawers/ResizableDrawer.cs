using UnityEditor;
using UnityEngine;
using IResizable = MarkovTest.ThreeDimension.IResizable;
using Vector3Int = MarkovTest.ThreeDimension.Vector3Int;

namespace Editor._3D.EditorElementDrawers
{
    public class ResizableDrawer : IEditorElementDrawer<IResizable, MarkovSimulationDrawer3D>
    {
        public IResizable Draw(IResizable elem, MarkovSimulationDrawer3D sim)
        {
            EditorGUILayout.BeginHorizontal();
            var size = elem.Size;
            var newSize = EditorGUILayout.Vector3IntField("Size", new UnityEngine.Vector3Int(size.X, size.Y, size.Z));

            newSize.x = Mathf.Max(newSize.x, 0);
            newSize.y = Mathf.Max(newSize.y, 0);
            newSize.z = Mathf.Max(newSize.z, 0);

            if (newSize.x != size.X || newSize.y != size.Y || newSize.z != size.Z)
                elem.Resize(new Vector3Int(newSize.x, newSize.y, newSize.z));

            EditorGUILayout.EndHorizontal();
            return elem;
        }
    }
}