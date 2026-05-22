using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class DaniTech_GameBookUI : DaniTechUIBase
{
    [Header("프리팹")]
    [SerializeField] private GameObject Prefab_GameBookSlot;

    [Header("닫기 버튼")]
    [SerializeField] private DaniTechUIButton Button_Quit;


    [Header("디테일 정보")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_MainDescription;

    [Header("부가 정보")]
    [SerializeField] private GameObject Layout_SubInfo ;


    [Header("슬롯 리스트 영억")]
    [SerializeField] private Transform Transform_SlotRoot;

    private Dictionary<string, DaniTech_GameBookSlotUI> _slotList = new Dictionary<string, DaniTech_GameBookSlotUI> ();

    private void OnEnable()
    {
        ReadItemListAndCreateSlot();
        Button_Quit.BindOnClickButtonEvent(OnClick_CloseGameBookUI);
        // 이 UI가 열릴 때 기본적으로 도감안에 있는 모든 데이터를 불러온다
    }

    private void OnDisable()
    {
        if(_slotList.Count > 0)
        {
            foreach (var slotkv in _slotList) 
            {
                var slot = slotkv.Value;
                DestroyImmediate(slot.gameObject);
            }
            _slotList.Clear();
        }
    }

    private void OnClick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNGameBookUI);
    }

    private void ReadItemListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if(data == null)
            {
                continue;
            }
            CreateGameBookSlot(data.Id);
        }

        if (_slotList.Count > 0)
        {
            foreach(var slotkv in _slotList)
            {
                var slot = slotkv.Value;
                slot.OnClick_GameBookSlot();
            }
        }
    }
    private void CreateGameBookSlot(string dataId)
    {
        var gObj = Instantiate(Prefab_GameBookSlot,Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<DaniTech_GameBookSlotUI>();
        if (slotComponent == null) return;
        //자식 슬롯 프리팹을 동적 생성하고 컴포넌트도 잘 가져오고 있다

        slotComponent.InitSlot(dataId,OnClickChildSlotSelected);
        _slotList.Add(dataId, slotComponent );



    }

    private void OnClickChildSlotSelected(string slotDataId)
    {
        var currentSalectedDate = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
        if (currentSalectedDate == null) return;

        Text_MainName.text = currentSalectedDate.Name;
        Text_MainDescription.text = currentSalectedDate.Description;

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon,currentSalectedDate.IconPath).Forget();

        foreach (var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);
        }
    }
}
