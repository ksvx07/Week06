using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Create SO/StageData", order = 1)]
public class StageDataSO : ScriptableObject
{
    #region Serialized Fields
    [SerializeField] private Sprite _stageImage;
    [SerializeField] private string _stageName;
    [SerializeField] private int _clearStar = 0;
    [SerializeField] private bool _isTried = false;
    #endregion

    #region 내부 변수
    private string _sceneName;
    #endregion

    #region 외부 전용 반환 메소드
    public Sprite StageImage
    {
        get => _stageImage;
        set => _stageImage = value;
    }

    public string StageName
    {
        get => _stageName;
        set => _stageName = value;
    }

    public int ClearStar
    {
        get => _clearStar;
        set => _clearStar = value;
    }

    public bool IsTried
    {
        get => _isTried;
        set => _isTried = value;
    }

    public string SceneName => _sceneName;

    #endregion

    #region Editor 전용 외부 메소드
    public void SetSceneName(string name)
    {
        _sceneName = name;
    }
    #endregion

}
