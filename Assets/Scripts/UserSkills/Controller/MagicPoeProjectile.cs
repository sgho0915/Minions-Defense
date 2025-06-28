// MagicPoeProjectile.cs
using UnityEngine;

/// <summary>
/// 포댕이 투사체 로직 (충돌 시 넉백·스턴·데미지)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MagicPoeProjectile : MonoBehaviour
{
    private MagicPoeLevelData _lvl;
    private Rigidbody _rb;

    public void Setup(MagicPoeLevelData lvl)
    {
        _lvl = lvl;
        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = transform.forward * lvl.moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Monster")) return;

        // 넉백
        if (other.attachedRigidbody != null)
            other.attachedRigidbody.AddForce(
                (other.transform.position - transform.position).normalized
                * _lvl.knockbackForce, ForceMode.Impulse);

        // 스턴
        var ec = other.GetComponent<MonsterController>();
        ec?.Stun(_lvl.stunDuration);

        // 데미지
        var monsterModel = other.GetComponent<MonsterModel>();
        monsterModel?.TakeDamage(_lvl.baseDamage);

        // 이펙트·사운드
        if (_lvl.hitEffectPrefab != null)
            Instantiate(_lvl.hitEffectPrefab, transform.position, Quaternion.identity);
        if (_lvl.hitSoundClip != null)
            AudioSource.PlayClipAtPoint(_lvl.hitSoundClip, transform.position);

        // 체인 연쇄 로직(3단계)
        if (_lvl.chainRadius > 0)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _lvl.chainRadius, 1 << LayerMask.NameToLayer("Monster"));
            foreach (var c in hits)
            {
                if (c.transform != other.transform)
                    c.GetComponent<MonsterModel>()?.TakeDamage(_lvl.baseDamage);
            }
        }

        Destroy(gameObject);
    }
}
