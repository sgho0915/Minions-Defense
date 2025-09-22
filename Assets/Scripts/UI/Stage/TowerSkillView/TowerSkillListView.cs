// TowerSkillListView.cs
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타워 선택 리스트 표시, 선택 시 이벤트 방출 View
/// </summary>
public class TowerSkillListView : UIView
{
    public event Action<TowerDataSO> OnTowerSelected;
    public event Action<ISkill> OnSkillSelected;    
    public event Action OnForceStartWaveButtonClicked;

    [Header("Display Remain Time Until Next Wave Start")]
    [SerializeField] private Button btnForceStartWave;
    [SerializeField] private TextMeshProUGUI txtCanvasNextWave;

    [Header("Tower 스크롤뷰 Content, 버튼 아이템")]
    [SerializeField] private Transform towerContentParent;
    [SerializeField] private TowerListItem towerItemPrefab;

    [Header("Skill 스크롤뷰 Content, 버튼 아이템")]
    [SerializeField] private Transform skillContentParent;
    [SerializeField] private SkillListItem skillItemPrefab;

    // 생성된 스킬 아이템 관리
    private Dictionary<ISkill, SkillListItem> _skillItems = new Dictionary<ISkill, SkillListItem>();


    protected override void Awake()
    {
        base.Awake();

        // 클릭이 들어오면 다음 웨이브를 강제로 시작하도록 StageUiController로 이벤트만 날려줌
        btnForceStartWave.onClick.AddListener(() => {
            if (!GameManager.Instance.isWaveStarted)
            {
                GameManager.Instance.isWaveStarted = true;
                StartCoroutine(GameManager.Instance.RunStage());
            }
            else
            {
                SoundManager.Instance.PlayButtonClicked();
                OnForceStartWaveButtonClicked?.Invoke();
            }
        });
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 에디터에 할당된 TowerDataSo 배열로 리스트를 채움
    /// </summary>
    public void PopulateTowers(TowerDataSO[] towerDataArray)
    {
        foreach (var data in towerDataArray)
        {
            var item = Instantiate(towerItemPrefab, towerContentParent);
            item.Setup(data, () => { OnTowerSelected?.Invoke(data); });
        }
    }

    /// <summary>
    /// 스킬 목록을 채움
    /// </summary>
    public void PopulateSkills(System.Collections.Generic.List<ISkill> skills)
    {
        foreach (var skill in skills)
        {
            var item = Instantiate(skillItemPrefab, skillContentParent);
            item.Setup(skill, (selectedSkill) => OnSkillSelected?.Invoke(selectedSkill));
            _skillItems.Add(skill, item);
        }
    }

    public void RefreshSkillItemView(ISkill updatedSkill)
    {
        if (_skillItems.TryGetValue(updatedSkill, out SkillListItem item))
            item.UpdateView();
    }

    /// <summary>
    /// “강제 웨이브 시작” 버튼을 활성/비활성화
    /// </summary>
    public void SetNextWaveTimeUIInteractable(bool interactable, bool islastWave)
    {
        btnForceStartWave.interactable = interactable;

        if (islastWave)
        {
            //btnForceStartWave.gameObject.SetActive(!islastWave);
            txtCanvasNextWave.gameObject.SetActive(!islastWave);
        }
    }

    /// <summary>
    /// 남은 초 단위로 표시
    /// </summary>
    public void UpdateNextWaveTimer(float remainSeconds)
    {
        txtCanvasNextWave.text = $"{remainSeconds:F1}s";
    }
}
