using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Test/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("적 정보")]
    public string name;  
    public float maxHp;       
    public float speed;       

    [Header("프리팹 주소 (Addressable")]
    public string prefabAddress;

    [Header("프리팹 직접 참조")]
    public GameObject prefab;
}
