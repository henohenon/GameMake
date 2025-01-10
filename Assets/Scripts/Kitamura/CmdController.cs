using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CmdController : MonoBehaviour
{
    [SerializeField] private InputActionReference execInput;
    [SerializeField] private InputActionReference anyInput;
    [SerializeField] private InputActionReference previousInput;
    [SerializeField] private InputActionReference nextInput;
    private TextField _currentTextField = null;
    private UIDocument _uiDocument;
    
    // Start is called before the first frame update
    private void Start()
    {
        execInput.action.started += ctx => Exec();
        execInput.action.Enable();
        
        _uiDocument = GetComponent<UIDocument>();
        
        CreateNewLine();
    }

    private void Exec()
    {
        var input = _currentTextField.text;
        ExecuteCommand(input);
        
        CreateNewLine();
    }

    // 正規表現でecho "any text"形式を確認
    private const string MscPattern = @"^msc\s+""(.*?)""$";
    private const string EchoPattern = @"^echo\s+""(.*?)""$";
    private const string CdPattern = @"^cd\s+(.+)$";
    private const string LsPattern = @"^ls(\s+.+)$";
    private const string CatPattern = @"^cat\s+(.+)$";

    private void ExecuteCommand(string command)
    {
        var mscMatch = Regex.Match(command, MscPattern);
        if (mscMatch.Success)
        {
            var mscText = mscMatch.Groups[1].Value;
            if (uint.TryParse(mscText, out uint result))
            {
                CreateLog(result.ToString());
            }
            else
            {
                CreateLog(mscText + " is not a valid number");
            }
        }
        
        CreateLog("command not found");
    }

    private void CreateLog(string text)
    {
        var logText = new Label(text);
        _uiDocument.rootVisualElement.Add(logText);
    }
    
    private void CreateNewLine()
    {
        if (_currentTextField != null)
        {
            _currentTextField.isReadOnly = true;
        }
        
        var line = new TextField();
        _uiDocument.rootVisualElement.Add(line);
        _currentTextField = line;
        
        line.label = "multisweeper@commander:~/opt/msc $";
        
        line.Focus();
        line.Focus();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_currentTextField != null)
        {
            Debug.Log(_currentTextField.cursorPosition);
        }

    }

    private void OnDisable()
    {
        execInput.action.Disable();
    }
}
