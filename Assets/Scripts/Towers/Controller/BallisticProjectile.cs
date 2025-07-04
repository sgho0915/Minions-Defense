using System.Drawing;
using UnityEngine;
using System.Linq;

/// <summary>
/// 발사 시점의 목표 위치를 향해 포물선 궤도로 날아가고,
/// 충돌 시 이펙트·사운드를 재생한 뒤 자신을 파괴합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BallisticProjectile : MonoBehaviour
{
    private TowerLevelData _levelData;

    private Rigidbody _rb;
    private GameObject _impactEffect;
    private AudioClip _impactSound;
    private AudioSource _audio;

    /// <summary>
    /// 투사체 초기 설정
    /// </summary>
    /// <param name="targetPoint">발사 시 목표 지점(현재 적 위치)</param>
    /// <param name="launchSpeed">초기 발사 속도</param>
    /// <param name="impactEffect">충돌 이펙트 Prefab</param>
    /// <param name="impactSound">충돌 사운드</param>
    public void Setup(TowerLevelData levelData, Vector3 targetPoint)
    {
        _levelData = levelData;
        _impactEffect = levelData.projectileImpactEffectPrefab;
        _impactSound = levelData.projectileImpactSoundClip;

        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;  // 물리 중력 사용
        _rb.linearDamping = 0f;
        
        _audio = gameObject.AddComponent<AudioSource>();

        // 1) 수평 거리와 높이 차 계산
        Vector3 toTarget = targetPoint - transform.position;
        float g = Mathf.Abs(Physics.gravity.y); // 9.81
        float v2 = levelData.projectileSpeed * levelData.projectileSpeed;
        float xz = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
        float y = toTarget.y;

        // 2) 포물선 공식: v^2 ± sqrt(...) → 두 해 중 하나 선택
        float underSqrt = v2 * v2 - g * (g * xz * xz + 2 * y * v2);
        if (underSqrt < 0)
        {
            // 사정거리 벗어남 → 직선 발사
            _rb.linearVelocity = toTarget.normalized * levelData.projectileSpeed;
            return;
        }

        float root = Mathf.Sqrt(underSqrt);
        // 높은 궤적(High arc)
        float tanHigh = (v2 + root) / (g * xz);
        // 낮은 궤적(Low arc)
        float tanLow = (v2 - root) / (g * xz);

        // 예시: 낮은 궤적을 쓰고 싶으면
        float angle = Mathf.Atan(tanLow);

        // 3) 초기 속도 벡터 계산
        Vector3 flatDir = new Vector3(toTarget.x, 0, toTarget.z).normalized;
        Vector3 velocity = flatDir * (levelData.projectileSpeed * Mathf.Cos(angle))
                           + Vector3.up * (levelData.projectileSpeed * Mathf.Sin(angle));
        _rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var hitPoint = collision.contacts[0].point; // 착탄지점

        // 1) 충돌 이펙트 생성
        if (_impactEffect != null)
        {
            var effectInstance = Instantiate(_impactEffect, hitPoint, Quaternion.identity);
            var systems = effectInstance.GetComponentsInChildren<ParticleSystem>();

            // 가장 긴 재생 시간을 찾기
            float maxDuration = 0f;
            foreach (var ps in systems)
            {
                var main = ps.main;
                //float lifetime = main.duration + main.startLifetime.constantMax;
                float lifetime = main.duration;
                if (lifetime > maxDuration) maxDuration = lifetime;
            }

            // 최대 재생 시간+약간의 여유(0.1s) 후에 파괴 예약
            Destroy(effectInstance, maxDuration - 0.03f);
        }

        // 2) 충돌 사운드
        if (_impactSound != null)
            AudioSource.PlayClipAtPoint(_impactSound, collision.contacts[0].point); // 착탄 지점에서 Audiosource 생성, 재생, 파괴

        // 3) 광역 데미지 AoE 처리
        if (_levelData.areaShape == TowerAreaShape.Circle && _levelData.splashRadius > 0)
        {
            var cols = Physics.OverlapSphere(
                hitPoint,
                _levelData.splashRadius,
                _levelData.targetLayerMask);

            foreach (var col in cols)
            {
                if (_levelData.targetTags.Contains(col.tag))
                {
                    var monsterController = col.GetComponent<MonsterController>();
                    monsterController?.ApplyDamage((int)_levelData.damage);
                }
            }
        }

        // 4) 자신 파괴
        Destroy(gameObject);        
    }
}
