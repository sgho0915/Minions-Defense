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
    private TowerDataSO _data;
    private TowerLevelData _curLevel;
    private AudioSource _audio;
    private Coroutine _attackRoutine;

    public void Initialize(TowerDataSO data)
    {
        _data = data;
        _audio = GetComponent<AudioSource>();

        // 설치 연출
        if (_curLevel == null)
        {
            if (_data.levelData[0].buildEffect != null)
                Instantiate(_data.levelData[0].buildEffect, transform.position, Quaternion.identity);
            if (_data.levelData[0].buildSoundClip != null)
                _audio.PlayOneShot(_data.levelData[0].buildSoundClip);
        }

        SetLevel(1);
    }

    public void SetLevel(int level, TowerLevel4Type branch = TowerLevel4Type.None)
    {
        _curLevel = _data.levelData
            .First(x => x.level == level && (level < 4 || x.level4Type == branch));

        // 업그레이드 연출 (Lv1 제외)
        if (level > 1)
        {
            if (_curLevel.upgradeSoundClip != null)
                _audio.PlayOneShot(_curLevel.upgradeSoundClip);
        }

        // 공격 루틴 재시작
        if (_attackRoutine != null) StopCoroutine(_attackRoutine);
        _attackRoutine = StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / _curLevel.attackSpeed);

            // 대상 탐색 (타겟팅 우선순위 적용)
            var hits = Physics.OverlapSphere(transform.position, _curLevel.range, _curLevel.targetLayerMask);
            var enemies = hits
                .Where(c => _curLevel.targetTags.Contains(c.tag))
                .Select(c => c.GetComponent<MonoBehaviour>()) // EnemyController 등으로 캐스팅
                .ToList();
            if (enemies.Count == 0) continue;

            // 우선순위별 선택 (예: First)
            var target = enemies[0];

            // 투사체 사용 여부
            if (_curLevel.projectilePrefab != null)
            {
                var proj = Instantiate(_curLevel.projectilePrefab, transform.position, Quaternion.identity);
                if (proj.TryGetComponent<Rigidbody>(out var rb))
                    rb.linearVelocity = (target.transform.position - transform.position).normalized * _curLevel.projectileSpeed;
                // ProjectileController 로직에서 OnHit 시 AoE, 데미지 적용
            }
            else
            {
                // 즉시 이펙트 + 데미지 처리 (AoE 포함)
                HandleAoE(transform.position);
            }

            // 이펙트 및 사운드
            if (_curLevel.attackEffectPrefab != null)
                Instantiate(_curLevel.attackEffectPrefab, transform.position, Quaternion.identity);
            if (_curLevel.attackSoundClip != null)
                _audio.PlayOneShot(_curLevel.attackSoundClip);
        }
    }

    private void HandleAoE(Vector3 point)
    {
        if (_curLevel.areaShape == AreaShape.None) return;

        Collider[] colliders;
        if (_curLevel.areaShape == AreaShape.Circle)
            colliders = Physics.OverlapSphere(point, _curLevel.splashRadius, _curLevel.targetLayerMask);
        else
            return; // Cone, Line 구현 생략

        foreach (var col in colliders)
        {
            if (_curLevel.targetTags.Contains(col.tag))
            {
                // col.GetComponent<EnemyController>().TakeDamage(_curLevel.damage);
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
}