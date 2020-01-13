using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAliveGameTwo : MonoBehaviour
{

    public GameObject Player;
    public List<GameObject> Bullets;
    public bool AimAtPlayer = false;
    public float EnvironmentWidth = 40;
    public float EnvironmentHeight = 20;
    public float MinBulletSpeed = 2f;
    public float MaxBulletSpeed = 10f;
    public float MinBulletLifetime = 3f;
    public float MaxBulletLifetime = 10f;

    List<float> BulletLives = new List<float>();
    List<Rigidbody> bulletRbs = new List<Rigidbody>();

    public float ValidStartXMin { get { return transform.position.x + (-EnvironmentWidth / 2f) + 1f; } }
    public float ValidStartXMax { get { return transform.position.x + (EnvironmentWidth / 2f) - 1f; } }
    public float ValidStartYMin { get { return transform.position.y + (-EnvironmentHeight / 2f) + 1f; } }
    public float ValidStartYMax { get { return transform.position.y + (EnvironmentHeight / 2f) - 1f; } }

    private void Start()
    {
        for (var i = 0; i < Bullets.Count; i++)
        {
            bulletRbs.Add(Bullets[i].GetComponent<Rigidbody>());
            BulletLives.Add(Random.Range(MinBulletLifetime, MaxBulletLifetime));
        }

        ResetAllBullets();
    }

    private void resetBullet(int idx)
    {
        var bulletRb = bulletRbs[idx];
        bulletRb.angularVelocity = Vector3.zero;
        bulletRb.transform.position = new Vector3(Random.Range(ValidStartXMin, ValidStartXMax), Random.Range(ValidStartYMin, ValidStartYMax), 0f);
        BulletLives[idx] = Random.Range(MinBulletLifetime, MaxBulletLifetime);

        if (AimAtPlayer)
        {
            bulletRb.velocity = (Player.transform.position - bulletRb.transform.position).normalized * Random.Range(MinBulletSpeed, MaxBulletSpeed);
        }
        else
        {
            var dx = Random.insideUnitCircle.normalized;
            bulletRb.velocity = new Vector3(dx.x, dx.y, 0f) * Random.Range(MinBulletSpeed, MaxBulletSpeed);
        }
    }

    public List<Rigidbody> GetBulletRigidBodies()
    {
        return bulletRbs;
    }

    public void ResetAllBullets()
    {
        for (var i = 0; i < BulletLives.Count; i++)
        {
            resetBullet(i);
        }
    }

    private void FixedUpdate()
    {
        for (var i = 0; i < BulletLives.Count; i++)
        {
            BulletLives[i] -= Time.fixedDeltaTime;
            if (BulletLives[i] < 0)
            {
                resetBullet(i);
            }
        }
    }
}
