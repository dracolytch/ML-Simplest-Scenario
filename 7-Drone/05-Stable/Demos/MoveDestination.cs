using UnityEngine;

public class MoveDestination : MonoBehaviour
{
    public float bobSpeed = 1f;
    public float bobOffset = 0f;
    public float bobAmplitude = 1f;
    const float TwoPi = Mathf.PI * 2f;
    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        var zMove = Mathf.Sin(bobOffset + (Time.timeSinceLevelLoad * TwoPi * bobSpeed)) * bobAmplitude;
        transform.position = startPosition + new Vector3(0f, 0f, zMove);
    }
}