using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyController m_Controller;
    public static readonly int hashBeenAttacked = Animator.StringToHash("BeenAttacked");
    public Damageable.DamageMessage LastedDamage;
    public Vector3 Backpos;
    public LayerMask Floor;
    public GameObject target { get { return m_Target; } }
    protected GameObject m_Target = null;
    public EnemyController controller { get { return m_Controller; } }
    public bool PlayerFriendly;
    public virtual void TestMethod()
    {
        Debug.Log("heihei EnemyBehavior");
    }
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
}
