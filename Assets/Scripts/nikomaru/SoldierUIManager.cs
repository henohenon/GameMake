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
    private VisualElement _logBox;
    private VisualElement _ItemBox_0;
    private VisualElement _ItemBox_1;
    private VisualElement _ItemBox_2;
    private Label _idLabel;
    private Label _Timer;
    private Label _clearText;
    private string _textEdit;
    private int maxLogCount = 6;       // 最大ログ数
    private float logLifetime = 5f;     // ログが消えるまでの時間（秒）

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _clearScreen = root.Q<VisualElement>("Clear");
        _gameOverScreen = root.Q<VisualElement>("GameOver");
        _logBox = root.Q<VisualElement>("LogBox");
        _idLabel = root.Q<Label>("ShareIDText");
        _Timer = root.Q<Label>("TimerText");
        _clearText = root.Q<Label>("Text_ClearTime");
        _ItemBox_0 = root.Q<VisualElement>("ItemBox0");
        _ItemBox_1 = root.Q<VisualElement>("ItemBox1");
        _ItemBox_2 = root.Q<VisualElement>("ItemBox2");

        //FlagのUI追加
        //_ItemBox_0.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>());

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

    public void AddLog(string text)
    {
        // 新しいログを作成
        var logLabel = new Label(text);
        _logBox.Add(logLabel);

        // 最大ログ数を超えたら古いログを削除
        if (_logBox.childCount > maxLogCount)
        {
            _logBox.RemoveAt(0);
        }
        // 一定時間後にログを削除する Coroutine を開始
        StartCoroutine(RemoveLogAfterTime(logLabel, logLifetime));
    }
    private IEnumerator RemoveLogAfterTime(Label logLabel, float delay)
    {
        yield return new WaitForSeconds(delay); // 指定時間待機

        if (_logBox.Contains(logLabel))    // ログがまだ存在していれば削除
        {
            _logBox.Remove(logLabel);
            Debug.Log("Log removed after delay.");
        }
    }


    public void SetItemIcon(ItemType type, int num, Sprite Icon)
    {

        switch (num)
        {
            case 1:
                _ItemBox_1.style.backgroundImage = new StyleBackground(Icon);
                break;

            case 2:
                _ItemBox_2.style.backgroundImage = new StyleBackground(Icon);
                break;

            default:
                Debug.Log("SetIcon failed.");
                break;
        }
    }

    public void RemoveItemIcon(int num)
    {
        switch (num)
        {
            case 1:
                _ItemBox_1.style.backgroundImage = new StyleBackground();
                break;

            case 2:
                _ItemBox_2.style.backgroundImage = new StyleBackground();
                break;

            default:
                Debug.Log("SetIcon failed.");
                break;
        }
    }

    public void SetSelect(int num)
    {
        _ItemBox_0.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f, 0.41f));
        _ItemBox_1.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f, 0.41f));
        _ItemBox_2.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f, 0.41f));
        
        switch (num)
        {
            case 0:
                _ItemBox_0.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f, 0.9f));
                break;
            case 1:
                _ItemBox_1.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f, 0.9f));
                break;

            case 2:
                _ItemBox_2.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f, 0.9f));
                break;

            default:
                Debug.Log("SetBackground failed.");
                break;
        }
        
    }


}

public enum InPlayScreenType
{
    GameClear,
    GameOver
}

