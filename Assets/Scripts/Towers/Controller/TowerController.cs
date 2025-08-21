// TowerController.cs
using System.Linq;
using System.Collections;
using UnityEngine;
using System;

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

    public TowerLevelData CurrentLevelData => _curLevel;   // 읽기 전용 참조 캡슐화
    public int CurrentLevelIndex => _curLevel != null ? _curLevel.level : 0;    // 현재 타워 레벨 읽기 전용 참조


    [Header("TowerInfoView에서 사용 요소")]
    public TowerDataSO TowerDataSO {  get; private set; }

    [Header("공격 이펙트 포지션")]
    public Transform attackEffectPoint;

    public event Action<TowerController, TowerController> OnTowerUpgraded;

    #region 타워 초기화 로직
    /// <summary>
    /// 팩토리에서 초기 레벨 데이터와 전체 레벨 배열을 받아 초기화
    /// </summary>
    /// <param name="initialLevel">초기 레벨 데이터</param>
    /// <param name="allLevels">전체 레벨 데이터 배열</param>
    public void Initialize(TowerLevelData initialLevel, TowerLevelData[] allLevels)
        => Initialize(null, initialLevel, allLevels);


    /// <summary>
    /// TowerDataSO까지 받도록 팩토리에서 호출할 Overload Initialize 함수
    /// </summary>
    public void Initialize(TowerDataSO towerDataSO, TowerLevelData initialLevel, TowerLevelData[] allLevels)
    {
        TowerDataSO = towerDataSO;
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
#endregion


    #region 타워 강화 로직
    /// <summary>
    /// 다음 레벨 데이터를 반환
    /// </summary>
    public TowerLevelData GetNextLevelData()
        => _allLevels?.FirstOrDefault(x => x.level == _curLevel.level + 1);


    /// <summary>
    /// 업그레이드 가능여부 반환
    /// </summary>
    public bool CanUpgrade(out TowerLevelData nextData, out int cost)
    {
        nextData = GetNextLevelData();
        cost = (nextData != null) ? nextData.upgradeCost : 0;
        return nextData != null;
    }


    /// <summary>
    /// 배치된 현재 타워를 강화하면서 전체 프리팹 스왑
    /// </summary>
    public bool TryUpgrade()
    {
        // 더이상 업그레이드가 불가능한 레벨이거나
        // 스테이지 포인트가 부족한 경우 업그레이드 불가
        if(!CanUpgrade(out var next, out var cost)) return false;
        if(!GameManager.Instance.TrySpendStagePoints(cost)) return false;

        // 현재 배치된 타워의 생성 위치 / 부모 / 회전 값 보존
        var parent = transform.parent;
        var pos = transform.position;
        var rot = transform.rotation;

        // 다음 레벨에 대한 새 타워 프리팹 생성
        var newTower = Instantiate(next.towerPrefab, pos, rot, parent);

        // 새 타워에 대한 TowerController 컴포넌트 보장
        var newController = newTower.GetComponent<TowerController>();
        if (newController == null) newController = newTower.AddComponent<TowerController>();

        // 동일한 DataSO / 레벨 배열로 초기화
        newController.Initialize(TowerDataSO, next, _allLevels);

        // 업그레이드 사운드
        if (next.upgradeSoundClip != null)
            newController.GetComponent<AudioSource>()?.PlayOneShot(next.upgradeSoundClip);

        // TowerInfoView에 업그레이드 완료 이벤트 발행
        OnTowerUpgraded?.Invoke(this, newController);

        // 기존 타워 제거
        Destroy(gameObject);
        return true;
    }
    #endregion


    #region 타워 판매 로직
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
    #endregion


    #region 몬스터 공격 로직
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
    #endregion

    

    // 디버그용: 에디터 상에서 공격 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _curLevel.range);
    }
}