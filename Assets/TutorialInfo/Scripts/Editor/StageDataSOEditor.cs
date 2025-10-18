#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(StageDataSO))]
public class StageDataSOEditor : Editor
{
    private string[] sceneNames;
    private double lastRefreshTime = 0f;

    private void OnEnable()
    {
        RefreshSceneList();
        // 빌드 설정이 변경될 때 자동으로 새로고침
        EditorBuildSettings.sceneListChanged += RefreshSceneList;
    }

    private void OnDisable()
    {
        EditorBuildSettings.sceneListChanged -= RefreshSceneList;
    }

    private void RefreshSceneList()
    {
        // 너무 자주 호출되지 않게 (불필요한 Repaint 방지)
        if (EditorApplication.timeSinceStartup - lastRefreshTime < 0.2f)
            return;

        var scenes = EditorBuildSettings.scenes;
        sceneNames = new string[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
            sceneNames[i] = Path.GetFileNameWithoutExtension(scenes[i].path);

        lastRefreshTime = EditorApplication.timeSinceStartup;
    }

    public override void OnInspectorGUI()
    {
        // 기본 필드 출력
        DrawDefaultInspector();

        StageDataSO data = (StageDataSO)target;

        if (sceneNames == null || sceneNames.Length == 0)
            RefreshSceneList();

        if (sceneNames.Length == 0)
        {
            EditorGUILayout.HelpBox("빌드 세팅에 등록된 씬이 없습니다.", MessageType.Warning);
            return;
        }

        // 현재 선택된 씬 인덱스 찾기
        int currentIndex = Mathf.Max(0, System.Array.IndexOf(sceneNames, data.SceneName));

        EditorGUI.BeginChangeCheck();
        int selectedIndex = EditorGUILayout.Popup("Scene Name", currentIndex, sceneNames);
        if (EditorGUI.EndChangeCheck())
        {
            // 플레이 중에는 변경 저장 금지
            if (Application.isPlaying)
            {
                Debug.LogWarning("Play 모드 중에는 StageDataSO의 SceneName을 변경할 수 없습니다.");
                return;
            }

            Undo.RecordObject(data, "Change Scene Name");
            data.SetSceneName(sceneNames[selectedIndex]);
            EditorUtility.SetDirty(data);
        }
    }
}
#endif
