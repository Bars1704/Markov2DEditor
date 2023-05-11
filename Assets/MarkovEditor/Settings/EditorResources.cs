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
                _instance.UpArrowContent = new GUIContent("", _instance.UpArrowSprite);
                _instance.DownArrowContent = new GUIContent("", _instance.DownArrowSprite);
                return _instance;
            }
        }

        public Material DefaultMaterial;
        public Sprite DefaultSprite;
        public EditorPalette EditorPalette;
        public GUIContent UpArrowContent;
        public GUIContent DownArrowContent;
        public Texture UpArrowSprite;
        public Texture DownArrowSprite;
    }
}