using MLAgents;
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

    public override void AgentAction(float[] vectorAction)
    {
        var discreteAction = (int)vectorAction[0];
        //Debug.Log(discreteAction);
        if (discreteAction == 1 && subject.activeInHierarchy == true) Correct(true);
        if (discreteAction == 1 && subject.activeInHierarchy == false) Incorrect(true);
        if (discreteAction == 0 && subject.activeInHierarchy == true) Incorrect(false);
        if (discreteAction == 0 && subject.activeInHierarchy == false) Correct(false);
    }

    void Correct(bool action)
    {
        AddReward(correctReward);
        Debug.Log("Correct: " + action + " when " + action);
    }

    void Incorrect(bool action)
    {
        AddReward(wrongPenalty);
        Debug.Log("Incorrect: " + action + " when " + !action);
    }

    public override void AgentReset()
    {
        var isShown = Random.Range(0, 2);
        if (isShown == 0) subject.SetActive(false);
        else subject.SetActive(true);
    }
}
