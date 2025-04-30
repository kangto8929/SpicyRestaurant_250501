using System;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    public static IngredientManager Instance;

    public List<Ingredient> IngredientsList = new List<Ingredient>();

    [SerializeField]
    private List<IngredientDataSO> _ingredientDataSOList;

    // ingredientCounts를 private로 유지하고, 외부에서 접근할 수 있도록 프로퍼티 제공
    private Dictionary<IngredientType, int> _ingredientCounts = new Dictionary<IngredientType, int>();

    public Dictionary<IngredientType, int> IngredientCounts
    {
        get { return _ingredientCounts; }
    }

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

        InitIngredientList();

       //SaveAllIngredientCounts();
       LoadAllIngredientCounts();

    }

    private void Start()
    {
        
        SetIngridentsAmount(GameManager.Instance.GetIngredientData());
    }

    
    private void InitIngredientList()
    {
        List<IngredientType> enabledIngredients = GameManager.Instance.GetEnableIngredients();

        foreach (IngredientDataSO ingredientData in _ingredientDataSOList)
        {
            for (int i = 0; i < enabledIngredients.Count; i++)
            {
                if (enabledIngredients[i] == ingredientData.Type)
                {
                    Ingredient ingredient = new Ingredient(ingredientData, 0);
                    IngredientsList.Add(ingredient);
                    // Initialize ingredient count to 0
                    _ingredientCounts[ingredientData.Type] = 0;
                }
            }
        }
    }

    public Ingredient GetIngredient(IngredientType type)
    {
        foreach (Ingredient ingredient in IngredientsList)
        {
            if (type == ingredient.Type)
            {
                return ingredient;
            }
        }

        Debug.Log($"{type} 재료가 없습니다!");
        return null;
    }

    public List<string> GetIngredientsTypeString()
    {
        if (IngredientsList.Count == 0)
        {
            Debug.Log("IngredeintsList가 비어있습니다. (담당자: 이형근)");
            return null;
        }

        List<string> selectableIngredients = new List<string>();
        foreach (Ingredient ingredient in IngredientsList)
        {
            if (ingredient.IsBasic)
            {
                continue;
            }
            string ingredientName = ConvertIngredientTypeToKoreanString(ingredient.Type);

            selectableIngredients.Add(ingredientName);
        }

        return selectableIngredients;
    }

    public void SetIngridentsAmount(Dictionary<IngredientType, uint> ingredientsDict)
    {
        foreach (Ingredient ingredient in IngredientsList)
        {
            if (ingredient.IsBasic)
            {
                ingredient.Amount = 0;
                continue;
            }

            if (!ingredientsDict.TryGetValue(ingredient.Type, out ingredient.Amount))
            {
                Debug.Log($"{ingredient.Type} 개수 할당 실패하였습니다. (담당자: 이형근)");
            }
        }
    }

    public void AddAllIngredient(IngredientType type, int amount)
    {
        if (_ingredientCounts.ContainsKey(type))
        {
            _ingredientCounts[type] += amount;
        }
        else
        {
            _ingredientCounts[type] = amount;
        }
    }

    public int GetAllIngredientCount(IngredientType type)
    {
        if (_ingredientCounts.ContainsKey(type))
        {
            return _ingredientCounts[type];
        }
        return 0;
    }

    public void SaveAllIngredientCounts()
    {
        Dictionary<string, int> dict = new();

        foreach (var ingredient in IngredientsList)
        {
            dict[ingredient.Type.ToString()] = (int)ingredient.Amount;
            Debug.Log($"재료 저장됨 {ingredient.Type}: {ingredient.Amount}");
        }

        GameManager.Instance.SetIngredientManage(dict);
        LoadAllIngredientCounts();
    }


    //정말 잘 됐었음
    /*public void LoadAllIngredientCounts()//이거 된다
    {

        Dictionary<IngredientType, uint> savedCounts = GameManager.Instance.GetIngredientData();

        foreach (var ingredient in IngredientsList)
        {
            if (savedCounts.TryGetValue(ingredient.Type, out uint savedAmount))
            {
                ingredient.Amount = savedAmount;
                Debug.Log($"재료 로드됨 {ingredient.Type} → {ingredient.Amount}");
            }
        }
    }*/

    public void LoadAllIngredientCounts()
    {
        Dictionary<IngredientType, uint> savedCounts = GameManager.Instance.GetIngredientData();

        foreach (var ingredient in IngredientsList)
        {
            // 저장된 값이 있으면 그걸 쓰고, 없으면 0으로 초기화
            if (!savedCounts.TryGetValue(ingredient.Type, out uint savedAmount))
            {
                savedAmount = 0;
                Debug.Log($"저장된 데이터 없음: {ingredient.Type}, 0으로 초기화됨");
            }

            ingredient.Amount = savedAmount;

            Debug.Log($"재료 로드됨 {ingredient.Type} → {ingredient.Amount}");
        }
    }



    //삭제
    public void ClearAllPlayerPrefsData()
    {
        // 모든 재료 데이터 삭제
        foreach (var ingredient in IngredientsList)
        {
            string key = ingredient.Type.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                Debug.Log($"{key} 데이터가 PlayerPrefs에서 삭제되었습니다.");
            }
        }

        // 변경 사항을 저장
        SaveAllIngredientCounts();
       // LoadAllIngredientCounts();
    }

    private string ConvertIngredientTypeToKoreanString(IngredientType type)
    {
        string ingredientName = "";
        switch (type)
        {
            case IngredientType.GlassNoodle:
                ingredientName = "중국당면";
                break;

            case IngredientType.Shrimp:
                ingredientName = "새우";
                break;

            case IngredientType.Cilantro:
                ingredientName = "고수";
                break;

            case IngredientType.Meat:
                ingredientName = "고기";
                break;

            case IngredientType.Tofu:
                ingredientName = "두부";
                break;

            case IngredientType.BokChoy:
                ingredientName = "청경채";
                break;

            case IngredientType.EnokiMushroom:
                ingredientName = "팽이버섯";
                break;

            case IngredientType.WoodEarMushroom:
                ingredientName = "목이버섯";
                break;

            default:
                Debug.Log("유효하지 않은 타입입니다. (담당자: 이형근)");
                break;
        }

        return ingredientName;
    }


}



