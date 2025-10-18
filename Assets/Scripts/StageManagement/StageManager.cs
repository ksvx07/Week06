using UnityEngine;

// so를 직접 수정하는거 주의하기.
public class StageManager : MonoBehaviour
{
    #region Public Fields
    public static StageManager Instance { get; private set; }
    public StageDataSO CurrentStageData { get; private set; }
    #endregion

    #region Public Methods
    public void SetStageData(StageDataSO data)
    {
        CurrentStageData = data;
    }
    #endregion

    #region Private Methods
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}