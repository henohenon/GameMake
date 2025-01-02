using System;
using Alchemy.Inspector;
using Scriptable;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemStackManager : MonoBehaviour
{
    [SerializeField] private ItemEffectsManager itemEffectsManager;
    [SerializeField] private InputActionReference[] stackNumbInputs;
    
    // 実行中のみ編集を許可
    private static bool IsPlaying => EditorApplication.isPlaying;
    [SerializeField, EnableIf("IsPlaying")]
    private ItemType[] itemStack = new[]
    {
        ItemType.Flag,
        ItemType.Empty,
        ItemType.Empty
    };
    private int _selectingStackIndex = 0;

    private void Start()
    {
        for (var i = 0; i < stackNumbInputs.Length; i++)
        {
            // iだと全部最後の数字になっちゃうので無理やりコピーしていく
            var index = i;
            stackNumbInputs[i].action.started += _ => OnInputNumber(index);
            stackNumbInputs[i].action.Enable();
        }
    }

    private void OnInputNumber(int numb)
    {
        Debug.Log("Select item: "+numb);
        _selectingStackIndex = numb;
    }

    public void AddItem(ItemType addType)
    {
        // 順番に見て言って、空いていたらそこに追加
        for (var i = 0; i < itemStack.Length; i++)
        {
            if (itemStack[i] == ItemType.Empty)
            {
                itemStack[i] = addType;

                Debug.Log("Add New Item: "+ addType + " at " + i);
                return;
            }
        }
        
        Debug.Log("Item stack is max");
    }

    public void RemoveItem()
    {
        var itemType = itemStack[_selectingStackIndex];
        // 旗なら捨てない
        if (itemType == ItemType.Flag)
        {
            Debug.Log("Can't remove flag item");
            return;
        }
        // 空にする
        itemStack[_selectingStackIndex] = ItemType.Empty;
    }

    public void UseItem()
    {
        var itemType = itemStack[_selectingStackIndex];
        // アイテムを実行
        itemEffectsManager.ExecItem(itemType);
        // 旗以外なら空にする
        if (itemType != ItemType.Flag)
        {
            itemStack[_selectingStackIndex] = ItemType.Empty;
        }
    }

    private void OnDisable()
    {
        foreach (var input in stackNumbInputs)
        {
            input.action.Disable();
        }
    }
}
