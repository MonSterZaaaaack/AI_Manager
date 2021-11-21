using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SImulator : EditorWindow
{
    IList<Characters> targetlist;

    public static void ShowWindow()
    {
        var window = GetWindow<SImulator>();
        window.title = "Simulator";
    }
    public void OnGUI()
    {
        GUILayout.BeginVertical();
        GetChars();
        ShowChars();
        GUILayout.Label("TargetSize" + targetlist.Count.ToString());
        if (GUILayout.Button("Begin Simulation"))
        {
            ShowChars();
        }
        if(GUILayout.Button("Back To Main"))
        {
            AI_Manager.ShowWindow();
            Close();
        }
        GUILayout.EndVertical();

    }
    public void GetChars()
    {
        CharacterClass testclass = AIManager.Instance.GetClass("Testclass");
        targetlist = new List<Characters>();
        foreach(string val in testclass.GetAllCharacters())
        {
            Characters temp = GameObject.Find(val).GetComponent<Characters>();
            if(temp != null)
            {
                targetlist.Add(temp);
            }
        }

    }
    public void ShowChars()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Character Name :");
        GUILayout.Label("Number of Brave Actions:");
        GUILayout.Label("Number of Timid Actions:");
        GUILayout.EndHorizontal();
        for(int i = 0; i < targetlist.Count; i++)
        {
            GUILayout.BeginHorizontal();
            int event1 = 0;
            int event2 = 0;
            EventCondition tempcon = new TestCondition();
            for(int j = 0; j < 1000; j++)
            {
                int testevent = targetlist[i].TestGetEvent(tempcon);
                if(testevent == 0)
                {
                    event1++;
                }
                else if(testevent == 1)
                {
                    event2++;
                }
            }
            GUILayout.Label(targetlist[i].GetName());
            GUILayout.Label(event1.ToString());
            GUILayout.Label(event2.ToString());
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndVertical();
    }
    public void Test()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Character Name :");
        GUILayout.Label("Number of Brave Actions:");
        GUILayout.Label("Number of Timid Actions:");
        GUILayout.EndHorizontal();
        for (int i = 0; i < targetlist.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(targetlist[i].GetName());
            GUILayout.Label(UnityEngine.Random.Range(1,20).ToString());
            GUILayout.Label(UnityEngine.Random.Range(50,80).ToString());
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }
}
