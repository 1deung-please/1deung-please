using UnityEngine;

// 모달, 패널 오브젝트에 이 컴포넌트만 붙이면 SetActive로 켜지고 꺼질 때 자동으로 반영됨.
public class ModalRegistration : MonoBehaviour
{
    void OnEnable()
    {
        UIModalState.Register();
    }

    void OnDisable()
    {
        UIModalState.Unregister();
    }
}