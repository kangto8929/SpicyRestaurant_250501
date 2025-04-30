using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_IngredientsHolder : MonoBehaviour
{

    public UI_Ingreident UI_IngreidentPrefab;
    public GameObject UI_IngredientDarkPrefab; // 버튼이 없고 어두운 아이콘용 프리팹

    [SerializeField]
    private RectTransform panelTransfrom;

    private List<UI_Ingreident> _ui_IngredientsList;
    private List<Ingredient> _ingredientsList;


    private void Start()
    {
        SetHolderPosition();

        _ui_IngredientsList = new List<UI_Ingreident>();
        _ingredientsList = IngredientManager.Instance.IngredientsList;

        SetContraintCount();

        int ingredientCount = _ingredientsList.Count;
        int darkCount = 14;  // 어두운 프리팹 개수 //19

        // 실제 프리팹 리스트를 만들어서 넣을 순서를 결정
        List<GameObject> prefabs = new List<GameObject>();

        // 1. 필요한 프리팹들을 먼저 넣는다
        for (int i = 0; i < ingredientCount; i++)
        {
            UI_Ingreident uiIngredient = Instantiate(UI_IngreidentPrefab, this.transform);
            uiIngredient.Ingredient = _ingredientsList[i];
            uiIngredient.Refresh();
            _ui_IngredientsList.Add(uiIngredient);
            prefabs.Add(uiIngredient.gameObject);  // 필요한 프리팹 리스트에 추가
        }

        // 2. 그 후에 어두운 프리팹들을 넣는다
        for (int i = 0; i < darkCount; i++)
        {
            GameObject darkPrefab = Instantiate(UI_IngredientDarkPrefab, this.transform);
            prefabs.Add(darkPrefab);  // 어두운 프리팹 리스트에 추가
        }

        // 이제 GridLayoutGroup에서 각 프리팹을 순서대로 배치하도록 한다.
        for (int i = 0; i < prefabs.Count; i++)
        {
            prefabs[i].transform.SetSiblingIndex(i);  // 순서대로 배치
        }
    }

    public void SetHolderPosition()
    {
        panelTransfrom = GetComponent<RectTransform>();
        Vector2 anchoredPos = panelTransfrom.anchoredPosition;
        anchoredPos.y = 0.000381582f;
        panelTransfrom.anchoredPosition = anchoredPos;
    }

    // GridLayoutGroup 설정
    private void SetContraintCount()
    {
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;  // 4열 고정
        }
        else
        {
            Debug.LogWarning("GridLayoutGroup 컴포넌트를 찾을 수 없습니다.");
        }
    }


}

