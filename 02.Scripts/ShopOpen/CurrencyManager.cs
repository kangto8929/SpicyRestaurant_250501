using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CurrencyType
{
    Money,
    Satisfaction,
    Count
}

public struct CurrencyInOut
{
    public int TodayGainBudget;
    public int TodayLossBudget;
    public int TodayStartSatisfaction;
    public int TodayEndSatisfaction;
}

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int Satisfaction => _playerCurrency[CurrencyType.Satisfaction];
    public int Budget => _playerCurrency[CurrencyType.Satisfaction];

    public CurrencyInOut CurrencyInOut;
    //[SerializeField]
    //private CurrencyInOut _currencyInOut;
    private Dictionary<CurrencyType, int> _playerCurrency;


    [SerializeField]
    private const int _initialBudget = 500;
    [SerializeField]
    private const int _initialSatisfaction = 50;
    private const int _maxSatisfaction = 100;

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

        // TODO
        //_playerCurrency = GameManager.Instance.PlayerCurrencyData;

        /* if (_playerCurrency == null)
         {
             _playerCurrency = new Dictionary<CurrencyType, int>((int)CurrencyType.Count);
             _playerCurrency.Add(CurrencyType.Money, _initialBudget);
             _playerCurrency.Add(CurrencyType.Satisfaction, _initialSatisfaction);
         }*/

        if (_playerCurrency == null)
        {
            _playerCurrency = new Dictionary<CurrencyType, int>((int)CurrencyType.Count);
        }

        if (!_playerCurrency.ContainsKey(CurrencyType.Money))
        {
            _playerCurrency[CurrencyType.Money] = _initialBudget;
        }

        if (!_playerCurrency.ContainsKey(CurrencyType.Satisfaction))
        {
            _playerCurrency[CurrencyType.Satisfaction] = _initialSatisfaction;
        }

        CurrencyInOut.TodayStartSatisfaction = _playerCurrency[CurrencyType.Satisfaction];

        ///LoadCurrencyData();
    }

    //민주-씬이 바뀔 때마다 호출
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로딩이 완료되면 한 번만 실행됨
        Debug.Log($"씬 로드됨: {scene.name}");

        LoadCurrencyData();
        LoadCurrencyInOutData();

        GameManager.Instance.LoadLevelData();

    }


    private void Start()
    {
        if(UI_Shop.Instance != null)
        {
            UI_Shop.Instance.RefreshCurrency(CurrencyType.Money, _playerCurrency[CurrencyType.Money]);
            UI_Shop.Instance.RefreshCurrency(CurrencyType.Satisfaction, _playerCurrency[CurrencyType.Satisfaction]);
        }
        
    }

    public bool AddCurrency(CurrencyType currencyType, int amount)
    {
        if (amount < 0)
        {
           // Debug.LogWarning($"유효하지 않는 재화 개수: {amount}");
            return false;
        }

        _playerCurrency[currencyType] += amount;

        if (currencyType == CurrencyType.Money)
        {
            CurrencyInOut.TodayGainBudget += amount;
        }

        if (_playerCurrency[CurrencyType.Satisfaction] > _maxSatisfaction)
        {
            _playerCurrency[CurrencyType.Satisfaction] = _maxSatisfaction;
        }

        UI_Shop.Instance.RefreshCurrency(currencyType, _playerCurrency[currencyType]);

        return true;
    }


    public bool RemoveCurrency(CurrencyType currencyType, int amount)
    {
        if (amount < 0)
        {
          //  Debug.LogWarning($"유효하지 않는 재화 개수: {amount}");
            return false;
        }

        _playerCurrency[currencyType] -= amount;

        if (currencyType == CurrencyType.Money)
        {
            CurrencyInOut.TodayLossBudget += amount;
        }

        if (_playerCurrency[CurrencyType.Satisfaction] < 0)
        {
            _playerCurrency[CurrencyType.Satisfaction] = 0;
        }

        UI_Shop.Instance.RefreshCurrency(currencyType, _playerCurrency[currencyType]);

        return true;
    }

    private void OnDestroy()
    {
        CurrencyInOut.TodayEndSatisfaction = _playerCurrency[CurrencyType.Satisfaction];

        GameManager.Instance.SetPlayerCurrecyData(_playerCurrency);
        GameManager.Instance.SetCurrencyInOut(CurrencyInOut);
    }


    public void SaveCurrencyData()
    {
        CurrencyInOut.TodayEndSatisfaction = _playerCurrency[CurrencyType.Satisfaction];

        GameManager.Instance.SetPlayerCurrecyData(_playerCurrency);
        GameManager.Instance.SetCurrencyInOut(CurrencyInOut);

        Debug.Log($"[저장 완료Money = {_playerCurrency[CurrencyType.Money]}, Satisfaction = {_playerCurrency[CurrencyType.Satisfaction]}");

        GameManager.Instance.SaveCurrencyInOut();
        //UI_Shop.Instance.RefreshCurrency(CurrencyType.Money, _playerCurrency[CurrencyType.Money]);
        //UI_Shop.Instance.RefreshCurrency(CurrencyType.Satisfaction, _playerCurrency[CurrencyType.Satisfaction]);

        if (_playerCurrency[CurrencyType.Money] >= EndingStateManager.Instance.GoodEndingMoney)//엔딩 확인
        {
            //굿 엔딩으로 저장
            EndingStateManager.Instance.IsGoodEnding = true;

            EndingStateManager.Instance.SaveEnding();
            Debug.Log("8100보다 커서 굿 엔딩 가능");
        }

        else
        {
            //배드 엔딩으로 저장
            EndingStateManager.Instance.IsGoodEnding = false;
            Debug.Log("8100보다 작아서 굿 엔딩 불가");
            EndingStateManager.Instance.SaveEnding();
        }
    }

    public void LoadCurrencyData()
    {
        // GameManager에서 저장된 데이터를 불러옴
        Dictionary<CurrencyType, int> loadedCurrency = GameManager.Instance.GetPlayerCurrencyData();

        // 만약 필수 키(Money 또는 Satisfaction)가 없으면 초기화
        if (!loadedCurrency.ContainsKey(CurrencyType.Money) || !loadedCurrency.ContainsKey(CurrencyType.Satisfaction))
        {
            Debug.Log("[로드 실패] 키 누락으로 인해 ResetCurrencyData 실행");
            ResetCurrencyData();
            return;
        }

        // 딕셔너리가 정상이라면 현재 클래스의 변수에 적용
        _playerCurrency = loadedCurrency;

        Debug.Log($"[불러오기 완료] Money = {_playerCurrency[CurrencyType.Money]}, Satisfaction = {_playerCurrency[CurrencyType.Satisfaction]}");

        if(UI_Shop.Instance != null)
        {
            UI_Shop.Instance.RefreshCurrency(CurrencyType.Money, _playerCurrency[CurrencyType.Money]);
            UI_Shop.Instance.RefreshCurrency(CurrencyType.Satisfaction, _playerCurrency[CurrencyType.Satisfaction]);
        }


        // 엔딩 조건 확인
        if (_playerCurrency[CurrencyType.Money] >= EndingStateManager.Instance.GoodEndingMoney)
        {
            EndingStateManager.Instance.IsGoodEnding = true;
            Debug.Log("8100보다 커서 굿 엔딩 가능");
        }
        else if (_playerCurrency[CurrencyType.Money] < EndingStateManager.Instance.GoodEndingMoney)
        {
            EndingStateManager.Instance.IsGoodEnding = false;
            Debug.Log("8100보다 작아서 굿 엔딩 불가");
        }

        EndingStateManager.Instance.SaveEnding();
    }



    public void ResetCurrencyData()
    {
        // 초기화: 돈과 만족도 0으로 설정
        _playerCurrency[CurrencyType.Money] = 500;
        _playerCurrency[CurrencyType.Satisfaction] = 50;


        // GameManager에 반영
        GameManager.Instance.SetPlayerCurrecyData(_playerCurrency);
        GameManager.Instance.SetCurrencyInOut(CurrencyInOut);

        // UI 갱신

        if(UI_Shop.Instance != null)
        {
            UI_Shop.Instance.RefreshCurrency(CurrencyType.Money, 0);
            UI_Shop.Instance.RefreshCurrency(CurrencyType.Satisfaction, 0);
        }

        Debug.Log("[리셋 완료] 모든 재화를 초기화했습니다.");

        GameManager.Instance.SaveCurrencyInOut();
        ResetCurrencyInOutData();
        SaveCurrencyInOutData();
    }

    //하루동안 변한 값들 저장/로드/리셋
    public void SaveCurrencyInOutData()
    {
        CurrencyInOut.TodayEndSatisfaction = _playerCurrency[CurrencyType.Satisfaction];

        GameManager.Instance.SetCurrencyInOut(CurrencyInOut);

        Debug.Log($"[InOut 저장 완료] 오늘 예산 증가: {CurrencyInOut.TodayGainBudget}, 감소: {CurrencyInOut.TodayLossBudget}, 시작 만족도: {CurrencyInOut.TodayStartSatisfaction}, 종료 만족도: {CurrencyInOut.TodayEndSatisfaction}");
    }

    public void LoadCurrencyInOutData()
    {
        CurrencyInOut loadedCurrencyInOut = GameManager.Instance.GetCurrencyInOut();
        CurrencyInOut = loadedCurrencyInOut;


        Debug.Log($"[InOut 불러오기 완료] 오늘 예산 증가: {CurrencyInOut.TodayGainBudget}, 감소: {CurrencyInOut.TodayLossBudget}, 시작 만족도: {CurrencyInOut.TodayStartSatisfaction}, 종료 만족도: {CurrencyInOut.TodayEndSatisfaction}");
    }

    //완전 리셋
    public void ResetCurrencyInOutData()
    {
        CurrencyInOut = new CurrencyInOut
        {
            TodayGainBudget = 0,
            TodayLossBudget = 0,
            TodayStartSatisfaction = _initialSatisfaction,
            TodayEndSatisfaction = _initialSatisfaction
        };

        GameManager.Instance.SetCurrencyInOut(CurrencyInOut);

        Debug.Log("[InOut 리셋 완료] 오늘 재화 흐름 초기화됨.");
    }
   
    //날이 바뀔 때만
    public void DayChangeCurrencyInOutData()
    {
        LoadCurrencyInOutData();

        CurrencyInOut = new CurrencyInOut
        {
            TodayGainBudget = 0,
            TodayLossBudget = 0,
            TodayStartSatisfaction = CurrencyInOut.TodayEndSatisfaction,//CurrencyInOut.TodayStartSatisfaction,
            TodayEndSatisfaction = CurrencyInOut.TodayEndSatisfaction
        };

        GameManager.Instance.SetCurrencyInOut(CurrencyInOut);

        Debug.Log("[InOut 리셋 완료] 오늘 재화 흐름 초기화됨.");
    }


}


