using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;


/// <summary>
/// ボタンイベントの際のシーン遷移を行ってます。最初のBooleanはアタッチされるオブジェクトによって有効/無効のものがあるのでそれらを分けるためにあります。
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class TitleUIManager : MonoBehaviour//ニコマル
{
    private VisualElement _creditScreen;
    private TutorialUIManager _tutorialManager;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _creditScreen = root.Q<VisualElement>("Credit_Scroller");
        var tutorialPopup = root.Q<VisualElement>("Tutorial_Popup");

        _tutorialManager = new TutorialUIManager(tutorialPopup);
        
        root.Q<Button>("Button_Credit").clicked += () => SetHiddenCredit(false);
        root.Q<Button>("Button_Soldier").clicked += LoadSoldeirScene;
        root.Q<Button>("Button_Commander").clicked += LoadCommanderScene;
        root.Q<Button>("Button_Tutorial").clicked += _tutorialManager.OpenTutorial;
        root.Q<Button>("Button_CloseCredit").clicked += () => SetHiddenCredit(true);
    }
    
    private void LoadSoldeirScene()
    {
        SceneManager.LoadScene("Soldier");
    }
    
    private void LoadCommanderScene()
    {
        SceneManager.LoadScene("Commander");
    }


    private void SetHiddenCredit(bool isHidden = true)
    {
        if (isHidden)
        {
            _creditScreen.AddToClassList("hidden");
        }
        else
        {
            _creditScreen.RemoveFromClassList("hidden");
        }
    }
}

public partial class TutorialUIManager
{
    private readonly VisualElement _root;
    private readonly TutorialElement[] _tutorialElements;
    private int tutorialIndex = 0;
    public TutorialUIManager(VisualElement root)
    {
        _root = root;

        _root.Q<Button>("closeButton").clicked += CloseTutorial;
        _root.Q<Button>("leftButton").clicked += PreviousTutorial;
        _root.Q<Button>("rightButton").clicked += NextTutorial;

        _tutorialElements = new[]
        {
            new TutorialElement(
                root.Q<VisualElement>("firstSelect"),
                root.Q<VisualElement>("firstTutorial")
            ),
            new TutorialElement(
                root.Q<VisualElement>("secondSelect"),
                root.Q<VisualElement>("secondTutorial")
            ),
            new TutorialElement(
                root.Q<VisualElement>("thirdSelect"),
                root.Q<VisualElement>("thirdTutorial")
            )
        };
        
        UpdateTutorialDisplay();
    }
    
    public void OpenTutorial()
    {
        // クラス指定がベストだが今更冗長かなで直接スタイル変更
        _root.style.display = DisplayStyle.Flex;
    }
    private void CloseTutorial()
    {
        _root.style.display = DisplayStyle.None;
    }
    private void NextTutorial()
    {
        tutorialIndex++;
        if (tutorialIndex >= _tutorialElements.Length)
        {
            tutorialIndex = 0;
        }
        UpdateTutorialDisplay();
    }
    private void PreviousTutorial()
    {
        tutorialIndex--;
        if (tutorialIndex < 0)
        {
            tutorialIndex = _tutorialElements.Length - 1;
        }

        UpdateTutorialDisplay();
    }
    private void UpdateTutorialDisplay()
    {
        foreach (var element in _tutorialElements)
        {
            element.Hide();
        }
        var tutorialElement = _tutorialElements[tutorialIndex];
        tutorialElement.Show();
    }
        
    private class TutorialElement
    {
        private readonly VisualElement numbSelector;
        private readonly VisualElement tutorialContainer;
        private readonly Color showSelectorColor = Color.green;
        private readonly Color hideSelectorColor = new (108, 108, 108, 100);

        public TutorialElement(VisualElement numbSelector, VisualElement tutorialContainer)
        {
            this.numbSelector = numbSelector;
            this.tutorialContainer = tutorialContainer;
        }

        public void Hide()
        {
            numbSelector.style.backgroundColor = hideSelectorColor;
            tutorialContainer.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            numbSelector.style.backgroundColor = showSelectorColor;
            tutorialContainer.style.display = DisplayStyle.Flex;
        }
    }
}
