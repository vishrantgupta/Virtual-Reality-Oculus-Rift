// MeasureSpaceAround.cs
using UnityEngine;
using System.Collections;
public class MeasureSpaceAround : MonoBehaviour
{
    void DrawCircle(Vector3 center, float radius, Color color)
    {
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 0; i < 30; i++)
        {
            float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Debug.DrawLine(prevPos, newPos, color);
            prevPos = newPos;
        }
    }
    void Update()
    {
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.FindClosestEdge(transform.position, out hit, UnityEngine.AI.NavMesh.AllAreas))
        {
            DrawCircle(transform.position, hit.distance, Color.cyan);
            Debug.DrawRay(hit.position, Vector3.up, Color.cyan);
        }
    }
}
