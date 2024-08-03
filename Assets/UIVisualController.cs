using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Util.EventSystem;

public class UIVisualController : MonoBehaviour
{
    [SerializeField] private Color showColor;
    [SerializeField] private Color hideColor;
    [SerializeField] private List<TMP_Text> targetList;
    
    [SerializeField] private Color showColor2;
    [SerializeField] private Color hideColor2;
    [SerializeField] private List<Image> targetList2;
    
    public void Show()
    {
        foreach (var target in targetList)
        {
            target.GetComponent<TMP_Text>()?.DOPause();
            target.GetComponent<TMP_Text>()?.DOColor(showColor, UIManager.AnimationTime);
        }
        foreach (var target in targetList2)
        {
            target.GetComponent<Image>()?.DOPause();
            target.GetComponent<Image>()?.DOColor(showColor2, UIManager.AnimationTime);
        }
    }

    public void Hide()
    {
        foreach (var target in targetList)
        {
            target.GetComponent<TMP_Text>()?.DOPause();
            target.GetComponent<TMP_Text>()?.DOColor(hideColor, UIManager.AnimationTime);
        }
        foreach (var target in targetList2)
        {
            target.GetComponent<Image>()?.DOPause();
            target.GetComponent<Image>()?.DOColor(hideColor2, UIManager.AnimationTime);
        }
    }
}
