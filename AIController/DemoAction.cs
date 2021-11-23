using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
[Serializable]
public class DemoAction
{
    public IList<string> ActionName;
    public IList<MethodInfo> methodslist;
    public DemoAction()
    {
        ActionName = new List<string>();
        Type t = this.GetType();
        MethodInfo[] m = t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.DeclaringType == t).OrderBy(x => x.Name).ToArray();
        methodslist = new List<MethodInfo>();
        foreach(MethodInfo temp in m)
        {
            ActionName.Add(temp.Name);
            methodslist.Add(temp);
            
        }
    }
    public void ChomperRunAway(object Chomper)
    {
        GameKitVersionCharacters chomper = (GameKitVersionCharacters)Chomper;
        Debug.Log(chomper.GetName() + " Choose To RunAway");
        chomper.damaged();
        return;
    }
    public void ChomperFight(object Chomper)
    {
        GameKitVersionCharacters chomper = (GameKitVersionCharacters)Chomper;
        Debug.Log(chomper.GetName() + " Choose To Fight " + chomper.gameObject.GetComponent<EnemyBehavior>().target.name);
        chomper.gameObject.GetComponent<EnemyBehavior>().StartPursuit();
        return;
    }
    public void TestAction3()
    {
        Debug.Log(3);
    }
    public void TestAction4()
    {
        Debug.Log(4);
    }
}
