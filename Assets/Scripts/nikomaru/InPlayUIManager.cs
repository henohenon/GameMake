using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


/// <summary>
/// ボタンイベントの際のシーン遷移を行ってます。最初のBooleanはアタッチされるオブジェクトによって有効/無効のものがあるのでそれらを分けるためにあります。
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class InPlayUIManager : MonoBehaviour//ニコマル
{
    private VisualElement ClearScreen;
    private VisualElement GameoverScreen;
    private Label idLabel;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        ClearScreen = root.Q<VisualElement>("Clear");
        GameoverScreen = root.Q<VisualElement>("GameOver");

        //クリア
        root.Q<Button>("Button_ClearToMenu").clicked += LoadGameScene;

        // ゲームオーバー
        root.Q<Button>("Button_GameoverToMenu").clicked += () => LoadGameScene();
    }

    public void SetShareID(uint seed)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        idLabel = root.Q<Label>("ShareIDText");
        idLabel.text = seed.ToString();
    }
    
    private void LoadGameScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
    
    public void SetHiddenCredit(InPlayScreenType type, bool isHidden = true)
    {
        VisualElement screen = null;
        switch (type)
        {
            case InPlayScreenType.GameClear:
                screen = ClearScreen;
                break;
            case InPlayScreenType.GameOver:
                screen = GameoverScreen;
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
}

public enum InPlayScreenType
{
    GameClear,
    GameOver
}

