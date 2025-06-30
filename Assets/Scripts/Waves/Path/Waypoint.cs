// Waypoint.cs
using UnityEngine;

/// <summary>
/// 씬에 배치해서 다음 Waypoint로 연결할 수 있는 단일 waypoint 컴포넌트
/// </summary>
public class Waypoint : MonoBehaviour
{
    [Tooltip("다음 Waypoint (null이면 마지막)")]
    public Waypoint next;
}
