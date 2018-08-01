using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheSimplestScenario
{
    public class SimplestAgent : Agent
    {
        public GameObject Marker;
        public GameObject Target;
        public float StartingHeightMin = -1f;
        public float StartingHeightMax = 1f;
        public float MarkerSpeed = 0.02f;
        public float DistanceToEarnReward = 0.1f;
        public float OnMarkerPoints = 0.1f;

        public override void InitializeAgent()
        {
        }

        // How to reinitialize when the game is reset. The Start() of an ML Agent
        public override void AgentReset()
        {
            Marker.transform.position = new Vector3(0, Random.Range(StartingHeightMin, StartingHeightMax), 0) + transform.position;
            Target.transform.position = new Vector3(0, Random.Range(StartingHeightMin, StartingHeightMax), 0) + transform.position;
        }

        // Tell the ML algorithm everything you can about the current state
        public override void CollectObservations()
        {
            AddVectorObs(Target.transform.position.y - Marker.transform.position.y); // distance to target
        }

        // What to do every step. The Update() of an ML Agent
        public override void AgentAction(float[] actions, string textAction)
        {
            // This example only uses continuous space
            if (brain.brainParameters.vectorActionSpaceType != SpaceType.continuous)
            {
                Debug.LogError("Must be continuous state type");
                return;
            }

            float action_y = actions[0]; // The agent has only one possible action. Up/Down amount
            action_y = Mathf.Clamp(action_y, -1, 1); // Bound the action input from -1 to 1
            action_y = action_y * MarkerSpeed; // Scale in put to marker speed
            Marker.transform.position += new Vector3(0, action_y, 0); // Move up or down

            if (Mathf.Abs(Marker.transform.position.y - Target.transform.position.y) < DistanceToEarnReward)
            {
                SetReward(OnMarkerPoints);
            }
        }
    }
}
