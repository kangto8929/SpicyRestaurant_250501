using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public struct CustomerTextStruct
{
    public CustomerTypes CustomerType;
    public List<string> OrderText;
    public string PositiveText;
    public string NegativeText;
}

public class UI_CustomerText : MonoBehaviour
{
    public static UI_CustomerText Instance;


    public TextMeshProUGUI CustomerTextUI;
    public Button YesButton;
    public Button GetOutButton;

    public Action OnNextCustomer;

    public CustomerTextStruct _customerTextStruct;//0416
    public int _currentIndex;
    public bool _isTextEnd = false;

    public float _duration;

    //[SerializeField]
    public float textSpeed;

    public UI_IngredientsHolder IngredientsHolder;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCustomerText(CustomerTextStruct inputTextStruct)
    {
        _customerTextStruct = inputTextStruct;
    }

    public void SetCustomerText()
    {
        gameObject.SetActive(true);
        PrintText();
    }

    public void PrintText()
    {


        //아래에는 원래 있었떤 거
        CustomerTextUI.DOKill();

        CustomerTextUI.text = _customerTextStruct.OrderText[_currentIndex];



        CustomerTextUI.maxVisibleCharacters = 0;

        _duration = _customerTextStruct.OrderText[_currentIndex].Length * textSpeed;

        int previousCharCount = 0;

        DOTween.To(x =>
        {
            int currentCharCount = (int)x;
            CustomerTextUI.maxVisibleCharacters = currentCharCount;

            if (currentCharCount > previousCharCount)
            {
                SoundManager.Instance.PlayRandomSoundInList(SoundManager.Instance.typeSounds);
                previousCharCount = currentCharCount;
            }
        }
        , 0f, CustomerTextUI.text.Length, _duration).SetEase(Ease.Linear).SetId("TextTween");

        // Mark as text end if this is the last text in the list
        if (_currentIndex == _customerTextStruct.OrderText.Count - 1)
        {
            _isTextEnd = true;
        }

        // Hide the YesButton if the customer type is OnlyText and the text has ended
        if (_isTextEnd && _customerTextStruct.CustomerType == CustomerTypes.OnlyText)
        {
            YesButton.gameObject.SetActive(false);
        }
    }

    public void PrintResutlText(bool result)
    {
        gameObject.SetActive(true);
        YesButton.gameObject.SetActive(false);

        _duration = _customerTextStruct.PositiveText.Length * textSpeed;

        //UI_Shop.Instance.AllowAnimation = true;

        if (result)
        {
            CustomerTextUI.text = _customerTextStruct.PositiveText;
            SoundManager.Instance.PlayRandomSoundInList(SoundManager.Instance.customerPositiveSounds);
        }
        else
        {
            CustomerTextUI.text = _customerTextStruct.NegativeText;
            //SoundManager.Instance.PlayRandomSoundInList(SoundManager.Instance.customerNegativeSounds);
            if (CustomerManager.Instance.InstancedCustomer != null)
            {
                // 자식 포함해서 SpriteRenderer 찾기
                SpriteRenderer spriteRenderer = CustomerManager.Instance.InstancedCustomer.GetComponentInChildren<SpriteRenderer>();

                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    string spriteName = spriteRenderer.sprite.name;
                    Debug.Log("현재 손님 스프라이트 이름: " + spriteName);

                    // 여자 손님
                    if (new List<string> { "customer_2", "customer_4", "customer_6"
    , "customer_8", "customer_11", "customer_12", "customer_13", "customer_15"
    , "customer_16", "customer_23", "customer_17"}.Contains(spriteName))
                    {
                        Debug.Log("주문 실패 여자 손님 한숨");

                        // int randomIndex = UnityEngine.Random.Range(0, 2); // 0 또는 1
                        AudioClip clip = SoundManager.Instance.customerNegativeSounds[0];
                        SoundManager.Instance.PlaySoundEffect(clip);

                    }
                    // 남자 손님
                    else if (new List<string> { "customer_1", "customer_3", "customer_5"
    , "customer_9", "customer_10", "customer_14", "customer_24"
    , "customer_7", "customer_18", "customer_19", "customer_20"
    , "customer_21", "customer_22"}.Contains(spriteName))
                    {
                        Debug.Log("주문 실패 남자 손님 한숨");

                        //int randomIndex = UnityEngine.Random.Range(2, 4); // 2 또는 3
                        AudioClip clip = SoundManager.Instance.customerNegativeSounds[2];
                        SoundManager.Instance.PlaySoundEffect(clip);

                    }
                }
            }

            else
            {
                Debug.Log("손님 없음");
            }
        }

        int previousCharCount = 0;
        CustomerTextUI.DOKill();
        DOTween.To(x =>
        {
            int currentCharCount = (int)x;
            CustomerTextUI.maxVisibleCharacters = currentCharCount;

            if (currentCharCount > previousCharCount)
            {
                SoundManager.Instance.PlayRandomSoundInList(SoundManager.Instance.typeSounds);
                previousCharCount = currentCharCount;
            }
        }
        , 0f, CustomerTextUI.text.Length, _duration).SetEase(Ease.Linear).SetId("TextTween");

        //UI_Shop.Instance.AllowAnimation = false;

    }

    public void OnYesButtonClick()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        if (!_isTextEnd)
        {
            _currentIndex++;
            PrintText();
            return;
        }
        DOTween.Kill("TextTween");
        UI_Shop.Instance.ToggleKitchen();
        //슬라이더 초기화
        IngredientsHolder.SetHolderPosition();

        //SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        gameObject.SetActive(false);

        //UI_Shop.Instance.AllowAnimation = true;
        //UI_Shop.Instance.AskButton.interactable = true;
    }

    public void OnGetOutButtonClick()
    {
        Debug.Log("몇 번째 손님 보냈는지?" + CustomerManager.Instance.CompletedCustomers + "번째 손님");

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        if (CustomerManager.Instance.CompletedCustomers + 1 == CustomerManager.Instance.MaxCustomers)
        {
            CustomerManager.Instance.DestroyCustomer();
        }
        else
        {
            OnNextCustomer();
        }


        _currentIndex = 0;
        _isTextEnd = false;

        DOTween.Kill("TextTween");
        // SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.matchSound);
        YesButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
