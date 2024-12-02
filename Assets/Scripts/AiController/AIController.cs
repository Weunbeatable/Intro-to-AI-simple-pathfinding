using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IntroAI.Control;
using System;

namespace IntroAI.Control
{
    /// <summary>
    /// This script is meant to give som ebasic logic that is extensible on how to run an AI controller. It's important to note
    /// that each system can be broken down into smaller manageable subsystems that are integrated into a larger whole. 
    /// In English, The logic for simply patrolling and engaging can be as complex as you'd like it to be depending on what your goals and 
    /// requirements are. Each part of the puzzle has its own mini steps, breaking down a problem into smaller steps allow you to have 
    /// really specific rules that say what should happen at each point. This is important because without it there is a lot to be desired.
    /// But with it, we can allow for preparation for a bunch of situations and it gives us the chance for some leeway when we want to try to
    /// account for more general problems. We can make generalizations on a situation so that some expected results happen whenever the AI 
    /// runs into a scenario that isn't normal. It's hard to cover everything (impossible) and we aren't even going to attempt that here,
    /// but we can make some solid rules and logic flow that allows us to have a basic, "believable" AI running. 
    /// </summary>
    public class AIController:MonoBehaviour
    {   [Tooltip("Patroll Behavior")] // Variables should be pre-declared to save time. 

        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float engageDistace = 25f;
        [SerializeField] float patrolSpeed = 2f;
        [SerializeField] float suspicionTime = 5f;
        float timeSinceLastSawPlayer = Mathf.Infinity;


        // Class refrences
        GameObject player;
        Transform target;
        private Rigidbody myRigidbody;
        Vector3 targetPosition;
       

       
        
        [Tooltip("Waypoint Behavior")]
        [SerializeField] float waypointTolerance = 2f;
        [SerializeField] PatrolPath patrolPath;
        int currentWaypointIndex = 0;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float waypointDwellTime = 2f;


        //TODO: 
        //flip characters rotation depending on if at the end or start of waypoint cycle.

        private void Start()
        {
           
            player = GameObject.FindWithTag("Hero"); // You can choose to create an alternate tag or method of player detection but this will be used for simplicity. 
            myRigidbody = GetComponent<Rigidbody>();
           // health = GetComponent<Health>(); You can later create yoru own health class with your own defined parameters, for the sake of keeping it simple 
           // we would simply check here if the character is alive or not to run any of this code. It will be commented out for the sake of the example. 
               
        }
        private void Update()
        {
            //Debug.Log("Current waypoint is " + currentWaypointIndex);
            targetPosition = this.transform.position;
            //if (health.IsDead()) return;

            // no player in sight
            if (player == null)
            {
                player = GameObject.FindWithTag("Hero");
                return;
            }
            // If we see a player and are withiin an acceptable range to attack. 
            if (attackRangeofPlayer() && facingPlayer()) // good habit is to refactor your if conditions. Try to figure out what they are meant to do 
                // then move them into a function that gets called, the function having the logic wrapped up into a name allows for easier readability and faster
                // debugging / updating. 
            {
                // If we meet the conditions of engagnement, run engagement logic. 
                EngageBehavior();
                timeSinceLastSawPlayer = 0f; // We will reset this value because we can currently see thet player!!!
            }

            // If we can see the player but can't attack due to distance, run a suspicion check 
            // basically asking the question "is anyone there"?
            else if (!attackRangeofPlayer() && timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }

            // If none of the other conditions were met, that means we aren't currently looking at the player. and there is no other action that hinders our regular 
            // way of life, so we should just keep patrolling our general area. 
            else
            {
                // print("I'm going back home");
                PatrolBehavior();
            }

            updateTimers();
        }

        private void DirectionToFace()// c ourtesy of unity forums
        {
            //The code commented out is how you would face a direction in a 2D game, I will be providing a solution for a 3D game but if you want to translate this code
            // feel free to use whats here, you can also make a request for 2D code and I will try to get on that ASAP. 

            /*           if (targetPosition.x - this.transform.position.x < 0)
                       {

                           // this.transform.localScale =  new Vector2(-1, 1);
                            this.transform.rotation = new Quaternion(0, 0, 0, 0); // changing rotating  so I don't have to modify facing player bool
                       }
                       else if (targetPosition.x - this.transform.position.x > 0)
                           {
                           //  Vector2 flipSIdes = this.transform.localScale;
                          // this.transform.localScale = new Vector2(1, 1);
                           this.transform.rotation = new Quaternion(0, -180, 0, 0);
                       }*/

            // Implementation for facing the right direction in 3D space. 
            FaceWaypoint();
           
        }

