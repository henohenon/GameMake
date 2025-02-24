using System;
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
    private VisualElement _titleWindows;
    private TutorialUIManager _tutorialManager;
    
    private Button _soldierButton;
    private Button _commanderButton;
    private Button _creditToTitleButton;
    private Button _titleToCreditsButton;
    private Button _tutorialToTitleButton;
    private Button _titleToTutorialsButton;
    private Slider _volumeSlider;
    
    [SerializeField]
    private AudioSource clickSound;
    [SerializeField]
    private AudioSource sliderSound;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _titleWindows = root.Q<VisualElement>("titleWindows");
        var tutorialPopup = root.Q<VisualElement>("Tutorial_Popup");

        _tutorialManager = new TutorialUIManager(tutorialPopup);

        _soldierButton = root.Q<Button>("Button_Soldier");
        _soldierButton.clicked += LoadSoldeirScene;
        _commanderButton = root.Q<Button>("Button_Commander");
        _commanderButton.clicked += LoadCommanderScene;
        _creditToTitleButton = root.Q<Button>("Button_CreditToTitle");
        _creditToTitleButton.clicked += MoveToTitle;
        _titleToCreditsButton = root.Q<Button>("Button_TitleToCredit");
        _titleToCreditsButton.clicked += MoveToCredit;
        _tutorialToTitleButton = root.Q<Button>("Button_TutorialToTitle");
        _tutorialToTitleButton.clicked += MoveToTitle;
        _titleToTutorialsButton = root.Q<Button>("Button_TitleToTutorial");
        _titleToTutorialsButton.clicked += MoveToTutorial;
        _volumeSlider = root.Q<Slider>("volumeSlider");
        _volumeSlider.RegisterValueChangedCallback(VolumeSliderChanged);
        _volumeSlider.SetValueWithoutNotify(AudioListener.volume);
    }

    private void VolumeSliderChanged(ChangeEvent<float> e)
    {
        AudioListener.volume = e.newValue;
        sliderSound.PlayDelayed(0.3f);
    }
    
    private void LoadSoldeirScene()
    {
        SceneManager.LoadScene("Soldier");
        clickSound.Play();
    }
    
    private void LoadCommanderScene()
    {
        SceneManager.LoadScene("Commander");
        clickSound.Play();
    }

    private void MoveToTitle()
    {
        _titleWindows.ClearClassList();
        _titleWindows.AddToClassList("middle");
        clickSound.Play();
    }
    private void MoveToTutorial()
    {
        _titleWindows.ClearClassList();
        _titleWindows.AddToClassList("right");
        clickSound.Play();
    }
    private void MoveToCredit()
    {
        _titleWindows.ClearClassList();
        _titleWindows.AddToClassList("left");
        clickSound.Play();
    }

    private void OnDestroy()
    {
        _soldierButton.clicked -= LoadSoldeirScene;
        _commanderButton.clicked -= LoadCommanderScene;
        _creditToTitleButton.clicked -= MoveToTitle;
        _titleToCreditsButton.clicked -= MoveToCredit;
        _tutorialToTitleButton.clicked -= MoveToTitle;
        _titleToTutorialsButton.clicked -= MoveToTutorial;
        _volumeSlider.UnregisterValueChangedCallback(VolumeSliderChanged);
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
