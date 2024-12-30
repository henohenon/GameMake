using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


/// <summary>
/// �{�^���C�x���g�̍ۂ̃V�[���J�ڂ��s���Ă܂��B�ŏ���Boolean�̓A�^�b�`�����I�u�W�F�N�g�ɂ���ėL��/�����̂��̂�����̂ł����𕪂��邽�߂ɂ���܂��B
/// </summary>
public class SceneLoader : MonoBehaviour//�j�R�}��
{
    public bool isTitle;
    public bool isCredit;
    public bool isClear;
    public bool isGameover;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;


        if (isTitle)
        {//���C�����j���[
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
        { //�Q�[���I�[�o�[���
            root.Q<Button>("Button_GameoverToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
        }

        if (isClear)
        { //�N���A���
            root.Q<Button>("Button_ClearToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
        }
    }
}
