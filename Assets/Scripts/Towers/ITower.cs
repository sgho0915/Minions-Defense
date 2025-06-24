/// <summary>
/// 타워 동작의 “계약(인터페이스)” 정의
/// </summary>
public interface ITower
{
    /// <summary>
    /// ScriptableObject 데이터로 초기화
    /// </summary>
    void Initialize(TowerDataSO towerDataSO);

    /// <summary>
    /// 레벨 변경(4단계 A,B 타입 분기 포함)
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lv4Branch"></param>
    void SetLevel(int level, TowerLevel4Type lv4Branch = TowerLevel4Type.None);

    /// <summary>
    /// 타워 판매 처리 후 반환 금액
    /// </summary>
    int Sell();
}
