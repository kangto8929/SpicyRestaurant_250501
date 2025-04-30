using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;


public class UI_Shop : MonoBehaviour
{
    public DialogWindowSetting DialogWindow;//추가

    public static UI_Shop Instance;


    [SerializeField]
    private RectTransform KitchenPanel;
    [SerializeField]
    private RectTransform FrontDeskpanel;

    //[SerializeField]
    //private Image OptionPanel;//0414

    //0414
    /*[SerializeField]
    private GameObject OptionPanel;
    [SerializeField]
    private Transform pauseUIBG;
    */


    [SerializeField]
    private Image _bowl;

    // [SerializeField]
    //private Button _askButton;
    public Button AskButton;

    /*[SerializeField]
    private TextMeshProUGUI DayText;
    [SerializeField]
    private TextMeshProUGUI BudestText;
    [SerializeField]
    private TextMeshProUGUI SatisfactionText;
    */

    public TextMeshProUGUI DayText;
    public TextMeshProUGUI BudestText;
    public TextMeshProUGUI SatisfactionText;

    public bool IsKitchenOpened = false;
    public float KitchenMoveAmount = 10f;

    public int AskPenalty;

    public Action OnKitchenToggled;

    private float _frontDeskPanelYPosition;


    // 애니메이션 실행 여부
    public bool AllowAnimation = false;

    private Sequence _askSequence; // 다시 물어볼 때 제출했을 경우때문에 제작
    private Coroutine _askCoroutine; // 다시 물어볼 때 제출했을 경우때문에 제작
    private Tween _typingTween;

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
        AllowAnimation = false;

        //RefreshDay();

        KitchenPanel.anchoredPosition = new Vector2(0, -KitchenPanel.rect.height);
        _frontDeskPanelYPosition = FrontDeskpanel.transform.position.y;

        //로드 //0420
        //  SaveManager.Instance.LoadCurrentGame();
        //Debug.Log("재료 정보 불러오기");





        CurrencyManager.Instance.LoadCurrencyData();//재화 및 만족도 불러오기
        CurrencyManager.Instance.LoadCurrencyInOutData();//재화 및 만족도 업앤다운 불러오기

        // IngredientManager.Instance.SaveAllIngredientCounts();//재료 불러오기
        // IngredientManager.Instance.LoadAllIngredientCounts();//재료 불러오기

        // SaveManager.LoadGame();
        CustomerManager.Instance.LoadCustomerIndex();//불러오기


        //GameManager.Instance.LoadLevelData();//이거 이상한 것 같음


        // 현재 씬 이름 확인 (디버그용)
        //  string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        // Debug.Log("현재 씬 유지하며 데이터만 불러옴: " + currentScene);

