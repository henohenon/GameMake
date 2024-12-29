using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


/// <summary>
/// ボタンイベントの際のシーン遷移を行ってます。
/// </summary>
public class SceneLoader : MonoBehaviour//ニコマル
{
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        //メインメニュー
        root.Q<Button>("Button_Soldier").clicked += () => SceneManager.LoadScene("nikomaru");
        root.Q<Button>("Button_Commander").clicked += () => SceneManager.LoadScene("nikomaru");
        root.Q<Button>("Button_Options").clicked += () => SceneManager.LoadScene("nikomaru");
        root.Q<Button>("Button_Credit").clicked += () => SceneManager.LoadScene("nikomaru");
        //ゲームオーバー画面
        root.Q<Button>("Button_GameoverToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
        //クリア画面
        root.Q<Button>("Button_ClearToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
    }
    void Update()
    {
        
    }
}
