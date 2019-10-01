using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public struct ViewCastInfo
    {
        //public bool hit;
        public Collider thingHit;
        public Vector3 point;
        public float distance;
        public float angle;
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;
    }

    // This is a great resource:
    // https://www.youtube.com/watch?v=rQG9aUWarwE

    public float range;
    [Range(0, 360)]
    public float angle = 45;
    public float resolution = 1;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public MeshFilter myMeshFilter;
    Mesh viewMesh;
    public int EdgeResolveIterations = 5;

    public List<Transform> visibleTargets = new List<Transform>();

    IEnumerator FindTargetsWithDelayCo(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        var targetsInView = Physics.OverlapSphere(transform.position, angle, targetMask);

        for (var i = 0; i < targetsInView.Length; i++)
        {
            var target = targetsInView[i].transform;
            var directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                var distanceToGarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToGarget, obstacleMask))
                {
                    // No obstacles
                    visibleTargets.Add(target);
                }
            }
        }

    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        var minAngle = minViewCast.angle;
        var maxAngle = maxViewCast.angle;

        var minPoint = Vector3.zero;
        var maxPoint = Vector3.zero;

        for (var i = 0; i < EdgeResolveIterations; i++)
        {
            var angle = (minAngle + maxAngle) / 2f;
            var newViewCast = ViewCast(angle);

            if (newViewCast.thingHit == minViewCast.thingHit)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
            
        }

        return new EdgeInfo { pointA = minPoint, pointB = maxPoint };
    }

    void DrawFieldOfView()
    {
        var steps = Mathf.RoundToInt(angle * resolution);
        var stepAngleSize = angle / steps;
        var viewPoints = new List<Vector3>();
        var oldViewCast = new ViewCastInfo();


        for (var i = 0; i <= steps; i++)
        {
            var loopAngle = transform.eulerAngles.y - angle / 2f + stepAngleSize * i;
            var viewCast = ViewCast(loopAngle);
            
            // Check to see if we've found an edge
            if (i > 0)
            {
                if (oldViewCast.thingHit != viewCast.thingHit)
                {
                    var edge = FindEdge(oldViewCast, viewCast);
                    if (edge.pointA != Vector3.zero) viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero) viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(viewCast.point);
            oldViewCast = viewCast;
        }

        var vertexCount = viewPoints.Count + 1;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        // All in local space
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        var dir = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, range, obstacleMask))
        {
            return new ViewCastInfo() { thingHit = hit.collider, point = hit.point, distance = hit.distance, angle = globalAngle };
        }
        else {
            return new ViewCastInfo() { thingHit = null, point = transform.position + dir * range, distance = range, angle = globalAngle };
        }
    }



    public Vector3 DirectionFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (angleIsGlobal == false)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

	// Use this for initialization
	void Start () {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        myMeshFilter.mesh = viewMesh;

        StartCoroutine(FindTargetsWithDelayCo(0.2f));
	}

    private void LateUpdate()
    {
        DrawFieldOfView();
    }
}
