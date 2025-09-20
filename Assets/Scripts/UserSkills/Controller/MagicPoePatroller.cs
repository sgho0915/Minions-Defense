// MagicPoePatroller.cs
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX.Utility;
using System.Reflection;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;

/// <summary>
/// “마법의 포댕이” 스킬 이동속도, 공격패턴, 소멸 등 유닛 자체의 행동 관리 로직
/// </summary>
public class MagicPoePatroller : MonoBehaviour
{
    private MagicPoeLevelData _levelData;
    private Waypoint[] _reversedPath;

    [SerializeField] private Transform effectPos;

    private int _currentPathIndex = 0;
    private float _totalDamageDealt = 0;
    private float _tickTimer = 0f;

    private Dictionary<MonsterController, GameObject> _activeAttackEffects = new Dictionary<MonsterController, GameObject>();

    public void Setup(MagicPoeLevelData levelData, Waypoint[] originalPath)
    {
        _levelData = levelData;

        // 경로를 복사한뒤 역주행 경로를 만듬
        _reversedPath = originalPath.ToArray();
        System.Array.Reverse(_reversedPath);

        effectPos = this.transform.Find("EffectPos");

        // 오라 이펙트를 자식으로 붙임
        if (_levelData.auraEffectPrefab != null)
            Instantiate(_levelData.auraEffectPrefab, effectPos.position, effectPos.rotation, effectPos);

        // 메인 로직 코루틴 시작
        StartCoroutine(PatrolAndAttackLoop());
    }

    private IEnumerator PatrolAndAttackLoop()
    {
        while (true)
        {
            HandleMovement();
            HandleAuraAttack();

            // 소멸조건 1) 최대 피해량 한도 채웠거나
            // 소멸조건 2) 경로의 끝(몬스터 스폰지점)에 도달했거나
            if (_totalDamageDealt >= _levelData.maxDamageOutput || _currentPathIndex >= _reversedPath.Length)
            {
                Despawn();
                break;  // 루프 탈출 후 소멸
            }

            yield return null;  // 다음 프레임까지 대기
        }
    }

    private void HandleMovement()
    {
        if (_currentPathIndex >= _reversedPath.Length) return;

        // 현재 목표 지점 설정
        Vector3 targetPosition = _reversedPath[_currentPathIndex].transform.position;

        // 목표를 향해 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _levelData.moveSpeed * Time.deltaTime);

        // 방향 전환
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 목표 지점에 거의 도달했으면 다음 인덱스로
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            _currentPathIndex++;
        }
    }

    private void HandleAuraAttack()
    {
        _tickTimer += Time.deltaTime;
        float tickInterval = 1f / _levelData.damageTickRate;

        // 정해진 공격주기가 됐는지 확인
        if (_tickTimer >= tickInterval)
        {
            _tickTimer -= tickInterval;

            // OverlapSphere를 사용해 주변의 모든 콜라이더를 찾음
            Collider[] colliders = Physics.OverlapSphere(transform.position, _levelData.auraRadius);

            var monsterInRange = new HashSet<MonsterController>();  // 범위 내 들어온 몬스터 목록

            // Collider 내의 몬스터를 찾아 목록에 추가
            foreach (var col in colliders)
            {
                if (col.CompareTag("Monster"))
                {
                    MonsterController monster = col.GetComponent<MonsterController>();
                    monsterInRange.Add(monster);                    
                }                
            }

            // 범위 내 존재 몬스터 목록에 대한 중복체크 후 데미지 부여 및 이펙트 생성 후 딕셔너리 추가
            foreach(var monster in monsterInRange)
            {
                if (!_activeAttackEffects.ContainsKey(monster))
                {
                    if (_levelData.attackEffectPrefab != null)
                    {
                        var attackEffect = Instantiate(_levelData.attackEffectPrefab, effectPos.position, effectPos.rotation, effectPos);
                        _activeAttackEffects.Add(monster, attackEffect);

                        VFXPropertyBinder binder = attackEffect.GetComponentInChildren<VFXPropertyBinder>();
                        if (binder != null)
                        {
                            var allBinders = binder.GetPropertyBinders<VFXBinderBase>();

                            foreach (var baseBinder in allBinders)
                            {
                                // 각 바인더의 실제 타입을 가져옵니다. (이 타입이 바로 internal인 VFXPositionBinder)
                                Type binderType = baseBinder.GetType();

                                // ★★★ 타입 정보로부터 "Property" 라는 이름의 필드(Field) 정보를 가져옵니다.
                                PropertyInfo propertyInfo = binderType.GetProperty("Property");

                                // ★★★ 타입 정보로부터 "Target" 이라는 이름의 필드(Field) 정보를 가져옵니다.
                                FieldInfo targetField = binderType.GetField("Target");

                                if (propertyInfo != null && targetField != null)
                                {
                                    // ★★★ "Property" 필드의 실제 값을 읽어옵니다. (예: "Pos1", "Pos4")
                                    string propertyName = (string)propertyInfo.GetValue(baseBinder);

                                    if (propertyName == "Pos1")
                                    {
                                        // ★★★ "Target" 필드의 값을 effectPos로 설정합니다.
                                        targetField.SetValue(baseBinder, this.effectPos);
                                    }
                                    else if (propertyName == "Pos4")
                                    {
                                        // ★★★ "Target" 필드의 값을 monster.transform으로 설정합니다.
                                        targetField.SetValue(baseBinder, monster.transform);
                                    }
                                }
                            }
                        }
                    }

                    // 데미지 적용
                    monster.ApplyDamage(_levelData.damagePerTick);
                    _totalDamageDealt += _levelData.damagePerTick;

                    Debug.Log($"포댕이 총 가용 체력 : {_levelData.maxDamageOutput} <-> 가한 공격력 : {_totalDamageDealt}");

                    // 스턴 적용
                    monster.Stun(_levelData.stunDuration);

                    // 사운드
                    SoundManager.Instance.PlaySFX(_levelData.attackSoundClip);
                }
            }
            
            // 현재 이펙트 딕셔너리 키(MonsterController)가 monsterInRange에 없거나 몬스터가 죽어 null인 경우 monsterOutRange에 추가
            var monsterOutRange = new List<MonsterController>();
            foreach(var monster in _activeAttackEffects.Keys)
            {
                if(!monsterInRange.Contains(monster) || monster == null)
                {
                    monsterOutRange.Add(monster);
                }
            }

            foreach(var removeMonster in monsterOutRange)
            {
                Destroy(_activeAttackEffects[removeMonster]);
                _activeAttackEffects.Remove(removeMonster);
            }
        }
    }

    private void Despawn()
    {
        // 소멸 이펙트, 사운드
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _levelData.auraRadius);
    }

    private void OnDestroy()
    {
        foreach(var effect in _activeAttackEffects.Values)
        {
            Destroy(effect.gameObject);
        }
        _activeAttackEffects.Clear();
    }
}
