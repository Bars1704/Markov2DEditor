using UnityEngine;

namespace Editor
{
    public static class GUIElementsFabric
    {
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public static GUIStyle CreateColorStyle(Color color)
        {
            return new GUIStyle(GUI.skin.box)
            {
                normal =
                {
                    background = MakeTex(2, 2, color)
                }
            };
        }
    }
}