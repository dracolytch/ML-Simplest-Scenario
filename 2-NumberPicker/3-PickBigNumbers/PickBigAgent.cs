using MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PickBigAgent : Agent {

    public int maxNumbers = 5; // Be sure to change brain's Observation size and branch 0 size to match this
    public float scaleFactor = 1;
    List<float> numbers;

    float targetNumber = 0;

    private void Start()
    {
        if (File.Exists("output.txt")) File.Delete("output.txt");

        numbers = new List<float>();
        for (var i = 0; i < maxNumbers; i++) {
            numbers.Add(i * scaleFactor);
        }
        targetNumber = numbers[maxNumbers-1];
        numbers = Shuffle(numbers);
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
        // Boolean reward

        var choice = (int)vectorAction[0];
        var rew = 0f;
        if (numbers[choice] == targetNumber) rew = 1f;
        using (var sw = File.AppendText("output.txt"))
        {
            sw.WriteLine(choice);
        }
        //Debug.Log("Reward:" + rew);
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
