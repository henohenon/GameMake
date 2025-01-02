using System.Collections;
using System.Collections.Generic;
using Scriptable;
using Unity.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemStackManager : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private ItemType[] itemStack = new[]
    {
        ItemType.Flag,
        ItemType.Empty,
        ItemType.Empty
    };
    private int _selectingStackIndex = 0;

    public void AddItem(ItemType addType)
    {
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
        if (_selectingStackIndex == 0)
        {
            Debug.LogError("Can't remove zero item");
        }

        itemStack[_selectingStackIndex] = ItemType.Empty;
    }

    public void UseItem()
    {
        if (_selectingStackIndex != 0)
        {
            itemStack[_selectingStackIndex] = ItemType.Empty;
        }
    }
}
