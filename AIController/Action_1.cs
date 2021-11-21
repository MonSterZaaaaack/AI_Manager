using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]

abstract public class Action_1
{
    private IList<Characters> Targets;
    private string name;
    abstract public void SetTargets();
    abstract public void Do();
}
