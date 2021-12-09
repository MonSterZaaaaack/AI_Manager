using Gamekit3D.Message;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamekit3D
{
    [DefaultExecutionOrder(100)]
    public class ChomperBehavior : EnemyBehavior, IMessageReceiver
    {
        public static readonly int hashInPursuit = Animator.StringToHash("InPursuit");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashHit = Animator.StringToHash("Hit");
        public static readonly int hashVerticalDot = Animator.StringToHash("VerticalHitDot");
        public static readonly int hashHorizontalDot = Animator.StringToHash("HorizontalHitDot");
        public static readonly int hashThrown = Animator.StringToHash("Thrown");
        public static readonly int hashGrounded = Animator.StringToHash("Grounded");
        public static readonly int hashVerticalVelocity = Animator.StringToHash("VerticalVelocity");
        public static readonly int hashSpotted = Animator.StringToHash("Spotted");
        public static readonly int hashNearBase = Animator.StringToHash("NearBase");

        public static readonly int hashIdleState = Animator.StringToHash("ChomperIdle");



        public TargetDistributor.TargetFollower followerData { get { return m_FollowerInstance; } }

        public Vector3 originalPosition { get; protected set; }
        [System.NonSerialized]
        public float attackDistance = 3;
       
        public MeleeWeapon meleeWeapon;
        public TargetScanner playerScanner;
        [Tooltip("Time in seconde before the Chomper stop pursuing the player when the player is out of sight")]
        public float timeToStopPursuit;

        [Header("Audio")]
        public RandomAudioPlayer attackAudio;
        public RandomAudioPlayer frontStepAudio;
        public RandomAudioPlayer backStepAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer gruntAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer spottedAudio;

        protected float m_TimerSinceLostTarget = 0.0f;

        protected TargetDistributor.TargetFollower m_FollowerInstance = null;
        protected void OnEnable()
        {
            m_Controller = GetComponentInChildren<EnemyController>();

            originalPosition = transform.position;

            meleeWeapon.SetOwner(gameObject);

            m_Controller.animator.Play(hashIdleState, 0, Random.value);

            SceneLinkedSMB<ChomperBehavior>.Initialise(m_Controller.animator, this);
            Backpos = originalPosition;
            receiver = gameObject.GetComponent<Characters>() as IMessageReceiver;
        }
        /// <summary>
        /// Called by animation events.
        /// </summary>
        /// <param name="frontFoot">Has a value of 1 when it's a front foot stepping and 0 when it's a back foot.</param>
        void PlayStep(int frontFoot)
        {
            if (frontStepAudio != null && frontFoot == 1)
                frontStepAudio.PlayRandomClip();
            else if (backStepAudio != null && frontFoot == 0)
                backStepAudio.PlayRandomClip ();
        }

        /// <summary>
        /// Called by animation events.
        /// </summary>
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

        protected void OnDisable()
        {
            if (m_FollowerInstance != null)
                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
        }

        private void FixedUpdate()
        {
            m_Controller.animator.SetBool(hashGrounded, controller.grounded);

            Vector3 toBase = originalPosition - transform.position;
            toBase.y = 0;

            m_Controller.animator.SetBool(hashNearBase, toBase.sqrMagnitude < 0.1 * 0.1f);
        }

        public void FindTarget()
        {
            //we ignore height difference if the target was already seen
            List<GameObject> Enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            Enemies.Add(GameObject.FindGameObjectWithTag("Player"));
            GameObject target = playerScanner.DetectEnemy(Enemies,this.gameObject, m_Target == null);

            if (m_Target == null)
            {
                //we just saw the player for the first time, pick an empty spot to target around them
                if (target != null)
                {
                    //m_Controller.animator.SetTrigger(hashSpotted);
                    receiver.OnReceiveMessage(MessageType.TARGETFOUND, this, null);
                    m_Target = target;
                    TargetDistributor distributor = target.GetComponent<TargetDistributor>();
                    if (distributor != null)
                        m_FollowerInstance = distributor.RegisterNewFollower();
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
                            if (m_FollowerInstance != null)
                                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

                            //the target move out of range, reset the target
                            m_Target = null;
                        }
                    }
                }
                else
                {
                    if (target != m_Target)
                    {
                        if (m_FollowerInstance != null)
                            m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

                        m_Target = target;

                        TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
                        if (distributor != null)
                            m_FollowerInstance = distributor.RegisterNewFollower();
                    }

                    m_TimerSinceLostTarget = 0.0f;
                }
            }
        }

        public override void StartPursuit()
        {
            if (m_FollowerInstance != null)
            {
                m_FollowerInstance.requireSlot = true;
                RequestTargetPosition();
            }

            m_Controller.animator.SetBool(hashInPursuit, true);
        }

        public void StopPursuit()
        {
            if (m_FollowerInstance != null)
            {
                m_FollowerInstance.requireSlot = false;
            }

            m_Controller.animator.SetBool(hashInPursuit, false);
        }

        public void RequestTargetPosition()
        {
            Vector3 fromTarget = transform.position - m_Target.transform.position;
            fromTarget.y = 0;
            m_FollowerInstance.requiredPoint = m_Target.transform.position + fromTarget.normalized * attackDistance * 0.9f;
        }

        public void WalkBackToBase()
        {
            if (m_FollowerInstance != null)
                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
            m_Target = null;
            StopPursuit();
            m_Controller.SetTarget(originalPosition);
            m_Controller.SetFollowNavmeshAgent(true);
        }
        public override void BeenAttacked()
        {
            if (LastedDamage.damageSource == null)
            {
               
            }
            else
            {
                if (m_FollowerInstance != null)
                    m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
                m_Target = null;
                StopPursuit();
                m_Controller.SetFollowNavmeshAgent(true);
                m_Controller.SetTarget(Backpos);
                Vector3 backed = transform.position - Backpos;
                m_Controller.animator.SetBool(hashBeenAttacked, backed.sqrMagnitude > 1.0f*1.0f);
                if (CheckDistance())
                {
                    m_Controller.SetTarget(gameObject.transform.position);
                    m_Controller.animator.SetBool(hashBeenAttacked, false);
                }
            }
        }
        public void TriggerAttack()
        {
            m_Controller.animator.SetTrigger(hashAttack);
        }

        public void AttackBegin()
        {
            meleeWeapon.BeginAttack(false);
        }

        public void AttackEnd()
        {
            meleeWeapon.EndAttack();
        }

        public void OnReceiveMessage(Message.MessageType type, object sender, object msg)
        {
            switch (type)
            {
                case Message.MessageType.DEAD:
                    Damageable.DamageMessage message = (Damageable.DamageMessage)msg;
                    Death(message);
                    break;
                case Message.MessageType.DAMAGED:
                    ApplyDamage((Damageable.DamageMessage)msg);
                    LastedDamage = (Damageable.DamageMessage)msg;
                    Vector3 Source = LastedDamage.damageSource;
                    Vector3 Direction = gameObject.transform.position - Source;
                    Backpos = gameObject.transform.position + Direction.normalized * 20;
                    Backpos.y = 0;
                    if(m_Target == null && !LastedDamage.damager.transform.root.gameObject.CompareTag("Player"))
                    {
                        m_Target = LastedDamage.damager.gameObject;
                        TargetDistributor distributor = m_Target.GetComponent<TargetDistributor>();
                        if (distributor != null)
                            m_FollowerInstance = distributor.RegisterNewFollower();

                    }
                    break;
                case Message.MessageType.TARGETDEAD:
                    Debug.Log(gameObject.name + " Stop Pursing " + ((GameObject)sender).name + " Target Died");
                    StopPursuit();
                    break;
                default:
                    break;
            }
            receiver.OnReceiveMessage(type, this, msg);
        }
        public override void FoundEnemy()
        {
            m_Controller.animator.SetTrigger(hashSpotted);

        }
        public override void IgnoreEnemy()
        {
            ;

        }

        public void Death(Damageable.DamageMessage msg)
        {
            Vector3 pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            controller.AddForce(pushForce.normalized * 7.0f - Physics.gravity * 0.6f);

            controller.animator.SetTrigger(hashHit);
            controller.animator.SetTrigger(hashThrown);

            //We unparent the hit source, as it would destroy it with the gameobject when it get replaced by the ragdol otherwise
            deathAudio.transform.SetParent(null, true);
            deathAudio.PlayRandomClip();
            GameObject.Destroy(deathAudio, deathAudio.clip == null ? 0.0f : deathAudio.clip.length + 0.5f);
        }

        public void ApplyDamage(Damageable.DamageMessage msg)
        {
            //TODO : make that more generic, (e.g. move it to the MeleeWeapon code with a boolean to enable shaking of camera on hit?)
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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            playerScanner.EditorGizmo(transform);
        }
#endif
    }
}