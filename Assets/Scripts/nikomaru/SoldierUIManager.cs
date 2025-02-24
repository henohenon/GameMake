using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Scriptable;


/// <summary>
/// ボタンイベントの際のシーン遷移を行ってます。最初のBooleanはアタッチされるオブジェクトによって有効/無効のものがあるのでそれらを分けるためにあります。
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class SoldierUIManager : MonoBehaviour//ニコマル
{
    private VisualElement _root;
    private VisualElement _gameClearScreen;
    private VisualElement _gameOverScreen;
    private VisualElement _logBox;
    private List<VisualElement> _itemIcons;
    private Label _idLabel;
    private Label _timer;
    private Label _clearLabel;
    
    private Color  _customRed = new Color(1.0f, 0f, 0.27f, 1.0f);
    private Color _customBlue = new Color(0f, 0.63f, 0.6f, 1.0f);
    private Color _customGreen = new Color(0.6f, 1.0f, 0.53f, 1.0f);
    private readonly int maxLogCount = 6;       // 最大ログ数
    private readonly float logLifetime = 5f;     // ログが消えるまでの時間（秒）
    [SerializeField] private GameRateAsset gameRateAsset;
    [SerializeField] private AudioSource clickSound;

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _gameClearScreen = _root.Q<VisualElement>("GameClear");
        _gameOverScreen = _root.Q<VisualElement>("GameOver");
        _logBox = _root.Q<VisualElement>("LogBox");
        _idLabel = _root.Q<Label>("ShareIDText");
        _timer = _root.Q<Label>("TimerText");
        _clearLabel = _root.Q<Label>("Text_Clear");
        _itemIcons = new ()
        {
            _root.Q<VisualElement>("ItemIcon0"),
            _root.Q<VisualElement>("ItemIcon1"),
            _root.Q<VisualElement>("ItemIcon2")
        };

        //FlagのUI追加
        //_ItemBox_0.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>());

        //クリア
        _root.Q<Button>("Button_ClearToNextGame").clicked += ReLoadGameScene;
        _root.Q<Button>("Button_ClearToMenu").clicked += LoadTitleScene;
        _root.Q<Button>("Button_Share").clicked += () =>
        {
            clickSound.Play();
            naichilab.UnityRoomTweet.Tweet("multi-sweeper", $"MISSION  COMPLETE!\nTime. {_timer.text}, Soldier. {StaticValues.deathCount:D3}");
        };
        
        // ゲームオーバー
        _root.Q<Button>("Button_GameoverToNextGame").clicked += ReLoadGameScene;
        _root.Q<Button>("Button_GameoverToMenu").clicked += LoadTitleScene;

    }

    public void SetShareID(uint seed)
    {
        _idLabel.text = seed.ToString();
    }

    private void LoadTitleScene()
    {
        clickSound.Play();
        SceneManager.LoadScene("TitleScene");
    }
    
    private void ReLoadGameScene()
    {
        clickSound.Play();
        SceneManager.LoadScene("Soldier");
    }

    // TODO: コールバックからにしたい
    public void SetPopupHidden(InPlayScreenType type, bool isHidden = true)
    {
        VisualElement screen = null;
        switch (type)
        {
            case InPlayScreenType.GameClear:
                screen = _gameClearScreen;
                _root.AddToClassList("gameClear");
                LMotion.String.Create128Bytes("", "MISSION  COMPLETE", 2f)
                    .WithScrambleChars(ScrambleMode.Uppercase)
                    .BindToText(_clearLabel);
                break;
            case InPlayScreenType.GameOver:
                screen = _gameOverScreen;
                _root.AddToClassList("gameOver");
                StaticValues.deathCount++;
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
        _timer.text = CurrentTime;
    }

    public void AddLog(string text, ColorType colorType)
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
        
        //文字の色を指定
        switch (colorType)
        {
            case ColorType.Red:
                logLabel.AddToClassList("logRed");
                break;
            case ColorType.Blue:
                logLabel.AddToClassList("logBlue");
                break;
            case ColorType.Green:
                logLabel.AddToClassList("logGreen");
                break;
        }
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


    public void SetItemIcon(ItemType type, int num, Sprite icon)
    {
        _itemIcons[num].style.backgroundImage = new StyleBackground(icon);
    }

    public void RemoveItemIcon(int num)
    {
        _itemIcons[num].style.backgroundImage = new StyleBackground();
    }
    
    public void SetSelect(int num)
    {
        // iconsよりbox側にactiveつけるべきだが実装上単純な法を選択
        _itemIcons.ForEach(icon => icon.RemoveFromClassList("active"));
        _itemIcons[num].AddToClassList("active");
    }
}

public enum InPlayScreenType
{
    GameClear,
    GameOver
}

public enum ColorType
{
    Red,
    Blue,
    Green
}

