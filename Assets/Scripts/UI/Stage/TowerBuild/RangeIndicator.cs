// RangeIndicator.cs
using UnityEngine;

/// <summary>
/// 타워 사거리 표시 링.
/// LineRenderer를 로컬공간(useWorldSpace=false)에서 원형으로 세팅.
/// 반지름 변경 시 원 점들을 재샘플링.
/// </summary>[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    [SerializeField] int segments = 64; // 원 분할(클수록 매끈)
    LineRenderer lr;


    /// <summary>라인렌더러 초기화(로컬 공간 원형, 두께 등)</summary>
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;             // 닫힌 원
        lr.useWorldSpace = false;   // 로컬 좌표 → 오브젝트와 함께 이동/회전
        lr.positionCount = segments;
        lr.widthMultiplier = 0.05f;
    }


    /// <summary>반지름 r로 원형 점 재계산</summary>
    public void SetRadius(float r)
    {
        for (int i = 0; i < segments; i++)
        {
            float t = (i / (float)segments) * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(Mathf.Cos(t) * r, 0f, Mathf.Sin(t) * r));
        }
    }


    /// <summary>시작/끝 컬러 동기화(단색 원)</summary>
    public void SetColor(Color c)
    {
        lr.startColor = lr.endColor = c;
    }
}
