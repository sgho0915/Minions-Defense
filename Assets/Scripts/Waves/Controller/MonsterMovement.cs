// MonsterMovement.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// 웨이포인트 경로를 따라 몬스터를 이동시키는 로직
/// </summary>
[RequireComponent(typeof(Animator))]
public class MonsterMovement : MonoBehaviour
{
    private Waypoint[] _path;
    private int _idx = 0;
    private Animator _anim;
    private MonsterLevelData _levelData;

    // 이동 경로(Path)를 설정하고, 이동 코루틴을 시작
    public void SetPath(Waypoint[] path)
    {
        _path = path;
        _anim = GetComponent<Animator>();
        StartCoroutine(MoveAlongPath());
    }

    // Waypoint 배열을 순회하며 몬스터를 이동
    private IEnumerator MoveAlongPath()
    {
        // 모든 웨이포인트를 순서대로 방문할 때까지 반복
        while (_idx < _path.Length)
        {
            // 현재 목표 지점 획득
            var target = _path[_idx].transform.position;
            // 이동 애니메이션 재생
            _anim.Play(_levelData.moveAnim.name);

            // 목표 지점까지 일정 속도로 이동
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    _levelData.moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 다음 웨이포인트로 인덱스 증가
            _idx++;
        }

        // 마지막 웨이포인트 도착 시 처리 (예: 본진 공격, 오브젝트 제거 등)
        Destroy(gameObject);
    }
}