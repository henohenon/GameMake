using System;
using Alchemy.Inspector;
using Scriptable;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemStackManager : MonoBehaviour
{
    [SerializeField] private ItemEffectsManager itemEffectsManager;
    [SerializeField] private InputActionReference[] stackNumbInputs;
    [SerializeField] private InputActionReference useItemInput;
    [SerializeField] private InputActionReference removeItemInput;
    [SerializeField] private SoldierUIManager soldirUIManager;


#if UNITY_EDITOR
    // 実行中のみ編集を許可
    private static bool IsPlaying => EditorApplication.isPlaying;
    [SerializeField, EnableIf("IsPlaying")]
#endif
    private ItemType[] itemStack = new[]
    {
        ItemType.Flag,
        ItemType.Empty,
        ItemType.Empty
    };
    public int _selectingStackIndex = 0;

    private void Start()
    {
        // アイテム選択の入力
        for (var i = 0; i < stackNumbInputs.Length; i++)
        {
            // iだと全部最後の数字になっちゃうので無理やりコピーしていく
            var index = i;
            stackNumbInputs[i].action.started += _ => OnInputNumber(index);
            stackNumbInputs[i].action.Enable();
        }
        
        // アイテム使用の入力
        useItemInput.action.started += _ => UseItem(_selectingStackIndex);
        useItemInput.action.Enable();
        // アイテム捨てるの入力
        removeItemInput.action.started += _ => RemoveItem(_selectingStackIndex);
        removeItemInput.action.Enable();
    }

    private void OnInputNumber(int numb)
    {
        Debug.Log("Select item: "+numb);
        _selectingStackIndex = numb;
    }

    public void AddItem(ItemType addType, Sprite addIcon)
    {
        // 順番に見て言って、空いていたらそこに追加
        for (var i = 0; i < itemStack.Length; i++)
        {
            if (itemStack[i] == ItemType.Empty)
            {
                itemStack[i] = addType;
                soldirUIManager.SetItemIcon(addType, i, addIcon);

                Debug.Log("Add New Item: "+ addType + " at " + i);
                //SoldierUIManagerにアイテムアイコンの表示依頼
                //soldirUIManagerSetItemIcon(string icon, int number)
                return;
            }
        }
        
        Debug.Log("Item stack is max");
    }

    private void RemoveItem(int num)
    {
        var itemType = itemStack[_selectingStackIndex];
        // 旗なら捨てない
        if (itemType == ItemType.Flag)
        {
            Debug.Log("Can't remove flag item");
            return;
        }
        soldirUIManager.RemoveItemIcon(_selectingStackIndex);
        // 空にする
        itemStack[_selectingStackIndex] = ItemType.Empty;
    }

#if UNITY_EDITOR
    [Button, EnableIf("IsPlaying")]
#endif
    private void UseItem(int num)
    {
        var itemType = itemStack[_selectingStackIndex];
        // アイテムを実行
        itemEffectsManager.ExecItem(itemType);
        // 旗以外なら空にする
        if (itemType != ItemType.Flag)
        {
            itemStack[_selectingStackIndex] = ItemType.Empty;
        }
        soldirUIManager.RemoveItemIcon(_selectingStackIndex);
    }

    private void OnDisable()
    {
        foreach (var input in stackNumbInputs)
        {
            input.action.Disable();
        }
        
        useItemInput.action.Disable();
        removeItemInput.action.Disable();
    }
}
