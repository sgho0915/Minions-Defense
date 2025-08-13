// TowerController.cs
using System.Linq;
using System.Collections;
using UnityEngine;

/// <summary>
/// Tower 동작 컨트롤러 (MVC 중 Controller)
/// ITower 구현체
/// TowerDataSO·TowerLevelData 읽어 공격 로직 수행
/// 레벨 변경, 공격 루프, 이펙트·사운드 연출, 판매 처리 담당
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TowerController : MonoBehaviour, ITower
{
    private TowerLevelData[] _allLevels;    // 전체 레벨 데이터 배열
    private TowerLevelData _curLevel;       // 현재 레벨 데이터
    private AudioSource _audio;
    private Coroutine _attackRoutine;

    [Header("공격 이펙트 포지션")]
    public Transform attackEffectPoint;

    /// <summary>
    /// 팩토리에서 초기 레벨 데이터와 전체 레벨 배열을 받아 초기화
    /// </summary>
    /// <param name="initialLevel">초기 레벨 데이터</param>
    /// <param name="allLevels">전체 레벨 데이터 배열</param>
    public void Initialize(TowerLevelData initialLevel, TowerLevelData[] allLevels)
    {
        _allLevels = allLevels;                     // 전체 레벨 배열 저장
        _audio = GetComponent<AudioSource>();
        _curLevel = initialLevel;                   // 초기 레벨 설정

        if (_curLevel.buildSoundClip != null)
            _audio.PlayOneShot(_curLevel.buildSoundClip);

        // 공격 루틴 시작
        _attackRoutine = StartCoroutine(AttackLoop());
    }

    /// <summary>
    /// 주어진 레벨과 브랜치 타입으로 업그레이드
    /// </summary>
    public void SetLevel(int level, TowerLevel4Type lv4Branch = TowerLevel4Type.None)
    {
        _curLevel = _allLevels
           .First(x => x.level == level && (level < 4 || x.level4Type == lv4Branch));

        // 업그레이드 연출 (Lv1 제외)
        if (level > 1 && _curLevel.upgradeSoundClip != null)
            _audio.PlayOneShot(_curLevel.upgradeSoundClip);

        // 공격 루틴 재시작
        if (_attackRoutine != null) StopCoroutine(_attackRoutine);
        _attackRoutine = StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        var wait = new WaitForSeconds(1f / _curLevel.attackSpeed);

        while (true)
        {
            // 대상 탐색 (타겟팅 우선순위 적용)
            var hits = Physics.OverlapSphere(transform.position, _curLevel.range, _curLevel.targetLayerMask);
            var enemies = hits
                .Where(c => _curLevel.targetTags.Contains(c.tag))
                .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                .ToArray();

            if(enemies.Length > 0)
            {
                var target = enemies[0].transform;

                attackEffectPoint.LookAt(target);

                // 투사체 사용 여부
                if (_curLevel.projectilePrefab != null)
                {
                    var projGO = Instantiate(_curLevel.projectilePrefab, attackEffectPoint.position, attackEffectPoint.rotation);

                    var bp = projGO.GetComponent<BallisticProjectile>() ?? projGO.AddComponent<BallisticProjectile>();

                    bp.Setup(_curLevel, target.transform.position);
                }
                else
                {
                    // 즉시 이펙트 + 데미지 처리 (AoE 포함)
                    HandleAoE(transform.position);
                }
            }

            if (_curLevel.attackSoundClip != null)
                _audio.PlayOneShot(_curLevel.attackSoundClip);

            yield return wait;
        }
    }

    private void HandleAoE(Vector3 point)
    {
        if (_curLevel.areaShape == TowerAreaShape.None) return;

        Collider[] colliders;
        if (_curLevel.areaShape == TowerAreaShape.Circle)
            colliders = Physics.OverlapSphere(point, _curLevel.splashRadius, _curLevel.targetLayerMask);
        else
            return; // Cone, Line 구현 생략

        foreach (var col in colliders)
        {
            if (_curLevel.targetTags.Contains(col.tag))
            {
                //col.GetComponent<MonsterController>().TakeDamage(_curLevel.damage);
            }
        }
    }

    public int Sell()
    {
        // 파괴 연출
        if (_curLevel.destroyEffect != null)
            Instantiate(_curLevel.destroyEffect, transform.position, Quaternion.identity);
        if (_curLevel.destroySoundClip != null)
            _audio.PlayOneShot(_curLevel.destroySoundClip);

        Destroy(gameObject);
        return _curLevel.sellPrice;
    }

    // 디버그용: 에디터 상에서 공격 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _curLevel.range);
    }
}