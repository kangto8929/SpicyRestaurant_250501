using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


//[ExecuteAlways]
public class DialogWindowSetting : MonoBehaviour
{
    [Header("DialogWindowParts")]
    public RectTransform MiddlePart;
    public TextMeshProUGUI DialogText;

    [Header("DialogWindowSettings")]
    public float Padding = 30f;
    public float AnimationDuration = 0.3f;
    public bool AnimateInPlayMode = true;

    private LayoutElement _middleLayout;
    private string _lastText = "";

    public bool Asking = false;

    void Awake()
    {
        _middleLayout = MiddlePart.GetComponent<LayoutElement>();
    }

    void Update()
    {
        if (DialogText == null || _middleLayout == null) return;

        if (DialogText.text != _lastText)
        {
            _lastText = DialogText.text;
            UpdateBubble();//원래 활성화 상태였음
        }

        //_lastText = DialogText.text;
        UpdateBubble();
        CheckWindowSize();

     //   Debug.Log("되어라");
    }

    void CheckWindowSize()
    {
        if (Asking == false)
        {
            DialogText.margin = new Vector4(40, 0, 40, 0); // 좌우 여백

            LayoutRebuilder.ForceRebuildLayoutImmediate(DialogText.rectTransform);

            float targetHeight = DialogText.preferredHeight + Padding;

            _middleLayout.preferredHeight = targetHeight;
        }

        else
        {
            return;
        }
            
    }

     public void UpdateBubble()
     {
        if(Asking == true)
        {
            DialogText.margin = new Vector4(40, 0, 40, 0); // 좌우 여백

            LayoutRebuilder.ForceRebuildLayoutImmediate(DialogText.rectTransform);

            float targetHeight = DialogText.preferredHeight + Padding;



            if (Application.isPlaying && AnimateInPlayMode)
            {
                DOTween.Kill(_middleLayout);
                DOTween.To(
                    () => _middleLayout.preferredHeight,
                    x => _middleLayout.preferredHeight = x,
                    targetHeight,
                    AnimationDuration
                ).SetEase(Ease.OutCubic).SetId(_middleLayout);
            }
            else
            {
                _middleLayout.preferredHeight = targetHeight;
            }
        }

        else
        {
            return;
        }
        
    }


}
