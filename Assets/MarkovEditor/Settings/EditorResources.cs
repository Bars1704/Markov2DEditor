using UnityEngine;

namespace MarkovEditor.Settings
{
    public class EditorResources : ScriptableObject
    {
        private static EditorResources _instance;

        public static EditorResources Instance
        {
            get
            {
                if (_instance != default)
                    return _instance;
                _instance = Resources.LoadAll<EditorResources>("/")[0];
                return _instance;
            }
        }

        public Material DefaultMaterial;
        public Sprite DefaultSprite;
        public EditorPalette EditorPalette;
    }
}