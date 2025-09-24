using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelManager Manager;
    [SerializeField] internal GameObject levelObj;
    public List<Transform> nearPoints;
    Bounds levelObjectBounds;
    public Bounds LevelObjectBounds => levelObjectBounds;

    private void Start()
    {
        CreateCustomBounds();
    }

    public Transform GetNearestPoint(Vector3 touchpos)
    {
        float minSqrDist = float.MaxValue;
        Transform nearestPoint = null;

        for (int i = 0; i < nearPoints.Count; i++)
        {
            float sqrDist = (nearPoints[i].position - touchpos).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearestPoint = nearPoints[i];
            }
        }

        return nearestPoint;
    }

    [ContextMenu("CreateBounds")]
    void CreateCustomBounds()
    {
        if (nearPoints == null || nearPoints.Count == 0)
            levelObjectBounds = new Bounds(Vector3.zero, Vector3.zero);

        levelObjectBounds = new Bounds(nearPoints[0].position, new Vector3(1, 0.2f, 1));
        foreach (Transform t in nearPoints)
            LevelObjectBounds.Encapsulate(t.position);

    }

    void OnDrawGizmos()
    {
        if (nearPoints == null || nearPoints.Count == 0) return;

        // Draw the bounding box
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(LevelObjectBounds.center, LevelObjectBounds.size);

        // (Optional) Draw spheres at the points for clarity
        Gizmos.color = Color.red;
        foreach (Transform t in nearPoints)
            Gizmos.DrawSphere(t.position, 0.1f);
    }
}
