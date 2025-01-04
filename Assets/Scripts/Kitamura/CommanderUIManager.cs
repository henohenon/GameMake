using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Scriptable;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CommanderUIManager : MonoBehaviour
{
    private UIDocument _document;
    [SerializeField, AssetsOnly] private GameRateAsset rateAsset;

    private void Start()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;

        var applyButton = root.Q<Button>("Apply");
        var idInputField = root.Q<TextField>("IdInputField");

        var detailText = root.Q<Label>("Details");

        applyButton.clicked += () =>
        {
            if (uint.TryParse(idInputField.value, out uint result))
            {
                var gameInfo = new GameInfo(rateAsset, 9, result);
                InfoLogger.LogGame(gameInfo, rateAsset);

            }
            else
            {
                Debug.LogWarning("Invalid input. Reverting to previous value.");
            }
        };
    }
}
