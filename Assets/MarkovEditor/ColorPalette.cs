using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityMarkov/Palette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private Color32[] Colors;

    public int Length => Colors.Length;
    public Color32 GetColor(int index) =>  Colors[index];
}