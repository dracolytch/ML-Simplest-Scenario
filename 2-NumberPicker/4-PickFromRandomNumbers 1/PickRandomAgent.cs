using MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PickRandomAgent : Agent {

    public int maxNumbers = 5; // Be sure to change brain's Observation size and branch 0 size to match this
    public float scaleFactor = 1;
    List<float> numbers;

    float targetNumber = 0;

    private void Start()
    {
        GetNewNumbers();
    }

    public override void CollectObservations()
    {
        for (var i = 0; i < numbers.Count; i++)
        {
            AddVectorObs(numbers[i]);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var choice = (int)vectorAction[0];
        var rew = 0f;
        if (numbers[choice] == targetNumber) rew = 1f;
        AddReward(rew);
        GetNewNumbers(); 
    }

    public void GetNewNumbers()
    {
        numbers = new List<float>();
        targetNumber = float.NegativeInfinity;
        for (var i = 0; i < maxNumbers; i++)
        {
            var num = Random.Range(0f, scaleFactor);
            numbers.Add(num);
            if (num > targetNumber) targetNumber = num;
        }
    }
 
}
