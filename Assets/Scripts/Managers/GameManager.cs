using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Settings")]
    public int stageIndex;

    [Header("Wave Manager")]
    public WaveManager waveManager;    // 씬에 배치 혹은 동적 생성

    //[Header("UI")]
    //public StageUIManager stageUIManager;   // 남은 HP, Gold, 타이머 등 HUD

    //[Header("Main Tower")]
    //public MainTowerController mainTower;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //// 메인타워 체력 초기화
        //mainTower.Initialize(/* initial HP */);
        //// 웨이브 시작
        //StartCoroutine(RunStage());
    }

    private IEnumerator RunStage()
    {
        yield return null;
        //// 웨이브 진행
        //yield return StartCoroutine(waveManager.RunWaves());
        //// 몬스터가 다 죽고 실패조건(메인타워 HP>0) 이면 클리어
        //if (mainTower.CurrentHp > 0)
        //    OnStageClear();
    }

    public void OnStageFail()
    {
        //stageUIManager.ShowGameOver(false);
    }

    public void OnStageClear()
    {
        //stageUIManager.ShowGameOver(true);
    }
}
