using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickNumbersDistance : Agent {

    public int maxNumbers = 5; // Be sure to change brain's Observation size and branch 0 size to match this
    List<float> numbers;
    List<float> distances;


    private void Start()
    {
        numbers = new List<float>();
        distances = new List<float>();
        for (var i = 0; i < maxNumbers; i++)
        {
            numbers.Add((float)i);
            distances.Add(Random.Range(0.8f, 1.2f) * i); // Add some randomness here
        }

        numbers = Shuffle(numbers);
        distances = Shuffle(distances);
    }
    public override void CollectObservations()
    {
        for (var i = 0; i < numbers.Count; i++)
        {
            AddVectorObs(numbers[i]);
        }

        for (var i = 0; i < distances.Count; i++)
        {
            AddVectorObs(distances[i]);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Which is best? beats me, let's track
        var max = float.NegativeInfinity;
        var answers = new List<int>(); // could be more than one answer

        for (var i = 0; i < numbers.Count; i++)
        {
            var val = numbers[i] * distances[i];
            if (val == max) // Could happen?
            {
                answers.Add(i);
            }
            if (val > max) // new best
            {
                answers = new List<int>();
                answers.Add(i);
                max = val;
            }
        }

        // Boolean reward
        var choice = (int)vectorAction[0];
        var rew = 0f;
        if (answers.Contains(choice)) rew = 1f;
        /*
        var s = "";
        foreach (var a in answers)
        {
            s = s + a + " ";
        }
        Debug.Log("picked:" + choice + " winning options were " + s);
        */
        AddReward(rew);

        numbers = Shuffle(numbers); // shuffle test
        distances = Shuffle(distances);
    }

    public override void AgentReset()
    {
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
