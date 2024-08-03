using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIVisualController : MonoBehaviour
    {
        [SerializeField] private Color showColor;
        [SerializeField] private Color hideColor;
        [SerializeField] private List<TMP_Text> targetList;
    
        [SerializeField] private Color showColor2;
        [SerializeField] private Color hideColor2;
        [SerializeField] private Image bg;

        public void Show()
        {
            Debug.Log("UIVisualController Show");
            foreach (var target in targetList)
            {
                target.DOPause();
                target.DOColor(showColor, UIManager.AnimationTime);
            }

            bg.DOPause();
            bg.DOColor(showColor2, UIManager.AnimationTime);
        }

        public void Hide()
        {
            Debug.Log("UIVisualController Hide");
            foreach (var target in targetList)
            {
                target.DOPause();
                target.DOColor(hideColor, UIManager.AnimationTime);
            }
            bg.DOPause();
            bg.DOColor(hideColor2, UIManager.AnimationTime);
        }
    }
}
