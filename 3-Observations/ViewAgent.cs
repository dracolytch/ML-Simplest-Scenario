using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAgent : Agent
{
    public float correctReward;
    public float wrongPenalty;
    public GameObject subject;

    private void Start()
    {
        AgentReset();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var discreteAction = (int)vectorAction[0];
        //Debug.Log(discreteAction);
        if (discreteAction == 1 && subject.activeInHierarchy == true) AddReward(correctReward);
        if (discreteAction == 1 && subject.activeInHierarchy == false) AddReward(wrongPenalty);
        if (discreteAction == 0 && subject.activeInHierarchy == true) AddReward(wrongPenalty);
        if (discreteAction == 0 && subject.activeInHierarchy == false) AddReward(correctReward);
    }

    public override void AgentReset()
    {
        var isShown = Random.Range(0, 2);
        if (isShown == 0) subject.SetActive(false);
        else subject.SetActive(true);
    }
}
