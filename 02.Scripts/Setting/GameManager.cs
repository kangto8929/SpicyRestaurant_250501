using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using static ScriptManager;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Match3에서 받은 데이터를 저장하는 공간
    public Dictionary<IngredientType, uint> IngredientDataDic = new Dictionary<IngredientType, uint>();


    // 손님 스크립트, 이미지 데이터
    public List<CustomerNormalTextData> CustomerNormalData = new List<CustomerNormalTextData>();
    public List<CustomerPositiveTextData> CustomerPositiveData = new List<CustomerPositiveTextData>();
    public List<CustomerNegativeTextData> CustomerNegativeData = new List<CustomerNegativeTextData>();
    public List<CustomerOnlyTextData> CustomerOnlyTextData = new List<CustomerOnlyTextData>();
    public List<CustomerSpecialTextData> CustomerSpecialData = new List<CustomerSpecialTextData>();
    public List<Sprite> CustomerNormalImages = new List<Sprite>();
    public Dictionary<string, CustomerSpecialTextData> CustomerSpecialDataByName = new Dictionary<string, CustomerSpecialTextData>();
    public List<LoadingTextData> LoadingTextData = new List<LoadingTextData>();

    // 손님 랜덤으로 관리하기 위한 큐
    public Queue<int> CustomerNormalIndexQueue;
    public Queue<int> CustomerPositiveIndexQueue;
    public Queue<int> CustomerNegativeIndexQueue;
    public Queue<int> CustomerOnlyTextIndexQueue;
    public Queue<int> CustomerNormalImageQueue;
    public Queue<int> LoadingIndexQueue;

    // 스페셜 손님리스트
    // 1스테이지에 4개, 2스테이지에 2개, 3스테이지에 1개 추가 된다.
    public List<GameObject> RandomSpeicalPrefabsPerStage;

    [SerializeField]
    private List<LevelDataSO> _levelDataSOs;
    [SerializeField]
    //public int _currentLevel = 0;
    private int _currentLevel = 0;
    
    private int _maxLevel;
    //허정범이 추가 - 문제시 삭제부탁드립니다.
    public int MaxLevel => _maxLevel;
    public List<IngredientType> EnabledIngredients;

    //0421
    private Dictionary<CurrencyType, int> _playerCurrencyData = new();
    private CurrencyInOut _todayCurrencyInoutData;


    public Dictionary<CurrencyType, int> PlayerCurrencyData => _playerCurrencyData;
    public CurrencyInOut TodayCurrencyInoutData => _todayCurrencyInoutData;

    private bool _isGoodEnding;
    public bool IsGoodEnding => _isGoodEnding;

    // 문제시 삭제 - 허정범이 만듦
    public int CurrentLevel => _currentLevel;

    public bool IsReadCSV = false;


    


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        //불러오기 // 재료 데이터
        

        _maxLevel = _levelDataSOs.Count;
        //SaveManager.LoadGame();//0424

        

        LoadLevelData();//불러와
    }


   




    //---------허정범 테스트용 Update 삭제 예정---
    private void Update()
    {
       // if (Input.GetKeyDown(KeyCode.D))
         //   SaveManager.DeleteSave();
    }
    //-------------------

    /// <summary>
    /// 레벨이 끝났을 때 Match3에서 얻은 정보를 IngredientManager에 넘겨준다.
    /// </summary>
    /// <param name="destroyedBlocksCount">string, int type dictionary</param>


    //0420
    public void SetIngredientManage(Dictionary<string, int> destroyedBlocksCount)
    {
        IngredientDataDic.Clear();//기존에 있떤 거  리셋


        Dictionary<IngredientType, uint> ingredientsDict = new Dictionary<IngredientType, uint>();

        foreach (var item in destroyedBlocksCount)
        {
            if (Enum.TryParse(item.Key, out IngredientType ingredientType))
            {
                ingredientsDict[ingredientType] = (uint)item.Value;
            }
            else
            {
                Debug.LogWarning($"Invalid ingredient type: {item.Key}");
            }
        }
        IngredientDataDic = ingredientsDict;
    }

    //IngredientDataDic 정보 가져오기
    public Dictionary<IngredientType, uint> GetIngredientData()
    {
        return IngredientDataDic;
    }


    public List<IngredientType> GetEnableIngredients()
    {
        if(EnabledIngredients == null )
        {
            LoadLevelData();
        }

        return EnabledIngredients;
      
    }


    //0421
    // 유저 재화 정보 저장
    public void SetPlayerCurrecyData(Dictionary<CurrencyType, int> playerCurrency)
    {
        if (playerCurrency == null || playerCurrency.Count == 0)
        {
            Debug.LogWarning($"{this.name}: 유효하지 않는 재화 데이터 입력");
            return;
        }
        _playerCurrencyData = playerCurrency;
        Debug.Log($"{this.name}: 유저 재화 정보 저장");
    }

    // 유저 재화 정보 불러오기
    public Dictionary<CurrencyType, int> GetPlayerCurrencyData()
    {
        if (_playerCurrencyData == null)
        {
            Debug.LogWarning($"{this.name}: 유저 재화 정보가 비어있음");
            _playerCurrencyData = new Dictionary<CurrencyType, int>();
            
        }
        return _playerCurrencyData;
    }

    // 매출 정보 저장
    public void SetCurrencyInOut(CurrencyInOut currencyInOut)
    {
        _todayCurrencyInoutData = currencyInOut;
        Debug.Log($"{this.name}: 매출 정보 저장");
        SaveCurrencyInOut();
    }

    // 매출 정보 저장 (PlayerPrefs)
    public void SaveCurrencyInOut()
    {
        string json = JsonUtility.ToJson(_todayCurrencyInoutData);
        PlayerPrefs.SetString("CurrencyInOutData", json);
        PlayerPrefs.Save();
    }

    // 매출 정보 불러오기 (PlayerPrefs)
    public CurrencyInOut GetCurrencyInOut()
    {
        string json = PlayerPrefs.GetString("CurrencyInOutData", string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            _todayCurrencyInoutData = JsonUtility.FromJson<CurrencyInOut>(json);
            Debug.Log($"{this.name}: PlayerPrefs에서 매출 정보 불러오기 완료");
        }
        else
        {
            Debug.LogWarning($"{this.name}: 저장된 매출 정보가 없어 기본값 생성");
            _todayCurrencyInoutData = new CurrencyInOut();

        }

        return _todayCurrencyInoutData;
    }

  
    public void SetCurrentLevel(int amount)
    {
        Debug.Log("[SetCurrentLevel 호출됨] amount: " + amount + "\n" + Environment.StackTrace);

        // 값이 유효한지 체크
        if (amount > _levelDataSOs.Count)
        {
            Debug.Log("스테이지 레벨 초과");
            _currentLevel = _levelDataSOs.Count;
        }
        else if (amount < 0)
        {
            _currentLevel = 0;
        }
        else
        {
            _currentLevel = amount;
        }

        Debug.Log("현재 며칠 (적용 후): " + (CurrentLevel + 1));

        // PlayerPrefs에 저장
        PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
        PlayerPrefs.Save();


    }

    public int GetCurrentLevel()
    {
        // PlayerPrefs에서 CurrentLevel 불러오기 (저장된 값이 없으면 기본값 1 반환)
        _currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);  // 기본값을 1로 설정
        Debug.Log("[GetCurrentLevel 호출됨] amount: " + _currentLevel);
        return _currentLevel;
        //ㄴ
        //이 친구 매치3에도 적용될 수 있게 해야 함
    }




    public void LoadLevelData()
    {
        int level = GetCurrentLevel(); // PlayerPrefs에서 CurrentLevel 불러오기


        Debug.Log("불러온거 현재 며칠? " + (level + 1));
        EnabledIngredients = _levelDataSOs[level].EnabledIngredients;

        SetCurrentLevel(level); // 이건 동기화 용도로 남겨두어도 괜찮음
        GetIngredientData();
        

        if (UI_Shop.Instance != null && UI_Shop.Instance.DayText != null)
        {
            UI_Shop.Instance.DayText.text = $"Day {level + 1}";
            Debug.Log("이게 적용된 버전이다");
        }
    }


    //리셋
    public void ResetLevel()
    {
        // CurrentLevel을 0으로 설정하여 첫 번째 레벨로 리셋
        SetCurrentLevel(0);

        // 첫 번째 레벨의 데이터를 불러옴
        SaveManager.SaveGame(0, PlayerCurrencyData, GetIngredientData(), Scenes.Match3Game);
        //LoadLevelData();
        Debug.Log("레벨 리셋");
        Debug.Log("현재 레벨은" + _currentLevel);
    }


}
