using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeInputFloorScript : MonoBehaviour {

    public ThreeInputAgent MyAgent;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject == MyAgent.Marker)
        {
            if (col.relativeVelocity.magnitude > MyAgent.CollisionVelocityThreshold)
            {
                MyAgent.CollisionHappened();
            }
        }
    }
}
