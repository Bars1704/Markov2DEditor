using Markov.MarkovTest.TwoDimension;
using MarkovEditor._2D;
using UnityEditor;
using UnityEngine;
using Vector2Int = Markov.MarkovTest.TwoDimension.Vector2Int;

namespace Editor._2D.EditorElementDrawers
{
    public class ResizableDrawer : IEditorElementDrawer<IResizable, MarkovSimulationDrawer2D>
    {
        public IResizable Draw(IResizable elem, MarkovSimulationDrawer2D sim)
        {
            EditorGUILayout.BeginHorizontal();
            var size = elem.Size;
            var newSize = EditorGUILayout.Vector2IntField("Size", new UnityEngine.Vector2Int(size.X, size.Y));
            newSize.x = Mathf.Max(newSize.x, 0);
            newSize.y = Mathf.Max(newSize.y, 0);
            
            if (newSize.x != size.X || newSize.y != size.Y)
                elem.Resize(new Vector2Int(newSize.x, newSize.y));

            EditorGUILayout.EndHorizontal();
            return elem;
        }
    }
}