using System;
using UnityEngine;

public class DaniTech_GameMonsterBase_Collider : MonoBehaviour
{
    public event Action<Collider2D> OnTriggerStayEvent;

    public event Action<Collider2D> OnTriggerEnterEvent;

    public event Action<Collider2D> OnTriggerExitEvent;



    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnTriggerStayEvent?.Invoke(other);

        }
        // 이벤트에 등록된 부모 메서드가 있다면 실행해라!
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnTriggerEnterEvent?.Invoke(other);

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            OnTriggerExitEvent?.Invoke(other);
        }
    }
}
