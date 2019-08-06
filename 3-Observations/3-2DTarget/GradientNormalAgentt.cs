using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientNormalAgentt : Agent {

    public GameObject Marker;
    public GameObject Target;
    public Vector2 WidthMinMax = new Vector2(-5, 5);
    public float MarkerSpeed = 1f;
    public float TotalPenaltyOverMaxSteps = -10f;
    public float SuccessReward = 2f;
    public AnimationCurve RewardCurve = AnimationCurve.Linear(0, 1, 1, 0); // Use this to calculate normalized score
    public float ProgressScale = 100f;
    public float RewardCurveScale = 1f;
    public float OutOfBoundsPenalty = -5f;

    float TotalWidth = 10f;
    public float winDistance = 0.01f;
    public float maxSteps = 400;
    public bool debug = false;

    Vector2 xBoundsMinMax;
    Vector2 zBoundsMinMax;

    int currentStep = 0;

    public override void CollectObservations()
    {
        AddVectorObs(Marker.transform.position - transform.parent.position);
        AddVectorObs(Target.transform.position - transform.parent.position);
    }

    public override void InitializeAgent()
    {
        TotalWidth = WidthMinMax.y - WidthMinMax.x;
        TotalWidth = Mathf.Sqrt(2f) * TotalWidth; // Farthest distance is from corner to corner
        xBoundsMinMax = new Vector2((transform.parent.position.x + WidthMinMax.x) - (transform.lossyScale.x / 2f), (transform.parent.position.x + WidthMinMax.y) + (transform.lossyScale.x / 2f)); // Find how far the player can go
        zBoundsMinMax = new Vector2((transform.parent.position.z + WidthMinMax.x) - (transform.lossyScale.z / 2f), (transform.parent.position.z + WidthMinMax.y) + (transform.lossyScale.z / 2f)); // Arena is a square, so WidthMinMax is reused
    }

    // How to reinitialize when the game is reset. The Start() of an ML Agent
    public override void AgentReset()
    {
        Marker.transform.position = Marker.transform.parent.position + new Vector3(Random.Range(WidthMinMax.x, WidthMinMax.y), Marker.transform.position.y, Random.Range(WidthMinMax.x, WidthMinMax.y));
        Target.transform.position = Marker.transform.parent.position + new Vector3(Random.Range(WidthMinMax.x, WidthMinMax.y), Marker.transform.position.y, Random.Range(WidthMinMax.x, WidthMinMax.y));
        currentStep = 0;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        currentStep++;
        var action = new Vector3(Mathf.Clamp(vectorAction[0], -1f, 1f), 0, Mathf.Clamp(vectorAction[1], -1f, 1f));
        if (debug) Debug.Log("action: " + action);

        var startingDistance = Vector3.Distance(Marker.transform.position, Target.transform.position) / TotalWidth;
        Marker.transform.Translate(action * Time.deltaTime);
        var dist = Vector3.Distance(Marker.transform.position, Target.transform.position) / TotalWidth;
        var progress = (startingDistance - dist) * ProgressScale;
        if (debug) Debug.Log("progress: " + progress);
        if (dist < winDistance)
        {
            var remainingSteps = maxSteps - currentStep;
            var totalReward = SuccessReward + (remainingSteps * RewardCurveScale);
            AddReward(totalReward);
            if (debug) Debug.Log("Done, reward: " + totalReward);
            if (debug) Debug.Log("Total Reward: " + GetCumulativeReward());
            Done();
        }
        else
        {
            if (transform.position.x < xBoundsMinMax.x || transform.position.x > xBoundsMinMax.y || transform.position.z < zBoundsMinMax.x || transform.position.z > zBoundsMinMax.y)
            {
                AddReward(OutOfBoundsPenalty);
                if (debug) Debug.Log("Out of bounds, total reward:" + GetCumulativeReward());
                Done();
            }

            var reward = 0f;
            if (progress > 0)
            {
                reward = RewardCurve.Evaluate(progress) * RewardCurveScale; // Removed RewardCurve.Evaluate, to see if that helps
            }
            else
            {
                reward = RewardCurve.Evaluate(Mathf.Abs(progress)) * RewardCurveScale * -1.5f; // Went the wrong way? It's a penalty, not a reward
            }

            //if (debug) Debug.Log("reward: " + reward);
            AddReward(reward);
        }
    }
}
