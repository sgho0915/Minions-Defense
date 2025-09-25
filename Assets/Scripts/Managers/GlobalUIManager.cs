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
        // 스택에 이미 다른 View가 열려있다면
        if(viewStack.Count > 0)
        {
            // 현재 최상단 View(Peek)를 클릭할 수 없도록 비활성화
            viewStack.Peek().canvasGroup.interactable = false;
        }

        // 새로운 View를 스택의 최상단에 추가(Push)
        viewStack.Push(view);
        // 새로운 View 활성화해 화면에 표시
        view.Show();
    }

    /// <summary>
    /// 스택 최상단 View를 닫고 이전 View 활성화
    /// </summary>
    public void PopView()
    {
        // 스택이 비어있지 않은 경우
        if (viewStack.Count > 0)
        {
            // 스택 최상단 View를 꺼내서(Pop) UIView 변수에 저장
            UIView viewToClose = viewStack.Pop();
            // 꺼낸 View를 비활성화
            viewToClose.Hide();

            // View를 닫은 후에도 스택에 다른 View가 남아있다면
            if (viewStack.Count > 0)
            {
                // 새로운 최상단 View(Peek)를 다시 활성화
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
