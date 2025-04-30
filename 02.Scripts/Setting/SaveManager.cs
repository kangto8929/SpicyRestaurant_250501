using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum Scenes
{
    Match3Game,
    ShopOpen,
    EndingScene,

    Count,
}

public class SaveData
{

    public int CurrentStage;
    // Dictionary는 직렬화가 불가능해서 List<CurrencyData>대체 구조를 사용한다.
    public List<CurrencyData> CurrencyList;
    public List<IngredientData> IngredientsList;
    public Scenes NextScene;

    // 중복방지를 위한 큐 데이타
    // 큐는 직렬화 되지 않기 때문에 LIst로 저장한다.
    public List<int> CustomerNormalIndexList;
    public List<int> CustomerPositiveIndexList;
    public List<int> CustomerNegativeIndexList;
    public List<int> CustomerOnlyTextIndexList;
    public List<int> CustomerNormalImageList;
    public List<int> LoadingIndexList;

    public SaveData(int currentStage, Dictionary<CurrencyType, int> playerCurrency, Dictionary<IngredientType, uint> ingredient, Scenes scene,
        Queue<int> customerNormalQueue, Queue<int> customerPositiveQueue, Queue<int> customerNegativeQueue, Queue<int> customerOnlyTextQueue,
        Queue<int> customerNormalImageQueue, Queue<int> loadingIndexQueue)
    {
        CurrentStage = currentStage;
        NextScene = scene;

        // Dictionary를 List로 변환
        CurrencyList = new List<CurrencyData>();
        if (playerCurrency != null)
        {
            foreach (var pair in playerCurrency)
            {
                CurrencyList.Add(new CurrencyData { Type = pair.Key, Amount = pair.Value });
            }
        }

        IngredientsList = new List<IngredientData>();
        if (ingredient != null)
        {
            foreach (var pair in ingredient)
            {
                IngredientsList.Add(new IngredientData { Type = pair.Key, Amount = (int)pair.Value });
            }
        }

        // Queue를 List로 변환
        CustomerNormalIndexList = new List<int>(customerNormalQueue ?? new Queue<int>());
        CustomerPositiveIndexList = new List<int>(customerPositiveQueue ?? new Queue<int>());
        CustomerNegativeIndexList = new List<int>(customerNegativeQueue ?? new Queue<int>());
        CustomerOnlyTextIndexList = new List<int>(customerOnlyTextQueue ?? new Queue<int>());
        CustomerNormalImageList = new List<int>(customerNormalImageQueue ?? new Queue<int>());
        LoadingIndexList = new List<int>(loadingIndexQueue ?? new Queue<int>());

    }
    public SaveData() { }

    public Dictionary<CurrencyType, int> GetCurrencyDictionary()
    {
        Dictionary<CurrencyType, int> result = new Dictionary<CurrencyType, int>();
        if (CurrencyList != null)
        {
            foreach (var currency in CurrencyList)
            {
                result[currency.Type] = currency.Amount;
            }
        }
        return result;
    }
    public Dictionary<string, int> GetIngredientDictionary()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        if (IngredientsList != null)
        {
            foreach (var ingredient in IngredientsList)
            {
                result[ingredient.Type.ToString()] = ingredient.Amount;
            }
        }
        return result;
    }

    // 리스트로 변환한 큐에서
    // 다시 큐를 받는 메서드
    public Queue<int> GetCustomerNormalIndexQueue()
    {
        Queue<int> queue = new Queue<int>();
        if (CustomerNormalIndexList != null)
        {
            foreach (var item in CustomerNormalIndexList)
            {
                queue.Enqueue(item);
            }
        }
        return queue;
    }

    public Queue<int> GetCustomerPositiveIndexQueue()
    {
        Queue<int> queue = new Queue<int>();
        if (CustomerPositiveIndexList != null)
        {
            foreach (var item in CustomerPositiveIndexList)
            {
                queue.Enqueue(item);
            }
        }
        return queue;
    }

    public Queue<int> GetCustomerNegativeIndexQueue()
    {
        Queue<int> queue = new Queue<int>();
        if (CustomerNegativeIndexList != null)
        {
            foreach (var item in CustomerNegativeIndexList)
            {
                queue.Enqueue(item);
            }
        }
        return queue;
    }

    public Queue<int> GetCustomerOnlyTextIndexQueue()
    {
        Queue<int> queue = new Queue<int>();
        if (CustomerOnlyTextIndexList != null)
        {
            foreach (var item in CustomerOnlyTextIndexList)
            {
                queue.Enqueue(item);
            }
        }
        return queue;
    }

    public Queue<int> GetCustomerNormalImageQueue()
    {
        Queue<int> queue = new Queue<int>();
        if (CustomerNormalImageList != null)
        {
            foreach (var item in CustomerNormalImageList)
            {
                queue.Enqueue(item);
            }
        }
        return queue;
    }

    public Queue<int> GetLoadingIndexQueue()
    {
        Queue<int> queue = new Queue<int>();
        if (LoadingIndexList != null)
        {
            foreach (var item in LoadingIndexList)
            {
                queue.Enqueue(item);
            }
        }
        return queue;
    }


}

