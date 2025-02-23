using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TitleCameraManager : MonoBehaviour
{
    [SerializeField]
    private LitMotionAnimation[] cameraAnimations;

    [SerializeField] private UIDocument uiDocument;
    
    private int animationNumber = 0;

    private VisualElement _background;

    private void Start()
    {
        animationNumber = Random.Range(0, cameraAnimations.Length - 1);
        PlayCameraAnimation().Forget();
        _background = uiDocument.rootVisualElement.Q("background");
    }

    private async UniTaskVoid PlayCameraAnimation()
    {
        cameraAnimations[animationNumber].Play();
        await UniTask.WaitForSeconds(58);
        _background.AddToClassList("active");
        await UniTask.WaitForSeconds(2);
        animationNumber++;
        if (animationNumber >= cameraAnimations.Length)
        {
            animationNumber = 0;
        }

        PlayCameraAnimation().Forget();
        _background.RemoveFromClassList("active");
    }
}
