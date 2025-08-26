// MagicPoePatroller.cs
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// “마법의 포댕이” 스킬 이동속도, 공격패턴, 소멸 등 유닛 자체의 행동 관리 로직
/// </summary>
public class MagicPoePatroller : MonoBehaviour
{
    private MagicPoeLevelData _levelData;
    private Waypoint[] _reversedPath;
    private int _currentPathIndex = 0;
    private float _totalDamageDealt = 0;
    private float _tickTimer = 0f;

    public void Setup(MagicPoeLevelData levelData, Waypoint[] originalPath)
    {
        _levelData = levelData;

        // 경로를 복사한뒤 역주행 경로를 만듬
        _reversedPath = originalPath.ToArray();
        System.Array.Reverse(_reversedPath);

        // 오라 이펙트를 자식으로 붙임
        if (_levelData.auraEffectPrefab != null)
            Instantiate(_levelData.auraEffectPrefab, transform.position, transform.rotation, transform);

        // 메인 로직 코루틴 시작
        StartCoroutine(PatrolAndAttackLoop());
    }

    private IEnumerator PatrolAndAttackLoop()
    {
        while (true)
        {
            HandleMovement();
            HandleAuraAttack();

            // 소멸조건 1) 최대 피해량 한도 채웠거나
            // 소멸조건 2) 경로의 끝(몬스터 스폰지점)에 도달했거나
            if (_totalDamageDealt >= _levelData.maxDamageOutput || _currentPathIndex >= _reversedPath.Length)
            {
                break;  // 루프 탈출 후 소멸
            }

            yield return null;  // 다음 프레임까지 대기
        }
    }

    private void HandleMovement()
    {
        if (_currentPathIndex >= _reversedPath.Length) return;

        // 현재 목표 지점 설정
        Vector3 targetPosition = _reversedPath[_currentPathIndex].transform.position;

        // 목표를 향해 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _levelData.moveSpeed * Time.deltaTime);

        // 방향 전환
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 목표 지점에 거의 도달했으면 다음 인덱스로
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            _currentPathIndex++;
        }
    }

    private void HandleAuraAttack()
    {
        _tickTimer += Time.deltaTime;
        float tickInterval = 1f / _levelData.damageTickRate;

        // 정해진 공격주기가 됐는지 확인
        if (_tickTimer >= tickInterval)
        {
            _tickTimer -= tickInterval;

            // OverlapSphere를 사용해 주변의 모든 콜라이더를 찾음
            Collider[] colliders = Physics.OverlapSphere(transform.position, _levelData.auraRadius);

            foreach (var col in colliders)
            {
                if (col.CompareTag("Monster"))
                {
                    MonsterController monster = col.GetComponent<MonsterController>();
                    if (monster != null)
                    {
                        // 데미지 적용
                        monster.ApplyDamage(_levelData.damagePerTick);
                        _totalDamageDealt += _levelData.damagePerTick;

                        // 스턴 적용
                        monster.Stun(_levelData.stunDuration);

                        // 이펙트, 사운드
                    }
                }
            }
        }
    }

    private void Despawn()
    {
        // 소멸 이펙트, 사운드
        Destroy(gameObject);
    }
}
