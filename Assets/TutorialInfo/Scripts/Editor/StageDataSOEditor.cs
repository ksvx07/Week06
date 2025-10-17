#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageDataSO))]
public class StageDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 필드 먼저 표시
        DrawDefaultInspector();

        StageDataSO data = (StageDataSO)target;

        // 빌드 세팅에 등록된 씬 리스트 불러오기
        var scenes = EditorBuildSettings.scenes;
        string[] sceneNames = new string[scenes.Length];
        for(int i = 0; i < scenes.Length; i++)
        {
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
        }

        // 현재 선택된 씬 인덱스 찾기
        int currentIndex = Mathf.Max(0, System.Array.IndexOf(sceneNames, data.SceneName));

        // 팝업 표시
        int selectedIndex = EditorGUILayout.Popup("Scene Name", currentIndex, sceneNames);
        if (selectedIndex != currentIndex)
        {
            Undo.RecordObject(data, "Change Scene Name");
            data.SetSceneName(sceneNames[selectedIndex]);
            EditorUtility.SetDirty(data);
        }
    }
}
#endif
