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
    private MonsterLevelData[] _allLevels;
    private MonsterLevelData _curLevel;
    private MonsterModel _monsterModel;
    private MonsterView _monsterView;
    private Animator _anim;
    private AudioSource _audio;

    public void Initialize(MonsterLevelData initialLevel, MonsterLevelData[] allLevels)
    {
        _allLevels = allLevels;                     // 전체 레벨 배열 저장
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();

        // Health MVC 세팅
        _monsterModel = new MonsterModel(initialLevel.maxHp, initialLevel.moveSpeed);
        _monsterView = GetComponentInChildren<MonsterView>();
        _monsterView.Initialize(_monsterModel);
    }

    public void SetSize(MonsterSize size)
    {
        _curLevel = _data.levelData.First(l => l.size == size);

        StartCoroutine(BehaviorLoop()); // 기본 로직 시작 (이동 -> 전투 -> 사망)
    }

    private IEnumerator BehaviorLoop()
    {
        while (_monsterModel.CurrentHp > 0)
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
    }

    public void Stun(float stunDuration)
    {

    }

    public void ApplyDamage(int amount)
    {
        _monsterModel.TakeDamage(amount);
    }

    private void Die()
    {
        // 사망 애니메이션
        if (_curLevel.deathAnim != null)
            _anim.Play(_curLevel.deathAnim.name);

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
