// MainTowerController.cs
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class MainTowerController : MonoBehaviour
{
    public event Action<int, int> OnHpChanged;
    public event Action OnDied;

    [Header("Health")]
    public int initialHP = 1000;
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    [Header("View")]
    [SerializeField] private MainTowerView view;

    [Header("Attack Settings")]
    public Transform[] attackPoints;    // 발사 이펙트 생성 위치
    public GameObject projectilePrefab; // 투사체 프리팹
    public float attackSpeed = 1f;    // 초당 공격 횟수
    public float attackRange = 10f;      // 공격 사정거리
    public float projectileSpeed = 10f; // 투사체 속도
    public int attackPower = 50;        // 투사체 적중 시 피해량

    [Header("Targeting")]
    public LayerMask targetLayerMask;   // OverlapShepre에 사용할 레이어 마스크
    public string[] targetTags;

    [Header("AudioClip")]
    public AudioClip attackClip;

    private void Awake()
    {
        Initialize(initialHP);
    }

    private void Start()
    {
        StartCoroutine(AttackLoop());
    }

    public void Initialize(int hp)
    {
        MaxHp = hp;
        CurrentHp = hp;
        view.Initialize(this);
    }

    public void ApplyDamage(int dmg)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - dmg);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
        if (CurrentHp == 0) OnDied?.Invoke();
    }

    private IEnumerator AttackLoop()
    {
        var wait = new WaitForSeconds(40f / attackSpeed);

        while (CurrentHp > 0)
        {
            // 범위 내 설정된 가장 가까운 targetLayerMask의 콜라이더 검색
            var hits = Physics.OverlapSphere(transform.position, attackRange, targetLayerMask);
            var targets = hits
                .Where(c => targetTags.Contains(c.tag))
                .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                .ToArray();

            if (targets.Length > 0)
            {
                var target = targets[0].transform;

                // 각각의 attackPoint 에서 투사체 발사
                foreach (var pt in attackPoints)
                {
                    pt.LookAt(target);

                    // 1) 프리팹 인스턴스화
                    var projGO = Instantiate(projectilePrefab, pt.position, pt.rotation);

                    // 만약 프리팹에 미리 붙어 있지 않으면 추가
                    var bp = projGO.GetComponent<BallisticProjectile>()
                          ?? projGO.AddComponent<BallisticProjectile>();

                    // 임시로 사용하는 TowerLevelData
                    var lvlData = new TowerLevelData
                    {
                        projectileSpeed = projectileSpeed,
                        areaShape = TowerAreaShape.Circle,                     // AoE 아니면 None
                        targetLayerMask = targetLayerMask,                   // splash 용이지만 여기선 안씀
                        targetTags = targetTags,                              // splash 용
                        damage = attackPower,
                        splashRadius = 0.5f
                    };

                    // 발사
                    bp.Setup(lvlData, target.position);
                    SoundManager.Instance.PlaySFX(attackClip);
                }
            }

            yield return wait;
        }
    }

    // 디버그용: 에디터 상에서 공격 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
