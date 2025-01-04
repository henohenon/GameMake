using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CommanderUIManager : MonoBehaviour
{
    private UIDocument _document;

    private void Start()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;

        var applyButton = root.Q<Button>("Apply");
        var idInputField = root.Q<TextField>("IdInputField");

        var detailText = root.Q<Label>("Details");

        applyButton.clicked += () =>
        {
            Debug.Log(idInputField.value);
        };
    }
}
