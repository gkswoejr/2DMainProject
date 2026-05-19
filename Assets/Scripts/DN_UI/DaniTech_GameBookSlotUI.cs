using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


public class DaniTech_GameBookSlotUI : MonoBehaviour
{

    [Header("슬롯기본 정보")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private GameObject GObj_Selected;
    [SerializeField] private DaniTechUIButton Button_SlotClick;


    private event Action<string> _onClickSlot;

    private string _slotDataId; //슬롯이 살아있는 동안 어떤 슬롯인지 아는 용도

    public string GetSlotDataId()
    {
        return _slotDataId;
    }
    private void OnEnable()
    {
        Button_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSlot);
    }

    public void OnClick_GameBookSlot()
    {
        _onClickSlot?.Invoke(_slotDataId);
    }
    private void OnDisable()
    {
        _onClickSlot = null;
    }


    public void InitSlot(string dataId ,Action<string> onClickCallback /*TableType*/) //TODO : 카테고리에 따라 다른 정보를 전달하는 방법도 생각하자
    {
        var itenData = DaniTechGameDataManager.Instance.GetDNItemData( dataId );
        if ( itenData == null ) return;
        
        Text_MainName.text = itenData.Name; //이름변경

        string iconPath = itenData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true) return;

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon,iconPath).Forget();

        _slotDataId = dataId;
        //데이터를 받아왔으면 잘 보관해두자.

        //Text_MainName.text = 
        _onClickSlot += onClickCallback;
    }

    public void SetSelectedUI(bool isSelect)
    {
        GObj_Selected.SetActive( isSelect );
    }
}
