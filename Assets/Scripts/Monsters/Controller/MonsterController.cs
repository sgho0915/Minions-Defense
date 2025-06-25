// MonsterController.cs
using System.Linq;
using System.Collections;
using UnityEngine;

/// <summary>
/// IMonster 구현체이자 MVC의 Controller 역할
/// - DataSO ↔ HealthModel ↔ HealthView 연결
/// - 이동·공격·사망 로직 수행
/// </summary>
[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class MonsterController : MonoBehaviour, IMonster
{
    private MonsterDataSO _data;
    private MonsterLevelData _curLevel;
    private MonsterHealthModel _healthModel;
    private MonsterHealthView _healthView;
    private Animator _anim;
    private AudioSource _audio;

    public void Initialize(MonsterDataSO data)
    {
        _data = data;
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();

        // Health MVC 세팅
        _healthModel = new MonsterHealthModel(data.levelData[0].maxHp);
        _healthView = GetComponentInChildren<MonsterHealthView>();
        _healthView.Initialize(_healthModel);
    }

    public void SetSize(MonsterSize size)
    {
        _curLevel = _data.levelData.First(l => l.size == size);

        // 스폰 애니메이션·연출
        if (_curLevel.spawnEffectPrefab != null)
            Instantiate(_curLevel.spawnEffectPrefab, transform.position, Quaternion.identity);
        if (_curLevel.spawnSoundClip != null)
            _audio.PlayOneShot(_curLevel.spawnSoundClip);

        StartCoroutine(BehaviorLoop());
    }

    private IEnumerator BehaviorLoop()
    {
        while (_healthModel.CurrentHp > 0)
        {
            // 이동
            _anim.Play(_curLevel.moveAnim.name);
            yield return MoveToTarget();

            // 공격
            _anim.Play(_curLevel.attackAnim.name);
            HandleAttack();
            yield return new WaitForSeconds(1f);
        }

        Die();
    }

    private IEnumerator MoveToTarget()
    {
        // TODO: 실제 이동 로직
        yield return null;
    }

    private void HandleAttack()
    {
        // 데미지 및 투사체
        // 예시: 충돌 시 _healthModel.TakeDamage(...)
        if (_curLevel.isRanged && _curLevel.projectilePrefab != null)
        {
            var proj = Instantiate(_curLevel.projectilePrefab, transform.position, Quaternion.identity);
            if (proj.TryGetComponent<Rigidbody>(out var rb))
                rb.linearVelocity = transform.forward * _curLevel.projectileSpeed;
        }
        else
        {
            // 근접 데미지 처리
        }

        // 타격 연출
        if (_curLevel.hitEffectPrefab != null)
            Instantiate(_curLevel.hitEffectPrefab, transform.position, Quaternion.identity);
        if (_curLevel.hitSoundClip != null)
            _audio.PlayOneShot(_curLevel.hitSoundClip);
    }

    private void Die()
    {
        // 사망 애니메이션
        if (_curLevel.deathAnim != null)
            _anim.Play(_curLevel.deathAnim.name);

        // 사망 연출
        if (_curLevel.hitEffectPrefab != null)
            Instantiate(_curLevel.hitEffectPrefab, transform.position, Quaternion.identity);

        // 보상
        int reward = GiveReward();
        // GameManager.Instance.AddGold(reward);

        Destroy(gameObject, 1f);
    }

    public int GiveReward()
    {
        return (_curLevel.rewardPointsMax > _curLevel.rewardPointsMin)
            ? Random.Range(_curLevel.rewardPointsMin, _curLevel.rewardPointsMax + 1)
            : _curLevel.rewardPointsMin;
    }
}
