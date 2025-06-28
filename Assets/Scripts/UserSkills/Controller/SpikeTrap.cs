// SpikeTrap.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// 스파이크 트랩 로직 (통과 몬스터 데미지·감속)
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class SpikeTrap : MonoBehaviour
{
    private SpikeLevelData _lvl;
    private float _endTime;

    public void Setup(SpikeLevelData lvl)
    {
        _lvl = lvl;
        _endTime = Time.time + lvl.trapDuration;

        // 콜라이더 세팅
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = lvl.trapRadius;

        // 이펙트·사운드
        if (lvl.trapEffectPrefab != null)
            Instantiate(lvl.trapEffectPrefab, transform.position, Quaternion.identity);
        if (lvl.trapSoundClip != null)
            AudioSource.PlayClipAtPoint(lvl.trapSoundClip, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        var monsterModel = other.GetComponent<MonsterModel>();
        if (monsterModel != null)
        {
            monsterModel.TakeDamage(_lvl.damagePerSecond);
            // 슬로우 기능이 MonsterModel에 구현되어 있다고 가정
            monsterModel.Slow(_lvl.slowAmount, _lvl.slowDuration);
        }
    }

    private void Update()
    {
        if (Time.time >= _endTime)
            Destroy(gameObject);
    }
}
