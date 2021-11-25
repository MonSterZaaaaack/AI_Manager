using System.Collections.Generic;
using Gamekit3D.Message;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamekit3D
{
    [DefaultExecutionOrder(100)]
    public class SpitterBehaviour : EnemyBehavior, IMessageReceiver
    {
        public static readonly int hashVerticalDot = Animator.StringToHash("VerticalHitDot");
        public static readonly int hashHorizontalDot = Animator.StringToHash("HorizontalHitDot");
        public static readonly int hashThrown = Animator.StringToHash("Thrown");
        public static readonly int hashHit = Animator.StringToHash("Hit");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashHaveEnemy = Animator.StringToHash("HaveTarget");
        public static readonly int hashFleeing = Animator.StringToHash("Fleeing");
        public static readonly int hashInPursuit = Animator.StringToHash("InPursuit");
        public static readonly int hashIdleState = Animator.StringToHash("Idle");
        public static readonly int hashMoveToEnemy = Animator.StringToHash("MoveToEnemy");

        public TargetScanner playerScanner;
        public float fleeingDistance = 3.0f;
        public RangeWeapon rangeWeapon;

        [Header("Audio")]
        public RandomAudioPlayer attackAudio;
        public RandomAudioPlayer frontStepAudio;
        public RandomAudioPlayer backStepAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer gruntAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer spottedAudio;
        protected float m_TimerSinceLostTarget = 0.0f;
        
        public float timeToStopPursuit;
        protected bool m_Fleeing = false;

        protected Vector3 m_RememberedTargetPosition;

        protected void OnEnable()
        {
            m_Controller = GetComponentInChildren<EnemyController>();

            m_Controller.animator.Play(hashIdleState, 0, Random.value);

            SceneLinkedSMB<SpitterBehaviour>.Initialise(m_Controller.animator, this);
            receiver = gameObject.GetComponent<Characters>() as IMessageReceiver;

        }

        public void OnReceiveMessage(Message.MessageType type, object sender, object msg)
        {
            var receiver = gameObject.GetComponent<Characters>() as IMessageReceiver;
            switch (type)
            {
                case Message.MessageType.DEAD:
                    Death((Damageable.DamageMessage)msg);
                    break;
                case Message.MessageType.DAMAGED:
                    Debug.Log(gameObject.GetComponent<GameKitVersionCharacters>().GetName());
                    ApplyDamage((Damageable.DamageMessage)msg);
                    LastedDamage = (Damageable.DamageMessage)msg;
                    Vector3 Source = LastedDamage.damageSource;
                    Vector3 Direction = gameObject.transform.position - Source;
                    Backpos = gameObject.transform.position + Direction.normalized * 30;
                    Backpos.y = 0;
                    if (m_Target == null)
                    {
                        m_Target = LastedDamage.damager.gameObject;
                    }
                    receiver.OnReceiveMessage(Gamekit3D.Message.MessageType.DAMAGED, this, msg);
                    break;
                default:
                    break;
            }
        }

        public void Death(Damageable.DamageMessage msg)
        {
            Vector3 pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            controller.AddForce(pushForce.normalized * 7.0f - Physics.gravity * 0.6f);

            controller.animator.SetTrigger(hashHit);
            controller.animator.SetTrigger(hashThrown);

            //We unparent the deathAudio source, as it would destroy it with the gameobject when it get replaced by the ragdol otherwise
            deathAudio.transform.SetParent(null, true);
            deathAudio.PlayRandomClip();

            GameObject.Destroy(deathAudio, deathAudio.clip == null ? 0.0f : deathAudio.clip.length + 0.5f);
        }

        public void ApplyDamage(Damageable.DamageMessage msg)
        {
            if (msg.damager.name == "Staff")
                CameraShake.Shake(0.06f, 0.1f);

            float verticalDot = Vector3.Dot(Vector3.up, msg.direction);
            float horizontalDot = Vector3.Dot(transform.right, msg.direction);

            Vector3 pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            controller.AddForce(pushForce.normalized * 5.5f, false);

            controller.animator.SetFloat(hashVerticalDot, verticalDot);
            controller.animator.SetFloat(hashHorizontalDot, horizontalDot);

            controller.animator.SetTrigger(hashHit);

            hitAudio.PlayRandomClip();
        }

        public void Shoot()
        {
            rangeWeapon.Attack(m_RememberedTargetPosition);
        }

        public void TriggerAttack()
        {
            m_Controller.animator.SetTrigger(hashAttack);
        }

        public void RememberTargetPosition()
        {
            if (m_Target == null)
                return;

            m_RememberedTargetPosition = m_Target.transform.position;
        }

        void PlayStep(int frontFoot)
        {
            if (frontStepAudio != null && frontFoot == 1)
                frontStepAudio.PlayRandomClip();
            else if (backStepAudio != null && frontFoot == 0)
                backStepAudio.PlayRandomClip ();
        }

        public void Grunt ()
        {
            if (gruntAudio != null)
                gruntAudio.PlayRandomClip ();
        }

        public void Spotted()
        {
            if (spottedAudio != null)
                spottedAudio.PlayRandomClip();
        }

        public void CheckNeedFleeing()
        {
            if (m_Target == null)
            {
                m_Fleeing = false;
                controller.animator.SetBool(hashFleeing, m_Fleeing);
                return;
            }

            Vector3 fromTarget = transform.position - m_Target.transform.position;

            if (m_Fleeing || fromTarget.sqrMagnitude <= fleeingDistance * fleeingDistance)
            {
                //player is too close from us, pick a point diametrically oppossite at twice that distance and try to move there.
                Vector3 fleePoint = transform.position + fromTarget.normalized * 2 * fleeingDistance;
                fleePoint.y = 0;
                Debug.DrawLine(fleePoint, fleePoint + Vector3.up * 10.0f);

                controller.SetFollowNavmeshAgent(true);
                m_Fleeing = controller.SetTarget(fleePoint);

                if(m_Fleeing)
                    controller.animator.SetBool(hashFleeing, m_Fleeing);
            }

            if (m_Fleeing && fromTarget.sqrMagnitude > fleeingDistance * fleeingDistance * 4)
            {
                //we're twice the fleeing distance from the player and fleeing, we can stop now
                m_Fleeing = false;
                controller.animator.SetBool(hashFleeing, m_Fleeing);
            }
        }

        public void FindTarget()
        {
            //we ignore height difference if the target was already seen
            List<GameObject> Enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            GameObject target = playerScanner.DetectEnemy(Enemies, this.gameObject, m_Target == null);

            if (m_Target == null)
            {
                //we just saw the player for the first time, pick an empty spot to target around them
                if (target != null)
                {
                    m_Target = target;
                    //m_Controller.animator.SetBool(hashHaveEnemy, m_Target != null);
                    receiver.OnReceiveMessage(Message.MessageType.TARGETFOUND, this, null);
                }
            }
            else
            {
                //we lost the target. But chomper have a special behaviour : they only loose the player scent if they move past their detection range
                //and they didn't see the player for a given time. Not if they move out of their detectionAngle. So we check that this is the case before removing the target
                if (target == null)
                {
                    m_TimerSinceLostTarget += Time.deltaTime;

                    if (m_TimerSinceLostTarget >= timeToStopPursuit)
                    {
                        Vector3 toTarget = m_Target.transform.position - transform.position;

                        if (toTarget.sqrMagnitude > playerScanner.detectionRadius * playerScanner.detectionRadius)
                        {
                            //the target move out of range, reset the target
                            m_Target = null;
                        }
                    }
                }
                else
                {
                    if (target != m_Target)
                    {
                        m_Target = target;

                    }

                    m_TimerSinceLostTarget = 0.0f;
                }
                m_Controller.animator.SetBool(hashHaveEnemy, m_Target != null);
            }
        }
        public override void BeenAttacked()
        {
            if (LastedDamage.damageSource == null)
            {

            }
            else
            {
                m_Target = null;
                m_Controller.SetFollowNavmeshAgent(true);
                m_Controller.SetTarget(Backpos);
                Vector3 backed = transform.position - Backpos;
                m_Controller.animator.SetBool(hashBeenAttacked, backed.sqrMagnitude > 1.0 * 1.0f);
                if (CheckDistance())
                {
                    m_Controller.SetTarget(gameObject.transform.position);
                    m_Controller.animator.SetBool(hashBeenAttacked, false);
                }
            }
        }
        public override void FoundEnemy()
        {
            Debug.Log("Spitter Found Enemy");

        }
        public override void StartPursuit()
        {
            CheckNeedFleeing();
            if (m_Fleeing)
            {
                return;
            }
            if (m_Target != null)
            {
                Vector3 toTarget = m_Target.transform.position - transform.position;
                if (toTarget.sqrMagnitude > playerScanner.detectionRadius * playerScanner.detectionRadius)
                {
                    Vector3 AttackPoint = transform.position + toTarget.normalized * (toTarget.magnitude - playerScanner.detectionRadius + 4);
                    controller.SetFollowNavmeshAgent(true);
                    bool MoveToTarget = controller.SetTarget(AttackPoint);
                    m_Controller.animator.SetBool(hashMoveToEnemy, MoveToTarget);
                }
                else
                {
                    m_Controller.animator.SetBool(hashMoveToEnemy, false);
                    TriggerAttack();
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            playerScanner.EditorGizmo(transform);
        }
#endif
    }
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(SpitterBehaviour))]
    public class SpitterBehaviourEditor : Editor
    {
        SpitterBehaviour m_Target;

        void OnEnable()
        {
            m_Target = target as SpitterBehaviour;
        }

        public override void OnInspectorGUI()
        {
            if (m_Target.playerScanner.detectionRadius < m_Target.fleeingDistance)
            {
                EditorGUILayout.HelpBox("The scanner detection radius is smaller than the fleeing range.\n" +
                    "The spitter will never shoot at the player as it will flee past the range at which it can see the player",
                    UnityEditor.MessageType.Warning, true);    
            }
            
            base.OnInspectorGUI();
        }
    }

#endif
}
