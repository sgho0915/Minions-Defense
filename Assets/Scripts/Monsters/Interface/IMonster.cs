// IMonster.cs
/// <summary>
/// 몬스터 동작 계약(Interface)
/// </summary>
public interface IMonster
{
    void Initialize(MonsterLevelData initialLevel, MonsterLevelData[] allLevels);
    void SetSize(MonsterSize size);
    int GiveReward();
}