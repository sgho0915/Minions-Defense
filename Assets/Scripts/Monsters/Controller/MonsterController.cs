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

        // 체력 감지 및 사망 이벤트 감시
        _monsterModel.OnDied += OnDied;
    }

    public void SetSize(MonsterSize size)
    {
        // 1) 레벨 결정
        _curLevel = _allLevels.First(l => l.size == size);

        // 2) 이동 끝나면 공격/사망 루프 시작
        StartCoroutine(BehaviorLoop());
    }

    private IEnumerator BehaviorLoop()
    {
        // 1) 이동
        var mover = gameObject.AddComponent<MonsterMovement>();
        mover.SetPath(WaveManager.Instance.path.Waypoints, _curLevel, _monsterModel);

        // mover.MoveAlongPath() 자체가 내부에서 매 프레임 사망 체크 하도록 아래에서 바꿔 줄 예정
        yield return StartCoroutine(mover.MoveAlongPath());

        // 2) 이동 중에 죽으면 여기까지 내려오지 않습니다.
        //    살아남았다면 공격 루프
        while (_monsterModel.CurrentHp > 0)
        {
            _anim.SetTrigger("Attack");
            HandleAttack();
            yield return new WaitForSeconds(_curLevel.attackAnim.length);
        }
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
        // 몬스터 체력이 0 이하면 확실히 데미지 처리되지 않도록 2차 예외처리
        if (_monsterModel.CurrentHp <= 0) return;

        _monsterModel.TakeDamage(amount);
    }

    private void OnDied()
    {
        Debug.Log("사망!");
        // 사망 판정 시 더 이상 공격을 받지 않도록 물리 예외 처리
        var collider = this.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // 현재 스폰된 몬스터 객체의 모든 코루틴 중단
        StopAllCoroutines();

        // 사망
        _anim.SetTrigger("Die");

        // 애니메이션 길이만큼 기다렸다가 파괴
        StartCoroutine(DestroyAfterDelay(_curLevel.deathAnim.length));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public int GiveReward()
    {
        return (_curLevel.rewardPointsMax > _curLevel.rewardPointsMin)
            ? Random.Range(_curLevel.rewardPointsMin, _curLevel.rewardPointsMax + 1)
            : _curLevel.rewardPointsMin;
    }
}
