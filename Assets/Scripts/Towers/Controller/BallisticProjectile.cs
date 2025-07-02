using UnityEngine;

/// <summary>
/// 발사 시점의 목표 위치를 향해 포물선 궤도로 날아가고,
/// 충돌 시 이펙트·사운드를 재생한 뒤 자신을 파괴합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BallisticProjectile : MonoBehaviour
{
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
    public void Setup(Vector3 targetPoint, float launchSpeed, GameObject impactEffect, AudioClip impactSound)
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;  // 물리 중력 사용
        _impactEffect = impactEffect;
        _impactSound = impactSound;
        _audio = gameObject.AddComponent<AudioSource>();

        // 1) 수평 거리와 높이 차 계산
        Vector3 toTarget = targetPoint - transform.position;
        float g = Physics.gravity.y;
        float v2 = launchSpeed * launchSpeed;
        float xz = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
        float y = toTarget.y;

        // 2) 포물선 공식: v^2 ± sqrt(...) → 두 해 중 하나 선택
        float underSqrt = v2 * v2 - g * (g * xz * xz + 2 * y * v2);
        if (underSqrt <= 0f)
        {
            // 발사 속도로는 도달 불가 → 단순 직선 발사
            _rb.linearVelocity = toTarget.normalized * launchSpeed;
            return;
        }
        float root = Mathf.Sqrt(underSqrt);
        // 낮은 궤적: (v² - √(...)) / (g·xz)
        float tanTheta = (v2 - root) / (-g * xz);
        float angle = Mathf.Atan(tanTheta);

        // 3) 초기 속도 벡터 계산
        Vector3 flatDir = new Vector3(toTarget.x, 0, toTarget.z).normalized;
        Vector3 velocity = flatDir * (launchSpeed * Mathf.Cos(angle))
                           + Vector3.up * (launchSpeed * Mathf.Sin(angle));
        _rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1) 충돌 이펙트
        if (_impactEffect != null)
            Instantiate(_impactEffect, collision.contacts[0].point, Quaternion.identity);

        // 2) 충돌 사운드
        if (_impactSound != null)
            _audio.PlayOneShot(_impactSound);

        // 3) 자신 파괴
        Destroy(gameObject);
    }
}
