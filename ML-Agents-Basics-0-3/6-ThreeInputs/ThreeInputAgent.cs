using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThreeInputAgent : Agent
{
    public GameObject LeftThruster;
    public GameObject RightThruster;
    public GameObject Marker;
    public GameObject Target;
    public GameObject Floor;
    public float StartingHeightMin = 0.5f;
    public float StartingHeightMax = 2f;
    public float TargetOffsetMin = -5f;
    public float TargetOffsetMax = 5f;
    public float UpForce = 20f;
    public float ManuverForce = 5f;
    public float MinRewardDistance = 0.1f;
    public float DistanceScale = 1f;
    public float OnMarkerPoints = 0.1f;
    public float OnCollisionPoints = -10.0f;
    public float CollisionVelocityThreshold = 5f;
    bool collisionHappened = false; // Did a collision happen?
    public int ignoreInputSteps = 1;
    int ignoreInputCount;

    public UnityEvent OnReset = new UnityEvent();

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
            MarkerRigidBody.angularVelocity = Vector3.zero;
        }
        Marker.transform.rotation = Quaternion.identity;
        Target.transform.position = transform.position + new Vector3(Random.Range(TargetOffsetMin, TargetOffsetMax), 0, 0); // Target is places randomly on the X
        Marker.transform.position = transform.position + new Vector3(Random.Range(TargetOffsetMin, TargetOffsetMax), Random.Range(StartingHeightMin, StartingHeightMax), 0); // Randomized X and Y location
        collisionHappened = false;

        ignoreInputCount = ignoreInputSteps;
        if (OnReset != null) OnReset.Invoke();
    }

    // Tell the ML algorithm everything you can about the current state
    public override void CollectObservations()
    {
        AddVectorObs(Target.transform.position.x - Marker.transform.position.x); // x distance to target
        AddVectorObs(Target.transform.position.y - Marker.transform.position.y); // y distance to target
        AddVectorObs(MarkerRigidBody.velocity.x); // Current x velocity
        AddVectorObs(MarkerRigidBody.velocity.y); // Current y velocity
        AddVectorObs(Floor.transform.position.y - Marker.transform.position.y); // distance to floor
        AddVectorObs(Marker.transform.rotation.z / 360f); // Z Rotation
    }

    // What to do every step. The Update() of an ML Agent
    public override void AgentAction(float[] actions, string textAction)
    {
        ignoreInputCount--;
        if (ignoreInputCount >= 0) return;

        // This example only uses continuous space
        if (brain.brainParameters.vectorActionSpaceType != SpaceType.continuous)
        {
            Debug.LogError("Must be continuous state type");
            return;
        }

        float leftThrust = actions[0]; // Left Thruster
        leftThrust = Mathf.Clamp(leftThrust, -1, 1); // Bound the action input from -1 to 1
        leftThrust = leftThrust * ManuverForce; // Scale in put to marker speed

        float rightThrust = actions[1]; // Right Thruster
        rightThrust = Mathf.Clamp(rightThrust, -1, 1); // Bound the action input from -1 to 1
        rightThrust = rightThrust * ManuverForce; // Scale in put to marker speed

        float upThrust = actions[2]; // Center Thruster
        upThrust = Mathf.Clamp(upThrust, -1, 1); // Bound the action input from -1 to 1
        upThrust = upThrust * UpForce; // Scale in put to marker speed

        if (MarkerRigidBody != null)
        {
            MarkerRigidBody.AddForceAtPosition(Marker.transform.up * upThrust, Marker.transform.position);
            MarkerRigidBody.AddForceAtPosition(LeftThruster.transform.up * leftThrust, LeftThruster.transform.position);
            MarkerRigidBody.AddForceAtPosition(RightThruster.transform.up * rightThrust, RightThruster.transform.position);
        }

        // There was a collision! End the game with a penalty.
        if (collisionHappened)
        {
            SetReward(OnCollisionPoints);
            ignoreInputCount = ignoreInputSteps;
            Done();
        }
        else // Everything is fine, just set a reward
        {
            SetReward(CalculateReward(Marker.transform.position, Target.transform.position));
        }
    }

    public float CalculateReward(Vector3 marker, Vector3 target)
    {
        var distance = Vector3.Distance(marker, target) * DistanceScale;
        distance = Mathf.Max(distance, MinRewardDistance); // Enforce min. distance

        var tempVal = (1.0f - (distance * distance)) * OnMarkerPoints;
        return Mathf.Max(0, tempVal);
    }

    public void CollisionHappened()
    {
        collisionHappened = true;
    }
}
