using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 스테이지 전용 초기화 
public class Stage: MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Image _stageImg;
    [SerializeField] private GameObject _clearStars;
    [SerializeField] private Sprite _filledStar;
    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private int _maxStars = 3;
    [SerializeField] private TextMeshProUGUI _stageNameText;
    #endregion

    private StageDataSO _stageDataSO;

    public void Init(StageDataSO stageSO)
    {
        _stageDataSO = stageSO;
        Debug.Log($"로드된 경로: {stageSO.StageImagePath}");


        // 이미지 반영
        _stageImg.sprite = stageSO.StageImage;
        _stageNameText.text = stageSO.StageName;

        // StageImagePath에 실제 파일이 존재한다면, Texture 로드
        if (!string.IsNullOrEmpty(stageSO.StageImagePath))
        {
            byte[] bytes = File.ReadAllBytes(stageSO.StageImagePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            stageSO.StageImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            Debug.Log($"[Init] StageImage 로드 완료: {stageSO.SceneName} ({stageSO.StageImagePath})");
        }else if (!File.Exists(stageSO.StageImagePath))
        {
            Debug.Log($"파일 자체 없음 {stageSO.StageImagePath}");
        }
        else
        {
            Debug.LogWarning($"[Init] StageImagePath가 비어 있거나 파일이 없음: {stageSO.SceneName} ({stageSO.StageImagePath})");
        }

        DrawClearStar();
    }

    private void DrawClearStar()
    {
        if (!_stageDataSO.IsTried)
        {
            _clearStars.SetActive(false);
            return;
        }

        _clearStars.SetActive(true);

        // 별 자식 Image들을 순회하며 sprite 변경
        int clearCount = Mathf.Clamp(_stageDataSO.ClearStar, 0, _maxStars);

        for (int i = 0; i < _maxStars; i++)
        {
            Image starImg = _clearStars.transform.GetChild(i).GetComponent<Image>();
            if (i < clearCount)
                starImg.sprite = _filledStar;
            else
                starImg.sprite = _emptyStar;
        }
    }

}