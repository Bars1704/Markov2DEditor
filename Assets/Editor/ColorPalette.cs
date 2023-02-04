using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityMarkov\\Palette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private Color32[] Colors;

    public Color32 GetColor(int index) => Colors[index + 1];
}