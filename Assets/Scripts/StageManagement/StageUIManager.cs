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
    #endregion

    #region Private Fields
    private List<StageDataSO> stageDataSOs = new();
    private List<Button> stageBtns = new();
    #endregion


    #region Private Methods
    // StageGroupSO 호출 및 UI 세팅
    void OnEnable()
    {
        SetStageUI();
    }

    void SetStageUI()
    {
        stageDataSOs = _stageGroupSO.stages;

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

        }
    }

    // 버튼에 이벤트 할당(씬 전환)
    void SetStageBtnEvent(string sceneName, StageDataSO stage)
    {
        stage.IsTried = true;
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

#endregion

    }
