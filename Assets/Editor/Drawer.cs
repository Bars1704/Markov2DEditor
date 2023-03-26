using System;
using System.Collections.Generic;
using System.Linq;
using MarkovEditor;
using MarkovTest.ThreeDimension;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class Drawer
    {
        private static readonly List<Type> DrawerTypes = new List<Type>();

        public static readonly Dictionary<object, object> CachedDrawers = new Dictionary<object, object>();

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
            if(elem == default)
            {
                EditorGUILayout.HelpBox($"Elem is null", MessageType.Error);
                return;
            }
            
            if (CachedDrawers.ContainsKey(elem))
            {
                InvokeDraw(CachedDrawers[elem], elem, sim);
                return;
            }
            
            var elemType = elem.GetType();

            var seekType = typeof(IEditorElementDrawer<,>).MakeGenericType(elemType, sim.GetType());
            var type = DrawerTypes.FirstOrDefault(x => seekType.IsAssignableFrom(x));

            if (type == default)
            {
                EditorGUILayout.HelpBox($"No Drawer for {elemType}", MessageType.Error);
                return;
            }

            var drawer = Activator.CreateInstance(type);
            CachedDrawers.Add(elem, drawer);
            
            InvokeDraw(drawer, elem, sim);
        }

        private static void InvokeDraw(object drawer, object elem, IMarkovSimulationDrawer sim)
        {
            var method = drawer.GetType().GetMethod("Draw", new Type[] { elem.GetType(), sim.GetType() });
            method.Invoke(drawer, new object[] { elem, sim });
        }
    }
}