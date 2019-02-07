using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeInputCurriculumAcademy : Academy {

    [Header("Specific to Three Input Lander")]
    public float DistanceScale = 0.4f;
    public float OnCollisionPoints = -1f;

    public override void AcademyReset()
    {
        DistanceScale = resetParameters["DistanceScale"];
        OnCollisionPoints = resetParameters["OnCollisionPoints"];
    }

    public override void AcademyStep()
    {
    }
}
