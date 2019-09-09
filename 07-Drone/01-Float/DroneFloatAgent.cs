using UnityEngine;
using MLAgents;

public class DroneFloatAgent : Agent {

    public GameObject Marker;
    public GameObject Target;
    public float StartingHeightMin = -1f;
    public float StartingHeightMax = 1f;
    public float UpForce = 10f;
    public float RewardScale = 1f;

    Rigidbody MarkerRigidBody;
    float previousDistance = 0f;

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
        previousDistance = Vector3.Distance(Marker.transform.position, Target.transform.position);
    }

    // Tell the ML algorithm everything you can about the current state
    public override void CollectObservations()
    {
        AddVectorObs(Target.transform.position - Marker.transform.position); // distance to target
        AddVectorObs(MarkerRigidBody.velocity); // Current velocity
        AddVectorObs(Marker.transform.rotation); // Quaternion
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
        var distance = Vector3.Distance(Marker.transform.position, Target.transform.position);
        var improvement = previousDistance - distance;
        previousDistance = distance;

        return RewardScale * improvement;
    }
}
