using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

public class MarkovDebugger : EditorWindow
{

    [MenuItem("Window/MarkowDebugger")]
    public static void OpenWindow()
    {
        GetWindow<MarkovDebugger>();
    }

    private void OnGUI()
    {
        
    }
}
#endif