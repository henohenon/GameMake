using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


/// <summary>
/// ボタンイベントの際のシーン遷移を行ってます。最初のBooleanはアタッチされるオブジェクトによって有効/無効のものがあるのでそれらを分けるためにあります。
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class SoldierUIManager : MonoBehaviour//ニコマル
{
    private VisualElement _clearScreen;
    private VisualElement _gameOverScreen;
    private Label _idLabel;
    private Label _Timer;
    private Label _clearText;
    private string _textEdit;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _clearScreen = root.Q<VisualElement>("Clear");
        _gameOverScreen = root.Q<VisualElement>("GameOver");
        _idLabel = root.Q<Label>("ShareIDText");
        _Timer = root.Q<Label>("TimerText");
        _clearText = root.Q<Label>("Text_ClearTime");

        //クリア
        root.Q<Button>("Button_ClearToMenu").clicked += LoadTitleScene;

        // ゲームオーバー
        root.Q<Button>("Button_GameoverToMenu").clicked += LoadTitleScene;
    }

    public void SetShareID(uint seed)
    {
        _idLabel.text = seed.ToString();
    }
    
    private void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
    
    // TODO: コールバックからにしたい
    public void SetPopupHidden(InPlayScreenType type, bool isHidden = true)
    {
        VisualElement screen = null;
        switch (type)
        {
            case InPlayScreenType.GameClear:
                screen = _clearScreen;
                break;
            case InPlayScreenType.GameOver:
                screen = _gameOverScreen;
                break;
            default:
                Debug.LogError("Screen does not found");
                return;
        }
        
        if (isHidden)
        {
            screen.AddToClassList("hidden");
        }
        else
        {
            screen.RemoveFromClassList("hidden");
        }
    }

    //
    public void UpdateTimer(string CurrentTime)
    {
        _Timer.text = CurrentTime;
        _textEdit = "Clear  Time " + CurrentTime;
        _clearText.text = _textEdit;
    }

}

public enum InPlayScreenType
{
    GameClear,
    GameOver
}

