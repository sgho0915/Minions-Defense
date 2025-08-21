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

        // 스턴
        var ec = other.GetComponent<MonsterController>();
        ec?.Stun(_lvl.stunDuration);

        // 데미지
        var monsterModel = other.GetComponent<MonsterModel>();
        monsterModel?.TakeDamage(_lvl.baseDamage);

        // 이펙트·사운드
        if (_lvl.attackEffectPrefab != null)
            Instantiate(_lvl.attackEffectPrefab, transform.position, Quaternion.identity);
        if (_lvl.attackSoundClip != null)
            AudioSource.PlayClipAtPoint(_lvl.attackSoundClip, transform.position);

        Destroy(gameObject);
    }
}
