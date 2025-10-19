using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FakeCursorClicker : MonoBehaviour
{
    [SerializeField]
    private RectTransform fakeCursorRect;

    void Update()
    {
        if (Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = fakeCursorRect.position;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                // 레이캐스트에 맞은 첫 번째 객체 (아마도 Text)
                GameObject target = results[0].gameObject;
                GameObject clickHandlerObject = null; // 클릭 이벤트를 실행할 객체

                // 1. target(Text)에 IPointerClickHandler가 있는지 확인
                IPointerClickHandler clickHandler = target.GetComponent<IPointerClickHandler>();

                if (clickHandler != null)
                {
                    // Text 자체에 클릭 핸들러가 있다면
                    clickHandlerObject = target;
                }
                else if (target.transform.parent != null)
                {
                    // 2. 부모 객체(Button)에 IPointerClickHandler가 있는지 확인
                    clickHandler = target.transform.parent.GetComponent<IPointerClickHandler>();
                    if (clickHandler != null)
                    {
                        // 부모(Button)에 핸들러가 있다면
                        clickHandlerObject = target.transform.parent.gameObject;
                    }
                }

                // 3. 클릭 핸들러를 찾았다면 이벤트 전송
                if (clickHandlerObject != null)
                {
                    ExecuteEvents.Execute(clickHandlerObject, pointerData, ExecuteEvents.pointerClickHandler);
                    Debug.Log(clickHandlerObject.name + "에 클릭 이벤트를 전송했습니다.");
                }
            }
        }
    }
}