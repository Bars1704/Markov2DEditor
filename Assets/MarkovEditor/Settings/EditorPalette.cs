using System;
using System.Collections.Generic;
using System.Linq;
using Markov.MarkovTest.Sequences;
using UnityEngine;

namespace MarkovEditor
{
    [Serializable]
    public class ColorStringTuple
    {
        public Color Color;
        public string TypeName;
    }

    public class EditorPalette : ScriptableObject
    {
        public List<ColorStringTuple> Colors = new List<ColorStringTuple>();

        public bool HasColor(Type type) => Colors.FirstOrDefault(x => x.TypeName == GetName(type)) != default;

        public Color GetColor(Type type)
        {
            var color = Colors.FirstOrDefault(x => x.TypeName == GetName(type));
            if (color == default)
            {
                Debug.LogError($"There is no color gor type {type.FullName}");
                return default;
            }

            return color.Color;
        }

        public void Fill()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => !t.IsInterface && !t.IsAbstract);

            var seekedType = typeof(ISequencePlayable<,>);

            var result = allTypes.Where(type => type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(x => seekedType.IsAssignableFrom(x))
            );

            var names = result.Select(GetName).ToList();

            var colorsToRemove = Colors.Where(x => !names.Contains(x.TypeName)).Select(x => x.TypeName).ToList();

            for (var i = 0; i < colorsToRemove.Count; i++)
            {
                var typeName = colorsToRemove[i];
                Colors.Remove(Colors.FirstOrDefault(x => x.TypeName == typeName));
            }

            foreach (var typeName in names.Where(typeName => Colors.All(x => x.TypeName != typeName)))
            {
                Colors.Add(new ColorStringTuple() { TypeName = typeName, Color = Color.clear });
            }
        }

        private string GetName(Type x)
        {
            return $"{x?.Namespace?.Replace("Markov.MarkovTest.", "")}.{x.Name.Split('`').First()}";
        }
    }
}