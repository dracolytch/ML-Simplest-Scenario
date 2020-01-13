using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Vector3 TeleportAmount;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position += TeleportAmount;
    }
}