        private void updateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime; // will update by the amount the frame took on every frame. if we don't reset it, resets when we see player.
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void SuspicionBehavior()
        {
          //  print("Hmm kinda sus bro");
            Vector2 holdPosition = transform.position;
            transform.position = holdPosition;
        }

        private void PatrolBehavior()
        {
            //Vector2 nextPosition = guardingLocation;
            // Assuming we have a path that we need to patrol  Lets run the logic of how that will work.
            if (patrolPath != null)
            {
                if (atWaypoint())
                {                  
                    timeSinceArrivedAtWaypoint = 0; // reset when arrived at next waypoint                
                    CycleWaypoint();
                }
                if (timeSinceArrivedAtWaypoint > waypointDwellTime)
                {
                    MoveToNextPoint();
                    GetComponent<Animator>().SetTrigger("walk");
                    //Debug.Log("The next pos is " + nextPosition);
                    //if (currentWaypointIndex == 0 || currentWaypointIndex)
                }

            }
             
            
          //  this.transform.position = Vector2.Lerp(transform.position, guardingLocation, regularSpeed); // have to reset the transform position to going to the stored guarding location otherwise the guarding location will just be the current location.
           
          
        }

        private void MoveToNextPoint()
        {
            Vector3 nextPosition = GetCurrentWaypoint();
            // To avoid weird floating, zero out the Y position. 
            //nextPosition.y = 0;
            float regularSpeed = patrolSpeed * Time.deltaTime;
           myRigidbody.transform.position = Vector3.MoveTowards(myRigidbody.transform.position, nextPosition, regularSpeed);
            DirectionToFace();
        }
        

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            // Update the next spot we need to check out on our patroll path. 
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool atWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            GetComponent<Animator>().SetTrigger("idle");
            // Debug.Log("waypointdistance is" + distanceToWaypoint);
            return distanceToWaypoint < waypointTolerance;
        }

        private bool attackRangeofPlayer()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position); // vector2.square Magnitude is faster but for such a arbitrarily small task it doesn't really matter in this example. 

                return distanceToPlayer < chaseDistance;
            }
            else return false;
        }

        private bool facingPlayer()
        {
            float dot = Vector3.Dot(transform.right, (player.transform.position - transform.position).normalized);
            float fov = 0.7f; // Field of View of AI, may make it adjustable for enemy types at some point.
            return dot > fov;
        }

        private void EngageBehavior()
        {
            // We meet the requirements for combat. 
            if (CanAttack() == false)
            {
                Debug.Log("I'm able to attack the enemy");
                AttackBehavior();
            }
            else
            {
                GetComponent<Animator>().SetTrigger("walk");
                // We don't meet the requirements for combat which were set to us when we asked the question CanAttack()
                // in this case in order to meet the requirement we need to get a bit closer. 
                float regularSpeed = engageDistace * Time.deltaTime;
                Vector3 playersPos = player.transform.position;
                playersPos.y = 0f;
                myRigidbody.transform.position = Vector3.MoveTowards(myRigidbody.transform.position, playersPos, regularSpeed);
                
            }
        }
        public void AttackBehavior()
        {
            // For simplicity, we will keep the logic for attacking very simple, simply state message to the debug console and play an animation. 
            // This logic can be as verbosely complex or simplistic as you'd like it to be. 
            Debug.Log("We are attacking the enemy ");
        }
         
       private bool CanAttack()
        {
            bool callAttack;
            float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
            if ( distanceToPlayer < engageDistace)
            {
                callAttack = true;
            }
            else
                callAttack = false;
            return callAttack;
        }
        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(240, 224, 161, 0.9f);
            Gizmos.DrawWireSphere(transform.position, 1);
        }

        protected void FaceWaypoint()
        {
            /// The purpose of this function is to find the direction to face, it uses some simple Vector math to find the direction to face
            /// and utilizes a built in function using Quaternions 
            if (player == null) { return; } // make sure we have a target
            Vector3 facing = (GetCurrentWaypoint() - myRigidbody.transform.position); // subtract target from our postion
            facing.y = 0;// dont care about height so we clear it

            myRigidbody.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
        }

    }
}
