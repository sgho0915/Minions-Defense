// MagicPoeController.cs
using UnityEngine;
using System.Linq;

/// <summary>
/// “마법의 포댕이” 스킬 시전, 비용, 사용 자원 등 스킬 사용 위한 규칙, 조건 관리 컨트롤러 클래스
/// </summary>
public class MagicPoeController : MonoBehaviour, ISkill
{
    private MagicPoeDataSO _data;       // 마법의 포댕이 SO
    private Transform _owner;           // 컨트롤러 소유자
    private int _currentLevel = 1;      // 현재 스킬 레벨
    private float _lastExecutionTime;    // 마지막 스킬 시전 시간

    // 현재 스킬의 레벨 데이터를 가져오기 위한 프로퍼티
    public SkillLevelData CurrentLevelData => _data.levels.FirstOrDefault(l => l.level == _currentLevel);
    private MagicPoeLevelData CurrentPoeLevelData => CurrentLevelData as MagicPoeLevelData;

    // 다음 레벨 스킬 레벨 데이터 가져오기
    public SkillLevelData NextLevelData => _data.levels.FirstOrDefault(l => l.level == _currentLevel + 1);
    private MagicPoeLevelData NextPoeLevelData => NextLevelData as MagicPoeLevelData;


    public void Initialize(SkillDataSO dataSo, Transform owner)
    {
        _data = dataSo as MagicPoeDataSO;   // Safe Type Casting
        if(_data == null)
        {
            Debug.LogError("MagicPoeDataSO 타입이 아닌 잘못된 타입이 전달되었습니다.", this);
            return;
        }
        _owner = owner;
        _lastExecutionTime = -float.MaxValue;   // 처음에는 즉시 사용 가능
    }

    public void SetLevel(int level)
    {
        _currentLevel = Mathf.Clamp(level, 1, _data.levels.Length); // level이 1부터 최대 레벨 사이의 값을 가지도록 보장
    }

    public bool IsReady()
    {
        if (CurrentLevelData == null) return false;
        return Time.time >= _lastExecutionTime + CurrentPoeLevelData.cooldown;
    }

    public void ExecuteSkill(Vector3 targetPosition)
    {
        if (!IsReady()) return;

        var lvlData = CurrentPoeLevelData;
        if (lvlData == null) return;

        // WaveManager로부터 현재 스테이지의 경로 정보(WayPath)를 가져옴
        var path = WaveManager.Instance.path.Waypoints;
        if(path == null || path.Length == 0)
        {
            Debug.LogError("경로가 설정되지 않았습니다.");
            return;
        }

        // 메인타워에 가장 가까운 WayPoint에 포댕이를 소환
        Vector3 spawnPosition = path[path.Length - 1].transform.position;
        var poeInstance = Instantiate(lvlData.modelPrefab, spawnPosition, Quaternion.identity);

        // 포댕이에 MagicPoeController 컴포넌트 부착 및 Setup 진행
        var patroller = poeInstance.AddComponent<MagicPoePatroller>();
        patroller.Setup(lvlData, path);

        // 마지막 스킬 시전 시간 기록
        _lastExecutionTime = Time.time;
    }
}