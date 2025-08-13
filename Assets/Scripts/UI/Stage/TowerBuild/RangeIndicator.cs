using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    [SerializeField] int segments = 64;
    [SerializeField] Transform followTarget; // 중심이 될 대상(없으면 자기 자신)
    [SerializeField] float radius;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = true;         // ★ 월드 공간
        lr.positionCount = segments;
        lr.widthMultiplier = 0.05f;      // 굵기는 월드 단위 감각 그대로
    }

    public void Attach(Transform t) => followTarget = t;
    public void SetRadius(float r) => radius = r;

    void LateUpdate()
    {
        // 중심(월드) 기준으로 원 좌표를 직접 세팅
        Vector3 c = followTarget ? followTarget.position : transform.position;

        for (int i = 0; i < segments; i++)
        {
            float t = (i / (float)segments) * Mathf.PI * 2f;
            lr.SetPosition(i, c + new Vector3(Mathf.Cos(t) * radius, 0f, Mathf.Sin(t) * radius));
        }
    }

    public void SetColor(Color c)
    {
        lr.startColor = lr.endColor = c;
    }
}
