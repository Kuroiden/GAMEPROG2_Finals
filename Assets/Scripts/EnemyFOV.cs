using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFOV : MonoBehaviour
{
    [Header("Linked Scripts")]
    public AIManager aiManager;
    public Enemy enemy;

    [Header("Components")]
    public LayerMask obstacles;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    //public GameObject lineOfSight;

    [Header("Parameters")]
    public float viewRad;
    [Range(0, 360)]
    public float viewAng;
    public float meshResolution;

    public List<Transform> foundPlayer = new List<Transform>();

    void Start()
    {
        enemy = this.GetComponent<Enemy>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindPlayerRoutine", 0.1f);

    }
    void LateUpdate()
    {
        DrawFOV();
    }
    public IEnumerator FindPlayerRoutine(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            FindPlayer();
        }
    }

    public Vector3 angleDir(float angleInDeg, bool angleIsGlobal)
    {
        if (!angleIsGlobal) angleInDeg += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDeg * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = angleDir(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRad, obstacles)) return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        else return new ViewCastInfo(false, transform.position + dir * viewRad, viewRad, globalAngle);
    }

    void DrawFOV()
    {
        int stepCount = Mathf.RoundToInt(viewAng * meshResolution);
        float stepAngle = viewAng / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAng / 2 + stepAngle * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
            //Debug.DrawLine(transform.position, transform.position + angleDir(angle, true) * viewRad, Color.red);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] faces = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                faces[i * 3] = 0;
                faces[i * 3 + 1] = i + 1;
                faces[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = faces;
        viewMesh.RecalculateNormals();
    }

    void FindPlayer()
    {
        foundPlayer.Clear();
        //enemy_Move.Stop();
        enemy.enemy_Move.ResetPath();
        //if (enemy.isHostile) enemy.enemy_Move.ResetPath();
        //enemy.isHostile = false;
        
        Collider[] playerInView = Physics.OverlapSphere(transform.position, viewRad);

        for (int x = 0; x < playerInView.Length; x++)
        {
            Transform target = playerInView[x].transform;
            Vector3 targetDir = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, targetDir) < viewAng / 2)
            {
                float targetDist = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, targetDir, targetDist, obstacles) && target.CompareTag("Player"))
                {
                    foundPlayer.Add(target);
                }
            }
        }
    }
}
