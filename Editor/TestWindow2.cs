using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestWindow2 : EditorWindow
{
    // Start is called before the first frame update
    [MenuItem("Asset/testWindow2")]
    public static void ShowWindow()
    {
        var window = GetWindow<TestWindow2>();
        window.title = "The Window";
    }

    // Update is called once per frame
    public void OnGUI()
    {
        GUILayout.Label("TestWindow2");
        if (GUILayout.Button("GotoWindow1"))
        {
            TestWindow1.ShowWindow();
            Close();
        }
    }
}