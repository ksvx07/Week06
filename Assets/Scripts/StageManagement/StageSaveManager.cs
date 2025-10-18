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

    [System.Serializable]
    private class Wrapper
    {
        public List<StageSaveData> list;
    }
}
