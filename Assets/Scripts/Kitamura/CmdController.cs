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

    /*
.88b  d88. .d8888.         .o88b.  .d88b.  .88b  d88. .88b  d88.  .d8b.  d8b   db d8888b. d88888b d8888b.
88'YbdP`88 88'  YP        d8P  Y8 .8P  Y8. 88'YbdP`88 88'YbdP`88 d8' `8b 888o  88 88  `8D 88'     88  `8D
88  88  88 `8bo.          8P      88    88 88  88  88 88  88  88 88ooo88 88V8o 88 88   88 88ooooo 88oobY'
88  88  88   `Y8b. C8888D 8b      88    88 88  88  88 88  88  88 88~~~88 88 V8o88 88   88 88~~~~~ 88`8b
88  88  88 db   8D        Y8b  d8 `8b  d8' 88  88  88 88  88  88 88   88 88  V888 88  .8D 88.     88 `88.
YP  YP  YP `8888Y'         `Y88P'  `Y88P'  YP  YP  YP YP  YP  YP YP   YP VP   V8P Y8888D' Y88888P 88   YD

     */
    
    
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
