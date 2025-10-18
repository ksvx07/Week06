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
    
    // so를 통해서 트라이도 하지 않았으면 setActive(flase)
    // 했으면 setActive(true)하고 별 그리기 메소드 호출
    // 별은 최대 3개, 3 - fill 개수 = empty 별로 오른쪽부터 채우기



    public void Init(StageDataSO stageSO)
    {
        _stageDataSO = stageSO;

        _stageImg.sprite = stageSO.StageImage;
        _stageNameText.text = stageSO.StageName;

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