        CurrencyManager.Instance.LoadCurrencyData();
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleKitchen();
        }


    }

    public void ToggleKitchen()
    {



        if (IsKitchenOpened)
        {
            KitchenPanel.DOMoveY(KitchenPanel.position.y - KitchenMoveAmount, 1f);
            FrontDeskpanel.DOMoveY(_frontDeskPanelYPosition, 1f);

            OnKitchenToggled();

            IsKitchenOpened = false;
        }
        else
        {
            KitchenPanel.DOMoveY(KitchenPanel.position.y + KitchenMoveAmount, 1f);
            FrontDeskpanel.DOMoveY(_frontDeskPanelYPosition + KitchenMoveAmount, 1f);

            OnKitchenToggled();

            IsKitchenOpened = true;
        }
    }

    /* public void RefreshDay()
     {
         //DayText.text = $"Day {GameManager.Instance.CurrentLevel + 1}";
         DayText.text = $"Day {GameManager.Instance.CurrentLevel + 1}";
         Debug.Log("날짜 출력합니다" + (GameManager.Instance.CurrentLevel + 1) + "일 째");
     }*/

    public void RefreshCurrency(CurrencyType currencyType, int amount)
    {
        // Apply animations for subsequent updates
        if (currencyType == CurrencyType.Money)
        {
            int currentAmount = int.Parse(BudestText.text);
            int changeAmount = amount - currentAmount;
            BudestText.text = $"{amount}";

            // Display change value
            /*ShowChangeValue(BudestText, changeAmount);

            // Use helper method for animations
            AnimateTextChange(BudestText, changeAmount);
            */
            if (AllowAnimation)
            {
                ShowChangeValue(BudestText, changeAmount);
                AnimateTextChange(BudestText, changeAmount);
            }
            return;
        }

        if (currencyType == CurrencyType.Satisfaction)
        {
            int currentSatisfaction = int.Parse(SatisfactionText.text.Replace("%", ""));
            int changeAmount = amount - currentSatisfaction;
            SatisfactionText.text = $"{amount}%";

            // Display change value
            //ShowChangeValue(SatisfactionText, changeAmount);

            // Use helper method for animations
            //AnimateTextChange(SatisfactionText, changeAmount);

            if (AllowAnimation)
            {
                ShowChangeValue(SatisfactionText, changeAmount);
                AnimateTextChange(SatisfactionText, changeAmount);
            }
            return;
        }
        //  SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
        // Debug.Log("재료 저장하기");


    }

    private void AnimateTextChange(TextMeshProUGUI targetText, int changeAmount)
    {
        Color targetColor = new Color(119f / 255f, 87f / 255f, 82f / 255f);//0414
        Color RedColor = new Color(203f / 255f, 8f / 255f, 8f / 255f); // 빨강
        Color GreenColor = new Color(63f / 255f, 200f / 255f, 45f / 255f); // 초록



        if (changeAmount > 0)
        {
            // Animation for increasing value
            targetText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
                targetText.transform.DOScale(1f, 0.2f));
            targetText.DOColor(GreenColor, 0.2f).OnComplete(() =>
                //targetText.DOColor(Color.black, 0.2f));
                targetText.DOColor(targetColor, 0.2f));

        }
        else if (changeAmount < 0)
        {
            // Animation for decreasing value
            targetText.transform.DOScale(0.8f, 0.2f).OnComplete(() =>
                targetText.transform.DOScale(1f, 0.2f));
            targetText.DOColor(RedColor, 0.2f).OnComplete(() =>
                //targetText.DOColor(Color.black, 0.2f));
                targetText.DOColor(targetColor, 0.2f));
        }

        Debug.Log("바뀌는 거 호출됨" + "바뀐 값" + changeAmount);
        //SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
        // SaveManager.Instance.SaveCurrentGame();
        // Debug.Log("재료 저장하기");
        //AllowAnimation = false;
    }


    private void ShowChangeValue(TextMeshProUGUI targetText, int changeAmount)
    {

        Color RedColor = new Color(203f / 255f, 8f / 255f, 8f / 255f); // 빨강
        Color GreenColor = new Color(63f / 255f, 200f / 255f, 45f / 255f); // 초록

        // Create a temporary text object
        GameObject changeTextObj = new GameObject("ChangeText");
        changeTextObj.transform.SetParent(targetText.transform.parent);
        TextMeshProUGUI changeText = changeTextObj.AddComponent<TextMeshProUGUI>();

        // Set text properties
        changeText.text = changeAmount > 0 ? $"+{changeAmount}" : $"{changeAmount}";
        changeText.fontSize = targetText.fontSize * 1.2f; // 더 크게
        changeText.color = changeAmount > 0 ? GreenColor : RedColor;
        changeText.alignment = TextAlignmentOptions.Center;

        // **Bold 적용**
        changeText.fontStyle = FontStyles.Bold;

        // 선택적으로 원래 텍스트의 폰트/머티리얼도 복사
        changeText.font = targetText.font;
        changeText.fontSharedMaterial = targetText.fontSharedMaterial;

        // Position the text above the target text
        RectTransform rectTransform = changeText.GetComponent<RectTransform>();
        rectTransform.anchorMin = targetText.rectTransform.anchorMin;
        rectTransform.anchorMax = targetText.rectTransform.anchorMax;
        rectTransform.pivot = targetText.rectTransform.pivot;
        rectTransform.anchoredPosition = targetText.rectTransform.anchoredPosition + new Vector2(0, 30);
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = targetText.rectTransform.sizeDelta;

        // Animate the text (fade out and move upward)
        changeText.DOFade(0, 1f).SetEase(Ease.OutQuad);
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 50, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Destroy(changeTextObj);
        });

        Debug.Log("바뀌는 거 호출됨2" + "바뀐 값" + changeAmount);

        //AllowAnimation = false;
        // SaveManager.Instance.SaveCurrentGame();
        //Debug.Log("재료 저장하기");
    }


    public void OnOptionClick()
    {
        // OptionPanel.transform.parent.gameObject.SetActive(true);//0414

        //옵션 버튼 눌렀을 때

        // Play pop sound
        //0414~
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        UI_Option.Instance.BlackBackground.localScale = Vector3.one;//
        UI_Option.Instance.BackgroundBlackImage.enabled = true;
        UI_Option.Instance.BackgroundCanvasGroup.localScale = Vector3.zero; // Start at zero scale
        UI_Option.Instance.BackgroundCanvasGroup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack); // Scale to full size with an easing effect
        //pauseUIBG.transform.localScale = Vector3.zero; // Start at zero scale
        //OptionPanel.SetActive(true); // Activate the UI
        //pauseUIBG.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack); // Scale to full size with an easing effect
    }
    public void OnAskButtonClick()
    {


        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        // 기존에 실행 중인 시퀀스와 코루틴 정리
        _askSequence?.Kill();
        _askSequence = null;

        if (_askCoroutine != null)
        {
            StopCoroutine(_askCoroutine);
            _askCoroutine = null;
        }

        AskButton.interactable = false;
        FrontDeskpanel.gameObject.SetActive(true);
        FrontDeskpanel.transform.GetChild(1).gameObject.SetActive(false);

        UI_CustomerText.Instance._currentIndex = 0;
        UI_CustomerText.Instance._isTextEnd = false;
        UI_CustomerText.Instance.CustomerTextUI.text = "";

        _bowl.raycastTarget = true;
        AllowAnimation = true;

        _askSequence = DOTween.Sequence();

        _askSequence.Append(FrontDeskpanel.DOMoveY(_frontDeskPanelYPosition, 1f))
                   .AppendCallback(() =>
                   {
                       CurrencyManager.Instance.RemoveCurrency(CurrencyType.Satisfaction, AskPenalty);

                       if (CustomerManager.Instance.InstancedCustomer != null)
                       {
                           SpriteRenderer spriteRenderer = CustomerManager.Instance.InstancedCustomer.GetComponentInChildren<SpriteRenderer>();

                           if (spriteRenderer != null && spriteRenderer.sprite != null)
                           {
                               string spriteName = spriteRenderer.sprite.name;
                               Debug.Log("현재 손님 스프라이트 이름: " + spriteName);

                               if (new List<string> {
                               "customer_2", "customer_4", "customer_6", "customer_8",
                               "customer_11", "customer_12", "customer_13", "customer_15",
                               "customer_16", "customer_23", "customer_17"
                               }.Contains(spriteName))
                               {
                                   Debug.Log("여자 손님 한숨");
                                   AudioClip clip = SoundManager.Instance.customerNegativeSounds[0];
                                   SoundManager.Instance.PlaySoundEffect(clip);
                               }
                               else if (new List<string> {
                               "customer_1", "customer_3", "customer_5", "customer_9",
                               "customer_10", "customer_14", "customer_24", "customer_7",
                               "customer_18", "customer_19", "customer_20", "customer_21", "customer_22"
                               }.Contains(spriteName))
                               {
                                   Debug.Log("남자 손님 한숨");
                                   AudioClip clip = SoundManager.Instance.customerNegativeSounds[2];
                                   SoundManager.Instance.PlaySoundEffect(clip);
                               }
                           }
                       }
                       else
                       {
                           Debug.Log("손님 없음");
                       }

                       // 첫 번째 대사 출력 + 콜백
                       PrintText(() =>
                       {
                           Debug.Log("0번째 대사 끝나고 ProceedToNextText 진입");
                           ProceedToNextText();
                       });
                   });
    }

    private void ProceedToNextText()
    {
        DialogWindow.Asking = false;
        Debug.Log("진입했다");
        UI_CustomerText.Instance._currentIndex++;

        if (!UI_CustomerText.Instance._isTextEnd)
        {
            Debug.Log("아직 안 끝났어");
            //CurrencyManager.Instance.RemoveCurrency(CurrencyType.Satisfaction, AskPenalty);

            _askCoroutine = StartCoroutine(WaitThenPrintText());
        }
        else
        {
            Debug.Log("끝났어");
            DialogWindow.Asking = false;
            _askCoroutine = StartCoroutine(FinalWaits());
        }
    }

    private IEnumerator WaitThenPrintText()
    {
        yield return new WaitForSeconds(1.0f);
        PrintText(() =>
        {
            Debug.Log("2진입");
            ProceedToNextText();
        });
    }

    private IEnumerator FinalWaits()
    {
        yield return new WaitForSeconds(1.0f);

        _askSequence?.Append(FrontDeskpanel.DOMoveY(_frontDeskPanelYPosition + KitchenMoveAmount, 1f))
                    .OnKill(() =>
                    {
                        Debug.Log("애니메이션이 종료되고 OnKill 콜백 실행됨");
                    });

        yield return new WaitForSeconds(1.0f);

        FrontDeskpanel.gameObject.SetActive(false);
        FrontDeskpanel.transform.GetChild(1).gameObject.SetActive(true);
        AskButton.interactable = true;
        _bowl.raycastTarget = true;

        Debug.Log("다시 터치하게 해줘");
    }


    public void PrintText(Action onComplete = null)
    {
        DialogWindow.Asking = true;

        AskButton.interactable = false;
        UI_CustomerText.Instance.CustomerTextUI.DOKill();

        var customerText = UI_CustomerText.Instance._customerTextStruct.OrderText[UI_CustomerText.Instance._currentIndex];

        UI_CustomerText.Instance.CustomerTextUI.text = customerText;
        UI_CustomerText.Instance.CustomerTextUI.maxVisibleCharacters = 0;

        float duration = customerText.Length * UI_CustomerText.Instance.textSpeed;

        int previousCharCount = 0;

        _typingTween = DOTween.To(() => 0, x =>
        {
            int charCount = Mathf.FloorToInt(x);
            UI_CustomerText.Instance.CustomerTextUI.maxVisibleCharacters = charCount;

            if (charCount > previousCharCount)
            {
                previousCharCount = charCount;
            }
        },
        customerText.Length,
        duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            UI_CustomerText.Instance._isTextEnd = (UI_CustomerText.Instance._currentIndex >= UI_CustomerText.Instance._customerTextStruct.OrderText.Count - 1);
            _typingTween = null;
            onComplete?.Invoke();
        });
    }



    public void StopCurrentProcess()
    {
        // 모든 코루틴 중단
        DialogWindow.Asking = false;
        StopAllCoroutines(); // 코루틴은 이대로 두되, 아래에서 필요한 것만 따로 관리할 수도 있음

        // DOTween 애니메이션 중지
        DOTween.Kill(_askSequence); // FrontDesk 애니메이션만 종료 (OnAskButtonClick에서 만든 seq)
        _askSequence = null;

        if (_typingTween != null && _typingTween.IsActive())
        {
            _typingTween.Kill();
            _typingTween = null;
        }

        // 대사 텍스트 초기화
        UI_CustomerText.Instance.CustomerTextUI.DOKill();
        UI_CustomerText.Instance.CustomerTextUI.text = "";
        UI_CustomerText.Instance._currentIndex = 0;
        UI_CustomerText.Instance._isTextEnd = false;

        // UI 위치 초기화
        FrontDeskpanel.transform.DOMoveY(_frontDeskPanelYPosition + KitchenMoveAmount, 0.0f);

        // UI 상태 초기화
        FrontDeskpanel.gameObject.SetActive(false);
        FrontDeskpanel.transform.GetChild(1).gameObject.SetActive(true);
        AskButton.interactable = true;
        _bowl.raycastTarget = true;

        Debug.Log("모든 진행 중인 작업이 취소되었습니다.");
    }

}
