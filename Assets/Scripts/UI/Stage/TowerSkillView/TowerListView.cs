// TowerListView.cs
using System;
using UnityEngine;

/// <summary>
/// 타워 선택 리스트 표시, 선택 시 이벤트 방출 View
/// </summary>
public class TowerListView : MonoBehaviour
{
    public event Action<TowerDataSO> OnTowerSelected;

    [SerializeField] private Transform contentParent;
    [SerializeField] private TowerListItem itemPrefab;

    /// <summary>
    /// 에디터에 할당된 TowerDataSo 배열로 리스트를 채움
    /// </summary>
    /// <param name="towerDataArray"></param>
    public void Populate(TowerDataSO[] towerDataArray)
    {
        foreach (var data in towerDataArray)
        {
            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(data, () => OnTowerSelected?.Invoke(data));
        }
    }
}