[Serializable]
public class CurrencyData
{
    public CurrencyType Type;
    public int Amount;
}
[Serializable]
public class IngredientData
{
    public IngredientType Type;
    public int Amount;
}



public class SaveManager : MonoBehaviour
{

    private const string SAVE_KEY = "SaveData";

    // 씬 이름 매핑 Dictionary 추가
    private static Dictionary<Scenes, string> sceneNameMapping = new Dictionary<Scenes, string>()
    {
        { Scenes.Match3Game, "Match3Game" },
        { Scenes.ShopOpen, "ShopOpen" },
        { Scenes.EndingScene, "EndingScene" },
        // 다른 씬들도 필요에 따라 추가
    };

    // 씬 이름을 가져오는 메서드 추가
    public static string GetSceneName(Scenes scene)
    {
        if (sceneNameMapping.TryGetValue(scene, out string sceneName))
        {
            return sceneName;
        }

        Debug.LogWarning($"씬 이름 매핑이 없습니다: {scene}. 기본 이름을 사용합니다.");
        return scene.ToString();
    }

    public static void SaveGame(int currentStage, Dictionary<CurrencyType, int> playerCurrency, Dictionary<IngredientType, uint> ingredient, Scenes scene)
    {
        // 디버그 출력: 재료 정보 확인
        if (ingredient != null && ingredient.Count > 0)
        {
            Debug.Log("저장될 재료 정보:");
            foreach (var pair in ingredient)
            {
                Debug.Log($"IngredientType: {pair.Key}, Amount: {pair.Value}");
            }
        }
        else
        {
            Debug.Log("저장될 재료 정보가 없습니다.");
        }

        SaveData data = new SaveData(currentStage, playerCurrency, ingredient, scene,
            GameManager.Instance.CustomerNormalIndexQueue,
            GameManager.Instance.CustomerPositiveIndexQueue,
            GameManager.Instance.CustomerNegativeIndexQueue,
            GameManager.Instance.CustomerOnlyTextIndexQueue,
            GameManager.Instance.CustomerNormalImageQueue,
            GameManager.Instance.LoadingIndexQueue);

        // 씬 이름 디버그 출력
        Debug.Log("저장될 씬 이름: " + scene.ToString());

        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, jsonData);
        PlayerPrefs.Save();

