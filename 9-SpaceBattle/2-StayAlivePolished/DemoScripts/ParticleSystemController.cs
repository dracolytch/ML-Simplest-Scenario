using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public ParticleSystem ControlledSystem;
    public int EmissionRate;
    public bool IsEmitting;

    public void StartEmitting()
    {
        IsEmitting = true;
    }

    public void StopEmitting()
    {
        IsEmitting = false;
    }

    // Update is called once per frame
    void Update()
    {
        var emission = ControlledSystem.emission;
        if (IsEmitting) emission.rateOverTime = EmissionRate;
        else emission.rateOverTime = 0;
    }
}
