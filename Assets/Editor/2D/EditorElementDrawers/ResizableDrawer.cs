using MarkovTest.TwoDimension;
using UnityEditor;

namespace Editor.EditorElementDrawers
{
    public class ResizableDrawer: IEditorElementDrawer<IResizable>
    {
        public IResizable Draw(IResizable elem, MarkovSimulation2D sim)
        {
            EditorGUILayout.BeginHorizontal();
            var size = elem.Size;
            var newSize = EditorGUILayout.Vector2IntField("Size", new UnityEngine.Vector2Int(size.X, size.Y));

            if (newSize.x != size.X || newSize.y != size.Y)
                elem.Resize(new Vector2Int(newSize.x, newSize.y));

            EditorGUILayout.EndHorizontal();
            return elem;
        }
    }
}