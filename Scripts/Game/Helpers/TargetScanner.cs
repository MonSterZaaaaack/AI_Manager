using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gamekit3D
{
    //use this class to simply scan & spot the player based on the parameters.
    //Used by enemies behaviours.
    [System.Serializable]
    public class TargetScanner
    {
        public float heightOffset = 0.0f;
        public float detectionRadius = 10;
        [Range(0.0f, 360.0f)]
        public float detectionAngle = 270;
        public float maxHeightDifference = 1.0f;
        public LayerMask viewBlockerLayerMask;


        /// <summary>
        /// Check if the player is visible according to that Scanner parameter.
        /// </summary>
        /// <param name="detector">The transform from which run the detection</param>
        /// /// <param name="useHeightDifference">If the computation should comapre the height difference to the maxHeightDifference value or ignore</param>
        /// <returns>The player controller if visible, null otherwise</returns>
        
        public PlayerController Detect(Transform detector, bool useHeightDifference = true)
        {
            //if either the player is not spwned or they are spawning, we do not target them
            
            if (PlayerController.instance == null || PlayerController.instance.respawning)
                return null;

            Vector3 eyePos = detector.position + Vector3.up * heightOffset;
            Vector3 toPlayer = PlayerController.instance.transform.position - eyePos;
            Vector3 toPlayerTop = PlayerController.instance.transform.position + Vector3.up * 1.5f - eyePos;

            if (useHeightDifference && Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
            { //if the target is too high or too low no need to try to reach it, just abandon pursuit
                return null;
            }

            Vector3 toPlayerFlat = toPlayer;
            toPlayerFlat.y = 0;

            if (toPlayerFlat.sqrMagnitude <= detectionRadius * detectionRadius)
            {
                if (Vector3.Dot(toPlayerFlat.normalized, detector.forward) >
                    Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
                {

                    bool canSee = false;

                    Debug.DrawRay(eyePos, toPlayer, Color.blue);
                    Debug.DrawRay(eyePos, toPlayerTop, Color.blue);

                    canSee |= !Physics.Raycast(eyePos, toPlayer.normalized, detectionRadius,
                        viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                    canSee |= !Physics.Raycast(eyePos, toPlayerTop.normalized, toPlayerTop.magnitude,
                        viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                    if (canSee)
                        return PlayerController.instance;
                }
            }

            return null;
        }
        // Find the lastest enemy around
        // Ignore itself and any members from its own class
        public GameObject DetectEnemy(IList<GameObject> Enemies,GameObject detector, bool useHeightDifference = true)
        {
            //if either the player is not spwned or they are spawning, we do not target them
            GameObject lastestEnemy = null;
            float distance = float.PositiveInfinity;
            string detectorclass = detector.GetComponent<Characters>().GetClassName();
            if (!detector.GetComponent<EnemyBehavior>().PlayerFriendly)
            {
                Enemies.Add(GameObject.FindGameObjectWithTag("Player"));
            }
            foreach(var enemy in Enemies)
            {
                if(GameObject.ReferenceEquals(enemy,detector))
                {
                    continue;
                }
                if (!enemy.tag.Equals("Player"))
                {
                    string enemyclass = enemy.GetComponent<Characters>().GetClassName();
                    if (detectorclass.Equals(enemyclass))
                    {
                        continue;
                    }
                }
                
                Vector3 tothis = enemy.transform.position - detector.transform.position;
                tothis.y = 0;
                float tothisflat = tothis.magnitude;
                if(tothisflat < distance)
                {
                    lastestEnemy = enemy;
                    distance = tothisflat;
                }
            }

            if (lastestEnemy == null)
                return null;

            Vector3 eyePos = detector.transform.position + Vector3.up * heightOffset;
            Vector3 toPlayer = lastestEnemy.transform.position - eyePos;
            Vector3 toPlayerTop = lastestEnemy.transform.position + Vector3.up * 1.5f - eyePos;

            if (useHeightDifference && Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
            { //if the target is too high or too low no need to try to reach it, just abandon pursuit
                return null;
            }

            Vector3 toPlayerFlat = toPlayer;
            toPlayerFlat.y = 0;

            if (toPlayerFlat.sqrMagnitude <= detectionRadius * detectionRadius)
            {
                if (Vector3.Dot(toPlayerFlat.normalized, detector.transform.forward) >
                    Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
                {

                    bool canSee = false;

                    Debug.DrawRay(eyePos, toPlayer, Color.blue);
                    Debug.DrawRay(eyePos, toPlayerTop, Color.blue);

                    canSee |= !Physics.Raycast(eyePos, toPlayer.normalized, detectionRadius,
                        viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                    canSee |= !Physics.Raycast(eyePos, toPlayerTop.normalized, toPlayerTop.magnitude,
                        viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                    if (canSee)
                        return lastestEnemy;
                }
            }

            return null;
        }

#if UNITY_EDITOR

        public void EditorGizmo(Transform transform)
        {
            Color c = new Color(0, 0, 0.7f, 0.4f);

            UnityEditor.Handles.color = c;
            Vector3 rotatedForward = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
            UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotatedForward, detectionAngle, detectionRadius);

            Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            Gizmos.DrawWireSphere(transform.position + Vector3.up * heightOffset, 0.2f);
        }

#endif
    }

}