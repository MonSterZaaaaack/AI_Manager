using Gamekit3D;
using Gamekit3D.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameKitVersionCharacters : Characters, IMessageReceiver
{
    public GameKitVersionCharacters(string Name, int ID, CharacterClass Class):base(Name,ID,Class)
    {
    }
    public void damaged()
    {

        EnemyBehavior Behaviour = gameObject.GetComponent<EnemyBehavior>();
        //controller.WalkBackToBase();
        int hashBeenAttacked = Animator.StringToHash("BeenAttacked");
        Behaviour.controller.animator.SetBool(hashBeenAttacked, true);
    }
    public void OnReceiveMessage(Gamekit3D.Message.MessageType type, object sender, object msg)
    {
        string condition = type.ToString();
        Debug.Log(condition);
        TestCondition eventcondition = new TestCondition(condition);
        if(msg != null)
        {
            lastestMessage = (Damageable.DamageMessage)msg;
        }
        GetEvent(eventcondition);

    }
}
