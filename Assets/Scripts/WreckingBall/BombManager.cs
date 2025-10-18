using UnityEngine;

/// <summary>
/// 씬에 존재하는 폭탄의 개수를 관리하는 싱글톤 매니저입니다.
/// UI 등에서 현재 남은 폭탄 개수를 확인할 때 사용합니다.
/// </summary>
public class BombManager : MonoBehaviour
{
    private static BombManager instance;

    public static BombManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<BombManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("BombManager");
                    instance = obj.AddComponent<BombManager>();
                }
            }
            return instance;
        }
    }

    [Header("Bomb Settings")]
    [Tooltip("폭탄으로 인식할 GameObject의 태그입니다.")]
    [SerializeField] private string bombTag = "Bomb";

    [Header("Debug Settings")]
    [Tooltip("폭탄 개수 변경 시 자동으로 로그를 출력합니다.")]
    [SerializeField] private bool enableAutoDebugLog = true;

    private int lastActiveBombCount = -1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 초기 상태 로그
        if (enableAutoDebugLog)
        {
            LogBombStatus("게임 시작");
            lastActiveBombCount = GetActiveBombCount();
        }
    }

    private void Update()
    {
        // [추가] 매 프레임 폭탄 개수 변경 감지
        if (enableAutoDebugLog)
        {
            int currentCount = GetActiveBombCount();
            if (currentCount != lastActiveBombCount)
            {
                LogBombStatus("폭탄 개수 변경 감지");
                lastActiveBombCount = currentCount;
            }
        }
    }

    /// <summary>
    /// 현재 씬에 존재하는 활성화된 폭탄의 개수를 반환합니다.
    /// </summary>
    public int GetActiveBombCount()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag(bombTag);
        
        if (bombs == null || bombs.Length == 0)
        {
            return 0;
        }

        int activeCount = 0;
        foreach (var bomb in bombs)
        {
            if (bomb != null && bomb.activeInHierarchy)
            {
                activeCount++;
            }
        }

        return activeCount;
    }

    /// <summary>
    /// 현재 씬에 존재하는 전체 폭탄의 개수를 반환합니다. (비활성화 포함)
    /// </summary>
    public int GetTotalBombCount()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag(bombTag);
        return bombs != null ? bombs.Length : 0;
    }

    /// <summary>
    /// 폭발한 폭탄의 개수를 반환합니다.
    /// </summary>
    public int GetExplodedBombCount()
    {
        return GetTotalBombCount() - GetActiveBombCount();
    }

    /// <summary>
    /// [추가] 폭탄 상태를 로그로 출력합니다.
    /// </summary>
    /// <param name="eventName">이벤트 이름 (예: "폭발", "게임 시작")</param>
    public void LogBombStatus(string eventName = "")
    {
        int total = GetTotalBombCount();
        int active = GetActiveBombCount();
        int exploded = GetExplodedBombCount();

        string prefix = string.IsNullOrEmpty(eventName) ? "" : $"[{eventName}] ";
        Debug.Log($"<color=yellow>[BombManager]</color> {prefix}전체: <color=cyan>{total}</color> | " +
                  $"남은 폭탄: <color=green>{active}</color> | " +
                  $"폭발한 폭탄: <color=red>{exploded}</color>");
    }

#if UNITY_EDITOR
    [ContextMenu("폭탄 상태 확인")]
    public void DebugBombStatus()
    {
        LogBombStatus("수동 확인");
    }
#endif
}