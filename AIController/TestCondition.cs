using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class TestCondition : EventCondition
{
    /**
     * A revised version of EventCondition that fits the 3D Gamekit Project
     */
    public string name;
    public TestCondition(string condition)
    {
        name = condition;
    }
    public TestCondition()
    {
        name = "Damaged";
    }
    public override bool Examcondition(EventCondition condition)
    {
        return condition.getName().Equals(name);
    }

    public override string getName()
    {
        return name;
    }
}
