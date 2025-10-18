using UnityEngine;

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

    // 스테이지 이미지 및 별 업데이트
    public void UpdateClearData(int starCount, string snapShotPath)
    {
        StageSaveManager.UpdateStageData(CurrentStageData, starCount, snapShotPath);
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