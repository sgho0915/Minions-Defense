// WayPath.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 지정된 첫 Waypoint부터 next를 타고 쭉 순회해 배열로 리턴
/// </summary>
public class WayPath : MonoBehaviour
{
    [Tooltip("출발 지점이 되는 첫 Waypoint")]
    public Waypoint start;          // 시작 지점
    private Waypoint[] _waypoints;  // waypoint 배열
    public Waypoint[] Waypoints
    {
        get
        {
            if (_waypoints == null)
                _waypoints = BuildPath();
            return _waypoints;
        }
    }

    private Waypoint[] BuildPath()
    {
        var list = new List<Waypoint>();
        var cur = start;
        while (cur != null) // next가 없을 때까지
        {
            list.Add(cur);  // 현재 waypoint를 리스트에 추가
            cur = cur.next; // 다음 waypoint로 이동
        }
        return list.ToArray();  // 만들어진 waypoint 배열을 리턴
    }
}
