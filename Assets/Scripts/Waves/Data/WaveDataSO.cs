// WaveDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDataSO", menuName = "TowerDefenseAssets/Wave/WaveDataSO")]
/// <summary>
/// 씬 전체 웨이브(1,2,3…N) 데이터를 담는 ScriptableObject
/// </summary>
public class WaveDataSO : ScriptableObject
{
    [Header("웨이브 리스트")]
    public WaveInfo[] waves;
}