using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MonsterProjectile : MonoBehaviour
{
    private MonsterLevelData _levelData;
    Rigidbody _rb;

    // 외부에서 한 번만 호출
    public void Setup(MonsterLevelData levelData, Vector3 targetPoint)
    {
        _levelData = levelData;

        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;  // 물리 중력 사용
        _rb.linearDamping = 0f;

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

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.TryGetComponent<MainTowerController>(out var tower))
            tower.ApplyDamage(_levelData.attackPower);

        Destroy(gameObject);
    }
}
