using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gravity
{
    public class GravityAgent : Agent
    {
        public GameObject Marker;
        public GameObject Target;
        public float StartingHeightMin = -1f;
        public float StartingHeightMax = 1f;
        public float UpForce = 0.02f;
        public float DistanceToEarnReward = 0.1f;
        public float OnMarkerPoints = 0.1f;

        Rigidbody MarkerRigidBody;
        BehaviorParameters behaviorParams;

        private void Start()
        {
            MarkerRigidBody = Marker.GetComponent<Rigidbody>();
            behaviorParams = GetComponent<BehaviorParameters>();
        }

        // How to reinitialize when the game is reset. The Start() of an ML Agent
        public override void AgentReset()
        {
            if (MarkerRigidBody != null)
            {
                MarkerRigidBody.velocity = Vector3.zero;
                MarkerRigidBody.rotation = Quaternion.identity;
            }

            Marker.transform.position = new Vector3(0, Random.Range(StartingHeightMin, StartingHeightMax), 0) + transform.position;
            Target.transform.position = new Vector3(0, Random.Range(StartingHeightMin, StartingHeightMax), 0) + transform.position;
        }

        // Tell the ML algorithm everything you can about the current state
        public override void CollectObservations()
        {
            AddVectorObs(Target.transform.position.y - Marker.transform.position.y); // distance to target
            AddVectorObs(MarkerRigidBody.velocity.y); // Current velocity
        }

        // What to do every step. The Update() of an ML Agent
        public override void AgentAction(float[] actions)
        {
            // This example only uses continuous space
            if (behaviorParams.brainParameters.vectorActionSpaceType != SpaceType.Continuous)
            {
                Debug.LogError("Must be continuous state type");
                return;
            }

            float action_y = actions[0]; // The agent has only one possible action. Up/Down amount
            action_y = Mathf.Clamp(action_y, -1, 1); // Bound the action input from -1 to 1
            action_y = action_y * UpForce; // Scale in put to marker speed
            if (MarkerRigidBody != null)
            {
                MarkerRigidBody.AddForce(0, action_y, 0);
            }

            if (Mathf.Abs(Marker.transform.position.y - Target.transform.position.y) < DistanceToEarnReward)
            {
                SetReward(OnMarkerPoints);
            }
        }
    }
}
