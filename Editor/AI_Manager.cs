using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AI_Manager : EditorWindow
{
    // Start is called before the first frame update
    [MenuItem("Asset/AI_Controller")]
    public static void ShowWindow()
    {
        var window = GetWindow<AI_Manager>();
        window.title = "AI_Controller";
    }

    // Update is called once per frame
    public void OnGUI()
    {
        GUILayout.Label("AI_Controller");
        if (GUILayout.Button("Classes"))
        {
            ClassManager.ShowWindow();
            Close();
        }
        if (GUILayout.Button("Characters"))
        {
            CharacterManager.ShowWindow();
            Close();
        }
        if (GUILayout.Button("Events"))
        {
            EventManager.ShowWindow();
            Close();
        }
        if (GUILayout.Button("Traits"))
        {
            TraitsManager.ShowWindow();
            Close();
        }
        if (GUILayout.Button("Showpath"))
        {
            Debug.Log(Application.persistentDataPath);
        }

    }
}