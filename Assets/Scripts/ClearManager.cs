using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearManager: MonoBehaviour
{
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _snapShotBtn;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Camera _snapShotCamera;

    private string _snapShotPath;


    public void ClearChecker()
    {
        
    }

    // 스테이지 끝나면 스냅샷
    public void SnapShot()
    {
        if (_snapShotCamera == null)
        {
            Debug.LogError("SnapShotCamera가 설정되어 있지 않습니다.");
            return;
        }

        // 카메라 위치 및 회전 동기화
        _snapShotCamera.transform.position = _targetTransform.position;
        _snapShotCamera.transform.rotation = _targetTransform.rotation;

        // 렌더링용 텍스처 생성
        int width = Screen.width;
        int height = Screen.height;
        RenderTexture rt = new RenderTexture(width, height, 24);
        _snapShotCamera.targetTexture = rt;

        // 스크린샷용 텍스처 준비
        Texture2D screenTex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // 카메라 렌더링 실행
        _snapShotCamera.Render();

        // RenderTexture -> Texture2D 복사
        RenderTexture.active = rt;
        screenTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenTex.Apply();

        // 카메라와 렌더텍스처 정리
        _snapShotCamera.targetTexture = null;
        RenderTexture.active = null;

        // RenderTexture 안전하게 해제
        rt.Release();
        Destroy(rt);

        // 저장 폴더 지정
        string folderPath = Path.Combine(Application.persistentDataPath, "StageImages");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // 파일명: 현재 씬 이름
        string sceneName = SceneManager.GetActiveScene().name;
        string fileName = $"{sceneName}.png";
        _snapShotPath = Path.Combine(folderPath, fileName);

        // PNG로 저장
        byte[] bytes = screenTex.EncodeToPNG();
        File.WriteAllBytes(_snapShotPath, bytes);

        Debug.Log($"스냅샷 저장 완료: {_snapShotPath}");

        // 메모리 정리
        Destroy(screenTex);

        // StageManager에 이미지 경로 업데이트
        StageManager.Instance.UpdateClearData(3, _snapShotPath);
    }


    private void OnEnable()
    {
        _exitBtn.onClick.AddListener(ExitBtn);
        _snapShotBtn.onClick.AddListener(SnapShot);
    }


    private void OnDisable()
    {
        _exitBtn.onClick.RemoveAllListeners();
        _snapShotBtn.onClick.RemoveAllListeners();
    }

    private void ExitBtn()
    {
        SceneManager.LoadScene("STAGE");
    }
}
