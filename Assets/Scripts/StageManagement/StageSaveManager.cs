using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class StageSaveManager
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "StageSaveData.json");

    // 저장
    public static void Save(List<StageDataSO> stages)
    {
        List<StageSaveData> saveList = new();
        foreach (var stage in stages)
        {
            saveList.Add(new StageSaveData
            {
                stageName = stage.StageName,
                isTried = stage.IsTried,
                clearStar = stage.ClearStar
            });
        }

        string json = JsonUtility.ToJson(new Wrapper { list = saveList }, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"스테이지 데이터 저장됨: {savePath}");
    }

    // 불러오기
    public static void Load(List<StageDataSO> stages)
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("처음 실행이므로 세이브 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(savePath);
        var wrapper = JsonUtility.FromJson<Wrapper>(json);

        foreach (var data in wrapper.list)
        {
            StageDataSO so = stages.Find(s => s.StageName == data.stageName);
            if (so != null)
            {
                so.IsTried = data.isTried;
                so.ClearStar = data.clearStar;
            }
        }

        Debug.Log($"세이브 데이터 로드 완료 ({wrapper.list.Count}개)");
    }

    // 스테이지 클리어 별 업데이트
    public static void UpdateStageData(StageDataSO stage, int earnedStars, string stageImagePath)
    {
        Wrapper wrapper;

        // 기존 데이터 호출
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            wrapper = JsonUtility.FromJson<Wrapper>(json);
        }
        else
        {
            wrapper = new Wrapper { list = new List<StageSaveData>() };
        }

        // 스테이지 검색 및 업데이트
        var existing = wrapper.list.Find(s => s.stageName == stage.StageName);
        if(existing != null)
        {
            existing.isTried = true;
            existing.clearStar = Mathf.Max(existing.clearStar, stage.ClearStar);
            existing.stageImagePath = stage.StageImagePath;
        }
        else
        {
            wrapper.list.Add(new StageSaveData
            {
                stageName = stage.StageName,
                isTried = stage.IsTried,
                clearStar = stage.ClearStar,
                stageImagePath = stage.StageImagePath
            });
        }

        string newJson = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, newJson);

        Debug.Log($"StageSaveManager: {stage.StageName} 저장 완료 ( {stage.ClearStar}, {stage.StageImagePath})");
    }


    [System.Serializable]
    private class Wrapper
    {
        public List<StageSaveData> list;
    }
}
