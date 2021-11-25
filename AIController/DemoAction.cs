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
    public Dictionary<string, MethodInfo> AvailableMethods;
    public DemoAction()
    {
        ActionName = new List<string>();
        Type t = this.GetType();
        MethodInfo[] m = t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.DeclaringType == t).OrderBy(x => x.Name).ToArray();
        methodslist = new List<MethodInfo>();
        AvailableMethods = new Dictionary<string, MethodInfo>();
        foreach(MethodInfo temp in m)
        {
            ActionName.Add(temp.Name);
            methodslist.Add(temp);
            AvailableMethods.Add(temp.Name, temp);
            
        }
    }
    public void AMethod()
    {
        return;
    }
    public void BMethod()
    {
        return;
    }
    public void TestMethod1()
    {
        return;
    }
    public void CMethod2()
    {
        return;
    }
    public void BMethod2()
    {
        return;
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
    public void CharacterFoundEnemy(object Chomper)
    {
        GameKitVersionCharacters chomper = (GameKitVersionCharacters)Chomper;
        chomper.gameObject.GetComponent<EnemyBehavior>().FoundEnemy();
    }
    public void CharacterIgnoreEnemy(object Chomper)
    {
        GameKitVersionCharacters chomper = (GameKitVersionCharacters)Chomper;
        chomper.gameObject.GetComponent<EnemyBehavior>().IgnoreEnemy();
        return;
    }

    public void TestAction4()
    {
        Debug.Log(4);
    }
    public void TestAction3()
    {
        Debug.Log(3);
    }
}
