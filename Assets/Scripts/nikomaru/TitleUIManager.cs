using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


/// <summary>
/// ボタンイベントの際のシーン遷移を行ってます。最初のBooleanはアタッチされるオブジェクトによって有効/無効のものがあるのでそれらを分けるためにあります。
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class TitleUIManager : MonoBehaviour//ニコマル
{
    private VisualElement creditScreen;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        creditScreen = root.Q<VisualElement>("Credit_Screen");
        
        //メインメニュー
        root.Q<Button>("Button_Credit").clicked += () => SetHiddenCredit(false);
        root.Q<Button>("Button_Soldier").clicked += LoadGameScene;
        root.Q<Button>("Button_Commander").clicked += LoadGameScene;

        // クレジット画面
        root.Q<Button>("Button_CloseCredit").clicked += () => SetHiddenCredit(true);
    }
    
    private void LoadGameScene()
    {
        SceneManager.LoadScene("nikomaru");
    }

    private void SetHiddenCredit(bool isHidden = true)
    {
        if (isHidden)
        {
            creditScreen.AddToClassList("hidden");
        }
        else
        {
            creditScreen.RemoveFromClassList("hidden");
        }
    }
}
