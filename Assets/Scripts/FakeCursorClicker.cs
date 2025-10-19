using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FakeCursorClicker : MonoBehaviour
{
    [SerializeField]
    private RectTransform fakeCursorRect;

    void Update()
    {
        // "Submit" 또는 마우스 왼쪽 버튼을 눌렀을 때 (프레임 시작 시)
        if (Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = fakeCursorRect.position;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                // 레이캐스트에 맞은 가장 위에 있는 객체
                GameObject target = results[0].gameObject;

                // --- 여기부터 수정 ---

                // 1. "Pointer Down" 이벤트 전송 (슬라이더 등이 반응)
                // GetEventHandler가 계층 구조를 따라 올라가며 IPointerDownHandler를 찾습니다.
                GameObject downHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(target);
                if (downHandler != null)
                {
                    ExecuteEvents.Execute(downHandler, pointerData, ExecuteEvents.pointerDownHandler);
                    Debug.Log(downHandler.name + "에 PointerDown 전송");
                }

                // 2. "Pointer Click" 이벤트 전송 (버튼 등이 반응)
                // GetEventHandler가 IPointerClickHandler를 찾습니다.
                GameObject clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
                if (clickHandler != null)
                {
                    ExecuteEvents.Execute(clickHandler, pointerData, ExecuteEvents.pointerClickHandler);
                    Debug.Log(clickHandler.name + "에 PointerClick 전송");
                }

                // 3. "Pointer Up" 이벤트 전송 (클릭 완료)
                // GetEventHandler가 IPointerUpHandler를 찾습니다.
                // Down과 Up을 한 프레임에 보내면 '클릭'으로 인식됩니다.
                GameObject upHandler = ExecuteEvents.GetEventHandler<IPointerUpHandler>(target);
                if (upHandler != null)
                {
                    ExecuteEvents.Execute(upHandler, pointerData, ExecuteEvents.pointerUpHandler);
                    Debug.Log(upHandler.name + "에 PointerUp 전송");
                }

                // --- 수정 끝 ---
            }
        }
    }
}