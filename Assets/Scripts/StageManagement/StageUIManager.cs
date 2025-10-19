using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private GameObject _stagePrefab;
    [SerializeField] private GameObject _stageTarget;
    [SerializeField] private StageGroupSO _stageGroupSO;
    [SerializeField] private Button _exitBtn;
    #endregion

    #region Private Fields
    private List<StageDataSO> stageDataSOs = new();
    private List<Button> stageBtns = new();
    private bool isAllCleared = false;  // 히든 스테이지 해금용도
    private GameObject hiddenStage;
    #endregion

    #region Initialize Methods

    // StageGroupSO 호출 및 UI 세팅
    void OnEnable()
    {
        stageDataSOs = _stageGroupSO.stages;
        StageSaveManager.Load(stageDataSOs);

        SetStageUI();

        _exitBtn.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        _exitBtn.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        // F1 누르면 히든 스테이지 해제
        if (Input.GetKeyDown(KeyCode.F1)){
            ActiveHiddenStage();
        }
    }
    #endregion

    #region Private Methods
    void SetStageUI()
    {

        AllStagesCleared();

        // 스테이지 프리팹 Instantiage 및 초기화
        foreach (var stage in stageDataSOs)
        {
            GameObject obj = Instantiate(_stagePrefab, _stageTarget.transform);

            Stage objStage = obj.GetComponent<Stage>();
            objStage.Init(stage);

            // 버튼 씬 전환 이벤트 등록
            Button objBtn = obj.GetComponentInChildren<Button>();
            stageBtns.Add(objBtn);

            objBtn.onClick.AddListener(() => SetStageBtnEvent(stage.SceneName, stage));

            // 히든 스테이지 해제 확인
            if(stage.SceneName == "JMKey")
            {
                hiddenStage = obj;
                if (!isAllCleared)
                {
                    obj.SetActive(false);
                }
            }

        }
    }

    // 버튼에 이벤트 할당(씬 전환)
    void SetStageBtnEvent(string sceneName, StageDataSO stage)
    {
        StageManager.Instance.SetStageData(stage, stageDataSOs);
        SceneManager.LoadScene(sceneName);
        ClearStageBtnEvent();
    }

    // 버튼에 할당한 이벤트 해제
    void ClearStageBtnEvent()
    {
        foreach (var stageBtn in stageBtns)
        {
            stageBtn.onClick.RemoveAllListeners();
        }
    }

    // 히든 스테이지[Key]해제 용, UI 세팅 전 호출 필요
    void AllStagesCleared()
    {
        int starCount = 0;
        int maxStar = (stageDataSOs.Count - 1) * 3;
        foreach(var stage in stageDataSOs)
        {
            if(stage.ClearStar < 3)
            {
                return;
            }

            starCount += stage.ClearStar;
        }

        if(starCount < maxStar)
        {
            return;
        }

        isAllCleared = true;
        return;
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    #endregion

    #region 디버깅용
    private void ActiveHiddenStage()
    {
        isAllCleared = true;
        hiddenStage.SetActive(true);
    }
    #endregion

}
