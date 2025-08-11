// RangeIndicator.cs
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    [SerializeField] int segments = 64;
    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = false; // 로컬 원
        lr.positionCount = segments;
        lr.widthMultiplier = 0.05f;
    }

    public void SetRadius(float r)
    {
        for (int i = 0; i < segments; i++)
        {
            float t = (i / (float)segments) * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(Mathf.Cos(t) * r, 0f, Mathf.Sin(t) * r));
        }
    }

    public void SetColor(Color c)
    {
        lr.startColor = lr.endColor = c;
    }
}
