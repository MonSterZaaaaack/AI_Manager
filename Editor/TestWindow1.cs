using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TestWindow1 : EditorWindow
{
    GameObject obj = null;
    GameObject tempobj = null;
    Vector3 vec3 = new Vector3(0, 0, 0);
    CharacterClass classname = null;
    string objname = "";
    [MenuItem("Asset/TestCreate1")]
    public static void ShowWindow()
    {
        var window = GetWindow<AddCharacter>();
        window.title = "Create Character";
    }
    public void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        obj = EditorGUILayout.ObjectField("Select game object to place", obj, typeof(GameObject), true) as GameObject;
        vec3 = EditorGUILayout.Vector3Field("placement position:", vec3);
        objname = EditorGUILayout.TextField("Name for new Character: ", objname);

        if (obj && objname != "")
        {
            if (GUILayout.Button("Add to scene"))
            {
                tempobj = (GameObject)Instantiate(obj, vec3, Quaternion.identity);
                tempobj.name = objname;
                Close();
            }
        }
        else
        {
            EditorGUILayout.LabelField("You must select an object and give it a name first");
        }
        EditorGUILayout.EndVertical();
    }
}
