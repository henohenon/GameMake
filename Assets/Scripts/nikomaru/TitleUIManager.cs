using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


/// <summary>
/// ボタンイベントの際のシーン遷移を行ってます。最初のBooleanはアタッチされるオブジェクトによって有効/無効のものがあるのでそれらを分けるためにあります。
/// </summary>
public class TitleUIManager : MonoBehaviour//ニコマル
{
    public bool isTitle;
    public bool isCredit;
    public bool isClear;
    public bool isGameover;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;


        if (isTitle)
        {//メインメニュー
            GameObject creditScreen = GameObject.Find("UIDocument_Credit");
            GameObject test = GameObject.Find("UIDocument");
            creditScreen.GetComponent<UIDocument>().enabled = false;
            root.Q<Button>("Button_Options").clicked += () => test.SetActive(false);
            root.Q<Button>("Button_Soldier").clicked += () => SceneManager.LoadScene("nikomaru");
            root.Q<Button>("Button_Commander").clicked += () => SceneManager.LoadScene("nikomaru");
            root.Q<Button>("Button_Credit").clicked += () => creditScreen.GetComponent<UIDocument>().enabled = true;

        }

        if (isCredit)
        {
            GameObject creditScreen = GameObject.Find("UIDocument_Credit");
            root.Q<Button>("Button_CloseCredit").clicked += () => creditScreen.GetComponent<UIDocument>().enabled = false;

        }

        if (isGameover) 
        { //ゲームオーバー画面
            root.Q<Button>("Button_GameoverToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
        }

        if (isClear)
        { //クリア画面
            root.Q<Button>("Button_ClearToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
        }
    }
}
