using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
abstract public class EventCondition
{

    private string name;
    public EventCondition()
    {
        name = "testcondition";
    }
    abstract public string getName();
    abstract public bool Examcondition(EventCondition con);

}
