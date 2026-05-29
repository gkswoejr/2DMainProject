using System.Collections.Generic;
using UnityEngine;

public class DaniTech_HudUI : DaniTechUIBase
{

    [SerializeField] private GameObject Prefab_HudSlot;
    [SerializeField] private Transform Transform_SlotRoot;

    //[SerializeField] private GameObject Prefab_SlotRoot_Monster;


    private Dictionary<int, DaniTech_HudSlotUI> _hudSlotList = new Dictionary<int, DaniTech_HudSlotUI>();

    public void AddHudSlot(int instanceId, Transform tagetTranform)
    {
        CreateHudSlot(instanceId,tagetTranform);
    }
    public void CreateHudSlot(int instanceId, Transform tagetTranform)
    {
        var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<DaniTech_HudSlotUI>();
        if (slotComponent == null) return;
        //자식 슬롯 프리팹을 동적 생성하고 컴포넌트도 잘 가져오고 있다

        slotComponent.InitSlot(instanceId, tagetTranform);

        _hudSlotList.Add(instanceId, slotComponent);
    }


    public void RemoveHudSlot(int instanceId)
    {
        if(_hudSlotList.ContainsKey(instanceId) == true)
        {
            var slot = _hudSlotList[instanceId];

            Destroy(slot.gameObject);
            _hudSlotList.Remove(instanceId);
        }
    }

}