        Debug.Log("Game saved: " + jsonData);
        Scenes savedScene = LoadGame();
    }
    public static Scenes LoadGame()
    {
        Debug.Log("로드 게임 함수 실행");

        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string jsonData = PlayerPrefs.GetString(SAVE_KEY);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
            Debug.Log("Game loaded: " + jsonData);

            // 레벨, 재화, 재료 정보 적용
            GameManager.Instance.SetCurrentLevel(data.CurrentStage);
            Debug.Log("SaveManager의 LoadGame에서 불러온 현재 스테이지 레벨은?" + data.CurrentStage);

            GameManager.Instance.SetPlayerCurrecyData(data.GetCurrencyDictionary());

            var ingredientDict = data.GetIngredientDictionary();
            if (ingredientDict != null && ingredientDict.Count > 0)
            {
                Debug.Log("로드된 재료 정보:");
                foreach (var pair in ingredientDict)
                {
                    Debug.Log($"IngredientType: {pair.Key}, Amount: {pair.Value}");
                }
            }
            else
            {
                Debug.Log("로드된 재료 정보가 없습니다.");
            }

            GameManager.Instance.SetIngredientManage(ingredientDict);

            // 큐 데이터
            GameManager.Instance.CustomerNormalIndexQueue = data.GetCustomerNormalIndexQueue();
            GameManager.Instance.CustomerPositiveIndexQueue = data.GetCustomerPositiveIndexQueue();
            GameManager.Instance.CustomerNegativeIndexQueue = data.GetCustomerNegativeIndexQueue();
            GameManager.Instance.CustomerOnlyTextIndexQueue = data.GetCustomerOnlyTextIndexQueue();
            GameManager.Instance.CustomerNormalImageQueue = data.GetCustomerNormalImageQueue();
            GameManager.Instance.LoadingIndexQueue = data.GetLoadingIndexQueue();

            

            // 엔딩 처리
            if (data.CurrentStage > 2)//>=이었음 //+1을 GameManager.Instance.MaxLevel+1을 추가함
            {
                Debug.Log("***SaveManager** 현재 레벨이 몇이길래 엔딩으로?" + data.CurrentStage);

                data.NextScene = Scenes.EndingScene;
                Debug.Log("엔딩처리");
            }


            Debug.Log("불러올 씬 이름: " + data.NextScene.ToString());
            return data.NextScene;
        }
        else
        {
            // 데이터 없을 때 초기화
            GameManager.Instance.SetCurrentLevel(0);
            GameManager.Instance.SetPlayerCurrecyData(null);

            Dictionary<string, int> emptyIngredients = new Dictionary<string, int>();
            GameManager.Instance.SetIngredientManage(emptyIngredients);

            GameManager.Instance.CustomerNormalIndexQueue = new Queue<int>();
            GameManager.Instance.CustomerPositiveIndexQueue = new Queue<int>();
            GameManager.Instance.CustomerNegativeIndexQueue = new Queue<int>();
            GameManager.Instance.CustomerOnlyTextIndexQueue = new Queue<int>();
            GameManager.Instance.CustomerNormalImageQueue = new Queue<int>();
            GameManager.Instance.LoadingIndexQueue = new Queue<int>();

            Debug.Log("저장된 데이터 없음 - 기본 Match3Game 씬으로 이동");

            Debug.Log("불러올 씬 이름 (기본값): " + Scenes.Match3Game.ToString());

            return Scenes.Match3Game;


        }
    }




    public static void DeleteSave()
    {
        Debug.LogWarning("DeleteSave 호출됨!!!");

        PlayerPrefs.DeleteKey(SAVE_KEY);
        Debug.Log("저장 데이터 삭제됨");

        // 기본값 설정
        //GameManager.Instance.SetCurrentLevel(0);

        Dictionary<CurrencyType, int> defaultCurrency = new Dictionary<CurrencyType, int>()
    {
        { CurrencyType.Money, 500 },          // 기본 예산
        { CurrencyType.Satisfaction, 50 }     // 기본 만족도
    };
        GameManager.Instance.SetPlayerCurrecyData(defaultCurrency);

        Dictionary<string, int> emptyIngredients = new Dictionary<string, int>();
        GameManager.Instance.SetIngredientManage(emptyIngredients);

        GameManager.Instance.CustomerNormalIndexQueue = new Queue<int>();
        GameManager.Instance.CustomerPositiveIndexQueue = new Queue<int>();
        GameManager.Instance.CustomerNegativeIndexQueue = new Queue<int>();
        GameManager.Instance.CustomerOnlyTextIndexQueue = new Queue<int>();
        GameManager.Instance.CustomerNormalImageQueue = new Queue<int>();
        GameManager.Instance.LoadingIndexQueue = new Queue<int>();

        //스토리 처음부터 나오게
        StorySaveManager.Instance.DeleteSave();
        //GameManager.Instance.SetCurrentLevel(0);
        // GameManager.Instance.LoadLevelData();
        GameManager.Instance.ResetLevel();

        CurrencyManager.Instance.ResetCurrencyData();
        IngredientManager.Instance.ClearAllPlayerPrefsData();

        //만족도 재화 초기화
        CurrencyManager.Instance.SaveCurrencyData();
        CurrencyManager.Instance.LoadCurrencyData();

        //재화 변동 초기화
        CurrencyManager.Instance.ResetCurrencyInOutData();
        CurrencyManager.Instance.SaveCurrencyInOutData();
        CurrencyManager.Instance.LoadCurrencyInOutData();

        //재료 초기화
        IngredientManager.Instance.SaveAllIngredientCounts();
       // IngredientManager.Instance.LoadAllIngredientCounts();

        Debug.Log("현재 스테이지 초기화됨: " + GameManager.Instance.CurrentLevel);

        // 초기 저장
        SaveGame(
             GameManager.Instance.CurrentLevel,  // 이걸 직접 0으로 받도록
            defaultCurrency,
            new Dictionary<IngredientType, uint>(),  // 빈 재료
            Scenes.Match3Game
        );


        Debug.Log("기본값으로 저장 완료");
    }

}