using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAgent : Agent {

    public int maxNumbers = 5; // Be sure to change brain's Observation size and branch 0 size to match this
    List<float> numbers;
    

    private void Start()
    {
        numbers = new List<float>();
        for (var i = 0; i < maxNumbers; i++) {
            numbers.Add((float)i / (float)maxNumbers);
        }
        numbers[maxNumbers-1] = 1f;
        numbers = Shuffle(numbers);
    }
    public override void CollectObservations()
    {
        for (var i = 0; i < numbers.Count; i++)
        {
            AddVectorObs(numbers[i]);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Boolean reward
        var choice = (int)(vectorAction[0] * 10f);
        var rew = 0f;
        if (choice == 1f) rew = 1f;
        Debug.Log(rew);
        AddReward(rew);
        numbers = Shuffle(numbers); // shuffle test
    }

    public override void AgentReset()
    {
        //numbers = Shuffle(numbers);
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, list.Count - 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
