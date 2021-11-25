using Gamekit3D;
using Gamekit3D.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyController m_Controller;
    public static readonly int hashBeenAttacked = Animator.StringToHash("BeenAttacked");
    public Damageable.DamageMessage LastedDamage;
    public Vector3 Backpos;
    public LayerMask Floor;
    public IMessageReceiver receiver;
    public GameObject target { get { return m_Target; } }
    protected GameObject m_Target = null;
    public EnemyController controller { get { return m_Controller; } }
    public bool PlayerFriendly;
    public virtual void StartPursuit()
    {
        Debug.Log(gameObject.name + "Entered Base Start Pursuit.");
    }
    public virtual void BeenAttacked()
    {
        Debug.Log(gameObject.name + " Called Base Function : Been Attacked");
    }
    public virtual bool CheckDistance()
    {
        bool HitFloor = false;
        HitFloor = Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, 1.0f, Floor, QueryTriggerInteraction.Ignore);
        return HitFloor;
    }
    public virtual void FoundEnemy()
    {
        Debug.Log("Base Found Enemy");
    }
    public virtual void IgnoreEnemy()
    {
        Debug.Log("Base Ignore Enemy");
    }
}
