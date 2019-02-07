using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAgentContinuous : Agent
{
    public GameObject Marker;
    public GameObject Target;
    public float StartingHeightMin = -1f;
    public float StartingHeightMax = 1f;
    public float UpForce = 20f;
    public float MinRewardDistance = 0.1f;
    public float MaxRewardDistance = 1f;
    public float OnMarkerPoints = 0.1f;
    public bool Visualize = true;

    Rigidbody MarkerRigidBody;

    private void Start()
    {
        MarkerRigidBody = Marker.GetComponent<Rigidbody>();
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

        if (Visualize)
        {
            for (var y = -2f; y < 2f; y += 0.05f)
            {
                var reward = CalculateReward(y, Target.transform.position.y);
                var c = Color.Lerp(Color.black, Color.white, reward * 5);
                Debug.DrawLine(new Vector3(Marker.transform.position.x, y, Marker.transform.position.z), new Vector3(Marker.transform.position.x, y + 0.05f, Marker.transform.position.z), c, 30);
            }
        }
    }

    // Tell the ML algorithm everything you can about the current state
    public override void CollectObservations()
    {
        AddVectorObs(Target.transform.position.y - Marker.transform.position.y); // distance to target
        AddVectorObs(MarkerRigidBody.velocity.y); // Current velocity
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
        action_y = action_y * UpForce; // Scale in put to marker speed
        if (MarkerRigidBody != null)
        {
            MarkerRigidBody.AddForce(0, action_y, 0);
        }

        var reward = CalculateReward(Marker.transform.position.y, Target.transform.position.y);
        SetReward(reward);
    }

    public float CalculateReward(float markerY, float targetY)
    {
        var distance = Mathf.Clamp(Mathf.Abs(markerY - targetY), MinRewardDistance, MaxRewardDistance); // clamp from 0.1 to 1 so the math works

        return OnMarkerPoints / ((10f * distance) * (10f * distance));
    }
}