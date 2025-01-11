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
    [SerializeField] private InputActionReference useItemInput;
    [SerializeField] private InputActionReference removeItemInput;
    [SerializeField] private SoldierUIManager soldirUIManager;
    [SerializeField] public AudioClip useitem;//アイテム使用時の音

    public AudioSource _audioSource;//アイテム使用時の音

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
    private int _selectingStackIndex = 0;

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
        useItemInput.action.started += _ => UseItem();
        useItemInput.action.Enable();
        // アイテム捨てるの入力
        removeItemInput.action.started += _ => RemoveItem();
        removeItemInput.action.Enable();
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
                //SoldierUIManagerにアイテムアイコンの表示依頼
                //soldirUIManagerSetItemIcon(string icon, int number)
                return;
            }
        }
        
        Debug.Log("Item stack is max");
    }

    private void RemoveItem()
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

#if UNITY_EDITOR
    [Button, EnableIf("IsPlaying")]
#endif
    private void UseItem()
    {
        var itemType = itemStack[_selectingStackIndex];
        // アイテムを実行
        itemEffectsManager.ExecItem(itemType);
        _audioSource.PlayOneShot(useitem);//アイテム使用時の音
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
        
        useItemInput.action.Disable();
        removeItemInput.action.Disable();
    }
}
