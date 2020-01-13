using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipAgentTwo : Agent
{
    public StayAliveGameTwo Environment;
    public float TurnSpeed = 1f;
    public float AccelerateSpeed = 1f;
    public float FrameSurvivedReward = 0.01f;
    public float DestroyedReward = -1f;
    public bool EmitEvents = false;

    public UnityEvent OnStraight = new UnityEvent();
    public UnityEvent OnLeft = new UnityEvent();
    public UnityEvent OnRight = new UnityEvent();
    public UnityEvent OnNoThrust = new UnityEvent();
    public UnityEvent OnThrust = new UnityEvent();
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent OnDie = new UnityEvent();

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Need a rigidBody");
    }

    public override void CollectObservations()
    {
        // Normalize inputs
        var halfWidth = Environment.EnvironmentWidth / 2f;
        var halfHeight = Environment.EnvironmentHeight / 2f;
        AddVectorObs((Environment.transform.position.x - transform.position.x) / halfWidth);
        AddVectorObs((Environment.transform.position.y - transform.position.y) / halfHeight);
        AddVectorObs((transform.rotation.eulerAngles.z) / 360f);

        AddVectorObs(rb.velocity.x / Environment.MaxBulletSpeed);
        AddVectorObs(rb.velocity.y / Environment.MaxBulletSpeed);
        AddVectorObs(rb.angularVelocity.z / 360f);

        var bulletRbs = Environment.GetBulletRigidBodies();
        foreach (var bulletRb in bulletRbs)
        {
            AddVectorObs((Environment.transform.position.x - bulletRb.transform.position.x) / halfWidth);
            AddVectorObs((Environment.transform.position.y - bulletRb.transform.position.y) / halfHeight);

            AddVectorObs(bulletRb.velocity.x / Environment.MaxBulletSpeed);
            AddVectorObs(bulletRb.velocity.y / Environment.MaxBulletSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody.gameObject.tag == "ball")
        {
            AddReward(DestroyedReward);
            if (EmitEvents && OnDie != null) OnDie.Invoke();
            Done();
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        // 0 = [0 no thrust, 1 thrust]
        // 1 = [0 no turn, 1 left, 2 right]
        // 2 = [0 no shoot, 1 shoot]
        var thrust = Mathf.FloorToInt(vectorAction[0]);
        var turn = Mathf.FloorToInt(vectorAction[1]);
        var shoot = Mathf.FloorToInt(vectorAction[2]);

        if (EmitEvents)
        {
            if (thrust == 1 && OnThrust != null) OnThrust.Invoke();
            if (thrust == 0 && OnNoThrust != null) OnNoThrust.Invoke();
            if (turn == 1 && OnLeft != null) OnLeft.Invoke();
            if (turn == 2 && OnRight != null) OnRight.Invoke();
            if (shoot == 1 && OnShoot != null) OnShoot.Invoke();
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
        transform.position = Environment.transform.position + new Vector3(Random.Range(Environment.ValidStartXMin, Environment.ValidStartXMax), Random.Range(Environment.ValidStartYMin, Environment.ValidStartYMax), 0f);
        Environment.ResetAllBullets();
    }
}
