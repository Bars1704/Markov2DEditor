using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Editor
{
    public static class Drawer
    {
        private static List<Type> DrawerTypes = new List<Type>();


        static Drawer()
        {
            DrawerTypes = GetAllDrawers().ToList();
        }

        private static IEnumerable<Type> GetAllDrawers()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => !t.IsInterface && !t.IsAbstract);

            var seekedType = typeof(IEditorElementDrawer<>);

            var result = allTypes.Where(type => type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(x => seekedType.IsAssignableFrom(x))
            );
            return result;
        }


        public static void Draw(object elem, MarkovSimulation2D sim)
        {
            var elemType = elem.GetType();
            var seekenType = typeof(IEditorElementDrawer<>).MakeGenericType(elemType);
            var type = DrawerTypes.FirstOrDefault(x => seekenType.IsAssignableFrom(x));

            if (type == default)
            {
                EditorGUILayout.HelpBox($"No Drawer for {elemType}", MessageType.Error);
                return;
            }

            var drawer = Activator.CreateInstance(type);
            var method = type.GetMethod("Draw", new Type[] { elemType, typeof(MarkovSimulation2D) });
            method.Invoke(drawer, new object[] { elem, sim });
        }
    }
}