using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


/// <summary>
/// �{�^���C�x���g�̍ۂ̃V�[���J�ڂ��s���Ă܂��B
/// </summary>
public class SceneLoader : MonoBehaviour//�j�R�}��
{
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        //���C�����j���[
        root.Q<Button>("Button_Soldier").clicked += () => SceneManager.LoadScene("nikomaru");
        root.Q<Button>("Button_Commander").clicked += () => SceneManager.LoadScene("nikomaru");
        root.Q<Button>("Button_Options").clicked += () => SceneManager.LoadScene("nikomaru");
        root.Q<Button>("Button_Credit").clicked += () => SceneManager.LoadScene("nikomaru");
        //�Q�[���I�[�o�[���
        root.Q<Button>("Button_GameoverToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
        //�N���A���
        root.Q<Button>("Button_ClearToMenu").clicked += () => SceneManager.LoadScene("TitleScene");
    }
    void Update()
    {
        
    }
}
