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
    public void SetPath(Waypoint[] path, MonsterLevelData levelData)
    {
        _path = path;
        _levelData = levelData;
        _anim = GetComponent<Animator>();
        //StartCoroutine(MoveAlongPath());
    }

    // Waypoint 배열을 순회하며 몬스터를 이동
    public IEnumerator MoveAlongPath()
    {
        _anim.SetBool("IsMoving", true);

        for (int i = 0; i < _path.Length; i++)
        {
            var target = _path[i].transform.position;

            // 회전 (방향 보정)
            Vector3 dir = (target - transform.position).normalized;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);

            // 목표 지점까지 이동
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    _levelData.moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // 마지막엔 Idle 이라든가 Die로 전이하고 Destroy
        _anim.SetBool("IsMoving", false);
        yield break;
    }
}