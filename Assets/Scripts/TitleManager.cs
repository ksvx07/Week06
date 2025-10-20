using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    bool isStart = false;
    private void OnEnable()
    {
        BombManager.Instance.OnBombCountChanged += RemainBombUpdate;
    }

    private void OnDisable()
    {
        BombManager.Instance.OnBombCountChanged -= RemainBombUpdate;
    }

    void RemainBombUpdate(int remainBomb)
    {
        if (remainBomb <= 0)
        {
            if (isStart) return;
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("STAGE");
    }
}
