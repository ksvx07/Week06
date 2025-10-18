using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class FakeCursorClicker : MonoBehaviour
{
    // 1. 가짜 커서의 RectTransform
    [SerializeField]
    private RectTransform fakeCursorRect;

    void Update()
    {
        // 2. "클릭" 입력을 받으면 (예: 마우스 왼쪽 클릭 또는 스페이스바)
        if (Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0))
        {
            // 3. '가짜 커서'의 위치로 UI 레이캐스트를 실행
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = fakeCursorRect.position;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            Debug.Log("레이캐스트 결과 수: " + results.Count);
            // 4. 레이캐스트 결과 확인
            if (results.Count > 0)
            {
                // 5. 가장 위에 있는 UI 요소에서 'Button' 컴포넌트를 찾음
                Button clickedButton = results[0].gameObject.GetComponent<Button>();

                if (clickedButton != null)
                {
                    // 6. 찾았다면, 수동으로 OnClick 이벤트를 실행!
                    clickedButton.onClick.Invoke();
                    Debug.Log(clickedButton.name + "이(가) 수동으로 클릭되었습니다.");
                }
            }
        }
    }
}