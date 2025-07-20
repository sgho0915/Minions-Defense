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

    [Header("원거리 투사체 생성 위치")]
    public Transform projectileCreatePos;

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
        // 메인타워 객체 찾기
        var mainTower = FindObjectOfType<MainTowerController>();

        // 1) 몬스터 이동 IEnumerator 가져오기
        var mover = gameObject.AddComponent<MonsterMovement>();
        mover.SetPath(WaveManager.Instance.path.Waypoints, _curLevel, _monsterModel);
        StartCoroutine(mover.MoveAlongPath());


        // 2) 몬스터와 메인타워가 살아있는동안 로직 수행
        while (_monsterModel.CurrentHp > 0 && mainTower != null && mainTower.CurrentHp > 0)
        {
            // 현재 몬스터와 메인타워 간의 거리 계산
            float distance = Vector3.Distance(this.transform.position, mainTower.transform.position);

            // distance와 몬스터의 공격 사정거리 비교
            if(distance <= _curLevel.attackRange)
            {
                Debug.Log($"몬스터 공격범위안에 메인타워가 들어옴 -> distance : {distance}, monster attack range : {_curLevel.attackRange}");
                // 사정권 내에 들어오면 이동 금지
                mover.CanMove = false;

                // 공격 로직 (애니메이션 + 대기 + 데미지)
                _anim.SetTrigger("Attack");
                yield return null;
                yield return new WaitForSeconds(GetCurrentStateClipLength());

                // 몬스터가 원거리 공격 타입인 경우
                if (_curLevel.isRanged)
                {
                    if (_curLevel.projectilePrefab != null) {
                        var projectile = Instantiate(_curLevel.projectilePrefab, projectileCreatePos != null ? projectileCreatePos.position : transform.position,
                    projectileCreatePos != null ? projectileCreatePos.rotation : Quaternion.identity);
                        projectile.AddComponent<MonsterProjectile>().Setup(_curLevel, mainTower.transform.position);
                    }                                        
                }
                else
                {
                    mainTower.ApplyDamage(_curLevel.attackPower);
                }

                yield return new WaitForSeconds(0.1f);
            }
            else 
            {
                Debug.Log($"몬스터 공격범위밖임 -> distance : {distance}, monster attack range : {_curLevel.attackRange}");
                // 공격 범위 밖: 이동 허용
                mover.CanMove = true;
                yield return null;
            }            
        }
    }


    private float GetCurrentStateClipLength()
    {
        var infos = _anim.GetCurrentAnimatorClipInfo(0);
        if (infos != null && infos.Length > 0)
            return infos[0].clip.length;
        return 0.5f;
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
        _monsterModel.OnDied -= OnDied;

        // 사망 판정 시 더 이상 공격을 받지 않도록 물리 예외 처리
        var collider = this.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // 현재 스폰된 몬스터 객체의 모든 코루틴 중단
        StopAllCoroutines();

        // 사망
        _anim.SetTrigger("Die");        

        // 애니메이션 길이만큼 기다렸다가 파괴
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitUntil(() =>
        {
            var info = _anim.GetCurrentAnimatorStateInfo(0);
            return info.IsName("Die") && info.normalizedTime >= 1f;
        });
        Destroy(gameObject);
    }

    public int GiveReward()
    {
        return (_curLevel.rewardPointsMax > _curLevel.rewardPointsMin)
            ? Random.Range(_curLevel.rewardPointsMin, _curLevel.rewardPointsMax + 1)
            : _curLevel.rewardPointsMin;
    }
}
