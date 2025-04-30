using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class UI_Result : MonoBehaviour
{
    public static UI_Result Instance;



    private Dictionary<CurrencyType, int> _playerCurrencyData;
    private CurrencyInOut _todayCurrencyInOutData;

    public TextMeshProUGUI TodayDayText;

    public TextMeshProUGUI TodaySatisfactionValueText;

    public TextMeshProUGUI TodayGainValueText;
    public TextMeshProUGUI TodayLossValueText;
    public TextMeshProUGUI TodayProfitValueText;

    public TextMeshProUGUI CurrentSatisfactionValueText;
    public TextMeshProUGUI CurrentBudgetValueText;

    public GameObject ButtonPanel;
    public GameObject ResultPanel;


    //각 요소사이 출력 대기시간
    private float waitTime = 1f;

    private bool _isGoodResult;

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
       

        _playerCurrencyData = GameManager.Instance.PlayerCurrencyData;
        _todayCurrencyInOutData = GameManager.Instance.TodayCurrencyInoutData;
        RefreshDayText();
        ButtonPanel.SetActive(false);

        //손님 리셋
        CustomerManager.Instance.DeleteCustomerSave();
    }

    public void AnimateResultPanel()
    {
        ResultPanel.gameObject.SetActive(true);
        ResultPanel.transform.DOMove(Vector3.zero, 3f).OnComplete(() =>
        {
            // Create a sequence to display text animations with delays
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => AnimateTodaySatisfaction())
                    .AppendInterval(waitTime) // Delay before the next text
                    .AppendCallback(() => AnimateTodayGain())
                    .AppendInterval(waitTime)
                    .AppendCallback(() => AnimateTodayLoss())
                    .AppendInterval(waitTime)
                    .AppendCallback(() => AnimateTodayProfit())
                    .AppendInterval(waitTime)
                    .AppendCallback(() => AnimateCurrentSatifaction())
                    .AppendInterval(waitTime*2)
                    .AppendCallback(() => AnimateCurrentBudget())
                    .AppendInterval(waitTime)
                    .AppendCallback(() => AnimateButtonPanel());
        });
    }

    public void RefreshDayText()//여기가 문제인 것으로 추정
    {
        // 재료 갯수 리셋
        //IngredientManager.Instance.ClearAllPlayerPrefsData();

        // 재료 초기화
        IngredientManager.Instance.SaveAllIngredientCounts();

        //GameManager.Instance._currentLevel = GameManager.Instance.CurrentLevel + 1;

        // 게임 상태 저장
        // SaveManager.SaveGame(GameManager.Instance._currentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);

        Debug.Log("현재 레벳이 몇이길래?" + GameManager.Instance.CurrentLevel);

        //0424
        if ((GameManager.Instance.CurrentLevel) > 2)//3이었음
        {
            Debug.Log("UI_Result 진짜 엔딩으로 간다고??");
            SaveManager.SaveGame((GameManager.Instance.CurrentLevel), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.EndingScene);
           
        }

        else
        {
            SaveManager.SaveGame((GameManager.Instance.CurrentLevel), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
        }

        //SaveManager.SaveGame((GameManager.Instance._currentLevel + 1), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
       

        GameManager.Instance.EnabledIngredients = null;

        // Day 텍스트 업데이트
        TodayDayText.text = $"Day {GameManager.Instance.CurrentLevel}"; // GetCurrentLevel() 사용

        // 재화 초기화
        CurrencyManager.Instance.DayChangeCurrencyInOutData();
       // GameManager.Instance.SetCurrentLevel(GameManager.Instance.CurrentLevel + 1);
    }


    public void AnimateTodaySatisfaction()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.resultElementSound?.OrderBy(_ => Random.value).FirstOrDefault());

        int todaysatisfaction = _todayCurrencyInOutData.TodayEndSatisfaction - _todayCurrencyInOutData.TodayStartSatisfaction;
        TodaySatisfactionValueText.gameObject.SetActive(true);
        TodaySatisfactionValueText.text = $"{todaysatisfaction} %";
        if (todaysatisfaction >= 0)
        {
            ColorUtility.TryParseHtmlString("#63a140", out var positiveColor);
            TodaySatisfactionValueText.color = positiveColor;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#a72929", out var negativeColor);
            TodaySatisfactionValueText.color = negativeColor;
        }

        TodaySatisfactionValueText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
            TodaySatisfactionValueText.transform.DOScale(1f, 0.2f));
    }

    public void AnimateTodayGain()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.resultElementSound?.OrderBy(_ => Random.value).FirstOrDefault());

        TodayGainValueText.gameObject.SetActive(true);
        TodayGainValueText.text = $"  {_todayCurrencyInOutData.TodayGainBudget}";
        ColorUtility.TryParseHtmlString("#63a140", out var positiveColor);
        TodayGainValueText.color = positiveColor;

        TodayGainValueText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
            TodayGainValueText.transform.DOScale(1f, 0.2f));
    }

    public void AnimateTodayLoss()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.resultElementSound?.OrderBy(_ => Random.value).FirstOrDefault());

        TodayLossValueText.gameObject.SetActive(true);
        TodayLossValueText.text = $"-{_todayCurrencyInOutData.TodayLossBudget}";
        ColorUtility.TryParseHtmlString("#a72929", out var negativeColor);
        TodayLossValueText.color = negativeColor;

        TodayLossValueText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
            TodayLossValueText.transform.DOScale(1f, 0.2f));
    }

    public void AnimateTodayProfit()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.resultElementSound?.OrderBy(_ => Random.value).FirstOrDefault());

        TodayProfitValueText.gameObject.SetActive(true);
        int todayProfit = _todayCurrencyInOutData.TodayGainBudget - _todayCurrencyInOutData.TodayLossBudget;
        TodayProfitValueText.text = $"{todayProfit}";
        if (todayProfit > 0)
        {
            ColorUtility.TryParseHtmlString("#63a140", out var positiveColor);
            TodayProfitValueText.color = positiveColor;
            _isGoodResult = true;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#a72929", out var negativeColor);
            TodayProfitValueText.color = negativeColor;
            _isGoodResult = false;
        }

        TodayProfitValueText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
            TodayProfitValueText.transform.DOScale(1f, 0.2f));
    }

    public void AnimateCurrentSatifaction()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.resultElementSound?.OrderBy(_ => Random.value).FirstOrDefault());

        CurrentSatisfactionValueText.gameObject.SetActive(true);
        CurrentSatisfactionValueText.text = $"{_playerCurrencyData[CurrencyType.Satisfaction]}";

        CurrentSatisfactionValueText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
            CurrentSatisfactionValueText.transform.DOScale(1f, 0.2f));
    }

    public void AnimateCurrentBudget()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.resultElementSound?.OrderBy(_ => Random.value).FirstOrDefault());

        CurrentBudgetValueText.gameObject.SetActive(true);
        CurrentBudgetValueText.text = $"{_playerCurrencyData[CurrencyType.Money]}";

        CurrentBudgetValueText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
            CurrentBudgetValueText.transform.DOScale(1f, 0.2f));

        if(_isGoodResult)
        {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.ReusltPositiveBGMSound);
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.ReusltPositiveSound);
        }
        else
        {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.ReusltNegativeBGMSound);
            //SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.ReusltNegativeSound);
        }
    }

    public void AnimateButtonPanel()
    {
        ButtonPanel.SetActive(true);
    }
}
