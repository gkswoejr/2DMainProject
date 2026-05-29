using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DaniTech_HudSlotUI : MonoBehaviour
{
    [SerializeField] private int SlotOffsetY;

    [SerializeField] private GameObject Layout_BuffSlot;
    [SerializeField] private Slider Slider_Hp;
    [SerializeField] private Slider Slider_Sp;
    [SerializeField] private TextMeshProUGUI _textMesh_Hp;


    private int _instanceId;
    private Transform _targetTransform;


    public void InitSlot(int instanceId, Transform targetTranform)
    {
        _instanceId = instanceId;
        _targetTransform = targetTranform;
        SlotOffsetY = 120;

        TryBindStatChangedEvent(targetTranform.gameObject);
    }

    private void TryBindStatChangedEvent(GameObject gObj)
    {
        var player = gObj.GetComponent<DaniTech_2DPlayer>();
        if (player != null)
        {
            player.BindOnstatChangedEvent(OnTargetEntityHpChange, OnTargetEntitySpChange);
            return;
        }
        var monster = gObj.GetComponent<DaniTech_GameMonster_Dog>();
        if (monster != null)
        {
            monster.BindOnstatChangedEvent(OnTargetEntityHpChange, OnTargetEntitySpChange);
            return;
        }

        
    }

    private void OnTargetEntityHpChange(int curHp, int maxHp)
    {
        Slider_Hp.value = (curHp/(float)maxHp);
        _textMesh_Hp.text = curHp.ToString();

    }

    private void OnTargetEntitySpChange(int curSp, int maxSp)
    {
        Slider_Sp.value = (curSp / (float)maxSp);

    }

    private void Update()
    {
        if (_targetTransform != null) 
        {
            this.gameObject.transform.position = _targetTransform.position;


            Vector2 screenPos = Camera.main.WorldToScreenPoint(_targetTransform.position);

            // UGUI에서 사용하려고
            var rectTransform = this.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 finalScreenPos = new Vector2(screenPos.x, screenPos.y - SlotOffsetY);
                rectTransform.anchoredPosition = finalScreenPos;
            }
        }
    }
}