/*using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    public static IngredientManager Instance;

    public List<Ingredient> IngredientsList = new List<Ingredient>();

    [SerializeField]
    private List<IngredientDataSO> _ingredientDataSOList;



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

        InitIngredientList();
    }

    private void Start()
    {
        SetIngridentsAmount(GameManager.Instance.GetIngredientData());
    }


    private void InitIngredientList()
    {
        List<IngredientType> enabledIngredients = GameManager.Instance.GetEnableIngredients();

        foreach (IngredientDataSO ingredientData in _ingredientDataSOList)
        {
            for (int i = 0; i < enabledIngredients.Count; i++)
            {
                if (enabledIngredients[i] == ingredientData.Type)
                {
                    Ingredient ingredient = new Ingredient(ingredientData, 0);
                    IngredientsList.Add(ingredient);
                }
            }
        }
    }

    public Ingredient GetIngredient(IngredientType type)
    {
        foreach (Ingredient ingredient in IngredientsList)
        {
            if (type == ingredient.Type)
            {
                return ingredient;
            }
        }

        Debug.LogWarning($"{type} 재료가 없습니다!");
        return null;
    }

    public List<string> GetIngredientsTypeString()
    {
        if(IngredientsList.Count == 0)
        {
            Debug.Log("IngredeintsList가 비어있습니다. (담당자: 이형근)");
            return null;
        }

        List<string> selectableIngredients = new List<string>();
        foreach(Ingredient ingredient in IngredientsList)
        {
            if(ingredient.IsBasic)
            {
                continue;
            }
            string ingredientName = ConvertIngredientTypeToKoreanString(ingredient.Type);

            selectableIngredients.Add(ingredientName);
        }

        return selectableIngredients;
    }

    public void SetIngridentsAmount(Dictionary<IngredientType, uint> ingredientsDict)
    {
        foreach(Ingredient ingredient in IngredientsList)
        {
            if(ingredient.IsBasic)
            {
                ingredient.Amount = 0;
                continue;
            }

            if(!ingredientsDict.TryGetValue(ingredient.Type, out ingredient.Amount))
            {
                Debug.Log($"{ingredient.Type} 개수 할당 실패하였습니다. (담당자: 이형근)");
            }
        }
    }

    private string ConvertIngredientTypeToKoreanString(IngredientType type)
    {
        string ingredientName = "";
        switch (type)
        {
            case IngredientType.GlassNoodle:
                ingredientName = "중국당면";
                break;

            case IngredientType.Shrimp:
                ingredientName = "새우";
                break;

            case IngredientType.Cilantro:
                ingredientName = "고수";
                break;

            case IngredientType.Meat:
                ingredientName = "고기";
                break;

            case IngredientType.Tofu:
                ingredientName = "두부";
                break;

            case IngredientType.BokChoy:
                ingredientName = "청경채";
                break;

            case IngredientType.EnokiMushroom:
                ingredientName = "팽이버섯";
                break;

            case IngredientType.WoodEarMushroom:
                ingredientName = "목이버섯";
                break;

            default:
                Debug.Log("유효하지 않은 타입입니다. (담당자: 이형근)");
                break;
        }

        return ingredientName;
    }

}*/
