// StageData.cs
using UnityEngine;

[System.Serializable]
public class StageData
{
    public int stageIndex;      // Scene 이름에 대응 (Stage_1, Stage_2...)
    public string stageName;    // Stage 이름
    public Sprite stageThumbnail;   // Stage 썸네일
    [Range(0, 3)]
    public int starGrade;   // 클리어 시점에 저장된 별 개수 (0~3)

    // 클리어 여부에 따른 자물쇠 효과, 버튼 interactable 여부 적용
}
