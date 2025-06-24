using UnityEngine;

// Enemy 기본 공통 로직
// Init 함수를 통해 ScriptableObject 데이터를 전달받아 셋업
public class Enemy : MonoBehaviour
{
    private string name;
    private float maxHp;
    private float speed;

    // 몬스터 데이터 초기화
    public void Init(EnemyData data)
    {
        name = data.name;
        maxHp = data.maxHp;
        speed = data.speed;
        Debug.Log($"{name}생성 | 체력:{maxHp} | 스피드:{speed}");
    }

    private void Update()
    {
        // 간단 이동 로직
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
