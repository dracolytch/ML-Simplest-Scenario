using MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomAgent : Agent {

    public override void CollectObservations()
    {
        AddVectorObs(Random.Range(0f, 1f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        using (var sw = File.AppendText("actions.txt"))
        {
            sw.WriteLine(vectorAction[0]);
        }
        AddReward(1);
    }
}
