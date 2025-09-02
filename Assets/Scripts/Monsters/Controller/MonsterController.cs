// MonsterController.cs
using System.Linq;
using System.Collections;
using UnityEngine;
using System;

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
    //public MonsterLevelData CurrentLevelData => _curLevel;
    private MonsterModel _monsterModel;
    private MonsterView _monsterView;
    private MonsterMovement _monsterMovement;

    private Animator _anim;
    private AudioSource _audio;
    private Coroutine _behaviourRoutine;
    private Coroutine _moveRoutine;
    private Coroutine _stunRoutine;
    private bool _isDead = false;
    private bool _isStunned = false;

    public event Action<int> OnGiveReward;
    public Transform attackPos;

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
        // 레벨 결정
        _curLevel = _allLevels.First(l => l.size == size);

        // 이동 컴포넌트 생성 및 저장
        _monsterMovement = gameObject.AddComponent<MonsterMovement>();
        _monsterMovement.SetPath(WaveManager.Instance.path.Waypoints, _curLevel, _monsterModel);

        // 이동, 공격, 행동 루프 시작
        _moveRoutine = StartCoroutine(_monsterMovement.MoveAlongPath());
        _behaviourRoutine = StartCoroutine(BehaviorLoop());
    }

    private IEnumerator BehaviorLoop()
    {
        var mainTower = FindObjectOfType<MainTowerController>();

        // 몬스터와 메인타워가 살아있는동안 로직 수행
        while (_monsterModel.CurrentHp > 0 && mainTower != null && mainTower.CurrentHp > 0)
        {
            if (_isStunned)
            {
                yield return null;
                continue;
            }

            // 현재 몬스터와 메인타워 간의 거리 계산
            float distance = Vector3.Distance(this.transform.position, mainTower.transform.position);

            if (distance <= _curLevel.attackRange)
            {
                // 메인타워가 몬스터 공격 사정거리 안에 들어오면 이동 중지
                _monsterMovement.CanMove = false;

                // 공격 로직 (애니메이션 + 대기 + 데미지)
                _anim.SetTrigger("Attack");
                yield return null;
                yield return new WaitForSeconds(GetCurrentStateClipLength());

                // 몬스터가 원거리 공격 타입인 경우
                if (_curLevel.isRanged && _curLevel.projectilePrefab != null)
                {
                    var projectile = Instantiate(_curLevel.projectilePrefab, attackPos.position, attackPos.rotation);
                    projectile.AddComponent<MonsterProjectile>().Setup(_curLevel, mainTower.transform.position);
                }
                else
                {
                    mainTower.ApplyDamage(_curLevel.attackPower);
                }
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                // 공격 범위 밖: 이동 허용
                _monsterMovement.CanMove = true;
                yield return null;
            }
        }
    }

    // 현재 진행중인 몬스터 모션의 클립 길이 Get
    private float GetCurrentStateClipLength()
    {
        var infos = _anim.GetCurrentAnimatorClipInfo(0);
        if (infos != null && infos.Length > 0)
            return infos[0].clip.length;
        return 0.5f;
    }

    public void Stun(float stunDuration)
    {
        // 포댕이 스킬 범위 있는동안 이미 스턴을 당하고 있는 상태면 스턴 스킵
        if (_isStunned)
        {
            StopCoroutine(_stunRoutine);
        }
        _stunRoutine = StartCoroutine(StunRoutine(stunDuration));
    }

    // 스턴효과 적용 코루틴
    private IEnumerator StunRoutine(float duration)
    {
        _isStunned = true;
        if(_monsterMovement != null)
        {
            _monsterMovement.CanMove = false;
        }

        yield return new WaitForSeconds(duration);

        _isStunned = false;

        // 스턴 풀렸을 때 몬스터가 죽지 않았으면 이동 재개
        if (this != null && _monsterMovement != null)
        {
            //_monsterMovement.CanMove = true;
        }
        _stunRoutine = null;
    }

    // 몬스터가 받은 데미지를 MonsterModel에게 전달
    public void ApplyDamage(int amount)
    {
        // 몬스터 체력이 0 이하면 확실히 데미지 처리되지 않도록 2차 예외처리
        if (_monsterModel.CurrentHp <= 0) return;

        _monsterModel.TakeDamage(amount);
    }

    private void OnDied()
    {
        // 이미 사망 판정 났으면 이후 로직 무시
        if (_isDead) return;
        _isDead = true;

        _monsterModel.OnDied -= OnDied;

        // 몬스터 처치 관련 스테이지 포인트 보상 이벤트 발행
        int reward = GiveReward();
        OnGiveReward?.Invoke(reward);

        // 사망 판정 시 더 이상 공격을 받지 않도록 몬스터의 모든 오브젝트 물리 예외 처리
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        // 현재 스폰된 몬스터 객체의 모든 코루틴 중단
        if (_behaviourRoutine != null)
            StopCoroutine(_behaviourRoutine);
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);
        if (_monsterMovement != null)
            Destroy(_monsterMovement);

        // 사망 모션 수행
        _anim.SetTrigger("Die");

        // 애니메이션 길이만큼 기다렸다가 파괴
        var dieClip = _anim.runtimeAnimatorController
                   .animationClips
                   .FirstOrDefault(c => c.name == "Die");
        float delay = (dieClip != null) ? dieClip.length : 1f;
                
        Destroy(gameObject, delay + 0.1f);
    }

    public int GiveReward()
    {
        return (_curLevel.rewardPointsMax > _curLevel.rewardPointsMin)
            ? UnityEngine.Random.Range(_curLevel.rewardPointsMin, _curLevel.rewardPointsMax + 1)
            : _curLevel.rewardPointsMin;
    }
}
