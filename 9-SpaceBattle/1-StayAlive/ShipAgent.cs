using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAgent : Agent
{
    public StayAliveGame Environment;
    public float TurnSpeed = 1f;
    public float AccelerateSpeed = 1f;
    public float FrameSurvivedReward = 0.01f;
    public float DestroyedReward = -1f;
    public float NumBullets = 6f; // Remember to adjust observations accordingly
    public bool AimAtPlayer = false;
    Rigidbody rb;
    List<Rigidbody> bulletRbs = new List<Rigidbody>();
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();

        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Need a rigidBody");

        for (var i = 0; i < NumBullets; i++) {
            bulletRbs.Add(Environment.Bullets[i].GetComponent<Rigidbody>());
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(Environment.transform.position.x - transform.position.x);
        AddVectorObs(Environment.transform.position.y - transform.position.y);
        AddVectorObs(transform.rotation.eulerAngles.z);

        AddVectorObs(rb.velocity.x);
        AddVectorObs(rb.velocity.y);
        AddVectorObs(rb.angularVelocity.z);

        foreach (var bulletRb in bulletRbs)
        {
            AddVectorObs(Environment.transform.position.x - bulletRb.transform.position.x);
            AddVectorObs(Environment.transform.position.y - bulletRb.transform.position.y);

            AddVectorObs(bulletRb.velocity.x);
            AddVectorObs(bulletRb.velocity.y);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody.gameObject.tag == "ball")
        {
            AddReward(DestroyedReward);
            Done();
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        // 0 no thrust/thrust, 1 turn none/left/right
        var thrust = Mathf.FloorToInt(vectorAction[0]);
        var turn = Mathf.FloorToInt(vectorAction[1]);

        if (ps != null)
        {
            var emission = ps.emission;
            if (thrust == 1) emission.rateOverTime = 100;
            else emission.rateOverTime = 0;
        }

        if (thrust == 1)
        {
            rb.AddForce(transform.right * AccelerateSpeed);
        }

        switch (turn)
        {
            case 1:
                rb.angularVelocity += new Vector3(0, 0, TurnSpeed);
                break;

            case 2:
                rb.angularVelocity += new Vector3(0, 0, -TurnSpeed);
                break;
        }
        AddReward(FrameSurvivedReward);
    }


    public override void AgentReset()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = Environment.transform.position + new Vector3(Random.Range(-9f, 9f), Random.Range(-9f, 9f), 0f);

        foreach (var bulletRb in bulletRbs)
        {
            bulletRb.angularVelocity = Vector3.zero;
            var dx = Random.insideUnitCircle.normalized;
            bulletRb.transform.position = Environment.transform.position + new Vector3(Random.Range(-9f, 9f), Random.Range(-9f, 9f), 0f);
            if (AimAtPlayer)
            {
                bulletRb.velocity = (transform.position - bulletRb.transform.position).normalized * Random.Range(2f, 10f);
            }
            else
            {
                bulletRb.velocity = new Vector3(dx.x, dx.y, 0f) * Random.Range(2f, 10f);
            }
        }
    }
}
