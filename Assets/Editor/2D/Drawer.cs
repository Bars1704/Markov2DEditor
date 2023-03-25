using System;
using System.Collections.Generic;
using System.Linq;
using MarkovEditor;
using MarkovEditor._2D;
using MarkovTest;
using UnityEditor;

namespace Editor._2D
{
    public static class Drawer
    {
        private static readonly List<Type> DrawerTypes = new List<Type>();

        static Drawer()
        {
            DrawerTypes = GetAllDrawers().ToList();
        }

        private static IEnumerable<Type> GetAllDrawers()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => !t.IsInterface && !t.IsAbstract);

            var seekedType = typeof(IEditorElementDrawer<,>);

            var result = allTypes.Where(type => type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(x => seekedType.IsAssignableFrom(x))
            );
            return result;
        }


        public static void Draw(object elem, IMarkovSimulationDrawer sim)
        {
            var elemType = elem.GetType();
            
            var type = DrawerTypes.Select(x => (x, x.GetInterfaces().Select(y => y.GetGenericArguments()[0])))
                .FirstOrDefault(x =>
                    x.Item2.Where(y => y.IsGenericType).Select(y => y.GetGenericTypeDefinition())
                        .Contains(elemType.GetGenericTypeDefinition())).x;
            
            if (type == default)
            {
                EditorGUILayout.HelpBox($"No Drawer for {elemType}", MessageType.Error);
                return;
            }

            var drawer = Activator.CreateInstance(type);
            var method = type.GetMethod("Draw", new Type[] { elemType, sim.GetType() });
            method.Invoke(drawer, new object[] { elem, sim });
        }
    }
}