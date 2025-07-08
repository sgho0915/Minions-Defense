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
    private MonsterLevelData[] _allLevels;
    private MonsterLevelData _curLevel;
    private MonsterModel _monsterModel;
    private MonsterView _monsterView;
    private Animator _anim;
    private AudioSource _audio;
    public MonsterLevelData CurrentLevelData => _curLevel;

    private Coroutine _moveRoutine;

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
        // 1) 레벨 결정
        _curLevel = _allLevels.First(l => l.size == size);

        // 2) Movement 컴포넌트 붙이고, “Waypoints + _curLevel” 를 넘김
        var mover = gameObject.AddComponent<MonsterMovement>();
        mover.SetPath(WaveManager.Instance.path.Waypoints, _curLevel);

        // 3) 이동 끝나면 공격/사망 루프 시작
        StartCoroutine(BehaviorLoop(mover));
    }

    private IEnumerator BehaviorLoop(MonsterMovement mover)
    {
        // 이동이 모두 끝날 때까지 대기
        yield return mover.MoveAlongPath();

        // 목표(예: 타워) 공격 루프
        while (_monsterModel.CurrentHp > 0)
        {
            _anim.SetTrigger("Attack");
            HandleAttack();
            yield return new WaitForSeconds(_curLevel.attackAnim.length);
        }

        // 사망
        _anim.SetTrigger("Die");
        yield return new WaitForSeconds(_curLevel.deathAnim.length);
        Die();
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
        Destroy(gameObject);
        // GameManager.Instance.AddGold(reward);
    }

    public int GiveReward()
    {
        return (_curLevel.rewardPointsMax > _curLevel.rewardPointsMin)
            ? Random.Range(_curLevel.rewardPointsMin, _curLevel.rewardPointsMax + 1)
            : _curLevel.rewardPointsMin;
    }
}
