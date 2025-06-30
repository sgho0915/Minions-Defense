// WaveInfo.cs
using UnityEngine;

[System.Serializable]
/// <summary>
/// 한 웨이브에 대한 정보
/// </summary>
public class WaveInfo
{
    [Tooltip("웨이브 번호")]
    public int waveNumber;
    [Tooltip("스폰 엔트리 리스트")]
    public SpawnEntry[] spawns;
    [Tooltip("다음 웨이브 시작 전 대기 시간(초)")]
    public float delayAfterWave = 5f;
}