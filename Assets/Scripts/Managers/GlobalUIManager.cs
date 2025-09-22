// GlobalUIManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance {  get; private set; }

    [Header("공용 UI 프리팹")]
    [SerializeField] private SettingsView settingsViewPrefab;

    private SettingsView settingsView;
    private Stack<UIView> viewStack = new Stack<UIView>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeCommonViews();
    }

    private void InitializeCommonViews()
    {
        if(settingsView == null)
        {
            settingsView = Instantiate(settingsViewPrefab, transform);
            settingsView.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 새로운 View를 스택에 쌓고 활성화
    /// </summary>
    public void PushView(UIView view)
    {
        // 현재 최상단 View가 있다면 비활성화
        if(viewStack.Count > 0)
        {
            viewStack.Peek().canvasGroup.interactable = false;
        }

        viewStack.Push(view);
        view.Show();
    }

    /// <summary>
    /// 스택 최상단 View를 닫고 이전 View 활성화
    /// </summary>
    public void PopView()
    {
        if (viewStack.Count > 0)
        {
            UIView viewToClose = viewStack.Pop();
            viewToClose.Hide();

            // 닫고 난 후 스택에 다른 뷰가 남아있다면 그 뷰를 다시 보여줌
            if (viewStack.Count > 0)
            {
                viewStack.Peek().gameObject.SetActive(true);
            }
        }
    }

    public void ShowSettingsView()
    {
        PushView(settingsView);
    }

    public void ShowQuitView()
    {
        PushView(settingsView);
    }
}
