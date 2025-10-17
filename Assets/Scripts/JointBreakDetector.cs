using UnityEngine;

public class JointBreakDetector : MonoBehaviour
{
    private void OnJointBreak(float breakForce)
    {
        Debug.LogWarning("조인트가 끊어졌습니다! 가해진 힘: " + breakForce);
        // 싱글톤 인스턴스가 존재하는지 확인하고,
        // PhysicsDragFinal의 공개 함수 NotifyJointBroken()를 직접 호출합니다.
        if (PhysicsDrag.Instance != null)
        {
            PhysicsDrag.Instance.NotifyJointBroken();
        }
    }
}