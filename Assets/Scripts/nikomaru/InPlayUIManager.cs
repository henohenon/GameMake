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
    
    private void LoadGameScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void SetHiddenCredit(int num, bool isHidden = true)
    {
        Debug.Log("InPlay");
        Debug.Log(num);
        Debug.Log(isHidden);
        switch (num)
        {
            case 1:
                if (isHidden)
                {
                    ClearScreen.AddToClassList("hidden");
                }
                else
                {
                    ClearScreen.RemoveFromClassList("hidden");
                }
                break;
            case 2:
                if (isHidden)
                {
                    GameoverScreen.AddToClassList("hidden");
                }
                else
                {
                    GameoverScreen.RemoveFromClassList("hidden");
                }
                break;
        }
    }
}


