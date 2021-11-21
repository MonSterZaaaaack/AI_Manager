using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AddClass : EditorWindow
{
    // Start is called before the first frame update
    string classname = "Please Enter the Name of the new Class";
    int size = 1;
    public static void ShowWindow()
    {
        var window = GetWindow<AddClass>();
        window.title = "Add New Class";
    }
    public void OnGUI()
    {
        classname = GUILayout.TextField(classname);
        if(GUILayout.Button("Add new Class"))
        {
            CharacterClass temp = new CharacterClass(classname, size);
            AIManager.Instance.AddClass(temp);
            ClassManager ManagerWindow = EditorWindow.GetWindow<ClassManager>();
            ManagerWindow.RefreshClasses();
            Close();
        }
    }
}
