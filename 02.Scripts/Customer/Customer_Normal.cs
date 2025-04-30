using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.Rendering.DebugUI;

public class Customer_Normal : Customer_Base
{
    [SerializeField]
    private ScriptManager.CustomerNormalTextData _customerNomalTextData;



    private void Start()
    {
        _customerSprite = ScriptManager.Instance.GetRandomCustomerNormalImages();
        _customerSpriterenderer = GetComponent<SpriteRenderer>();
        _customerSpriterenderer.sprite = _customerSprite;
        _selectableIngredients = IngredientManager.Instance.GetIngredientsTypeString();
        // 랜덤으로 요구할 재료 구하기
        SetRequiredIngredients();

        // 주문하기
        OrderManager.Instance.SetOrder(_requiredIngredients);
        MakeCustomerTextStruct();
    }

    private void SetRequiredIngredients()
    {
        _requiredIngredients = new Dictionary<IngredientType, uint>();
        for (int i = 0; i < _requiredIngredientsCount; i++)
        {
            string randomIngredientKorean = _selectableIngredients[Random.Range(0, _selectableIngredients.Count)];
            var randomIngredient = ConvertKoreanStringToIngredientType(randomIngredientKorean);
            if (_requiredIngredients.ContainsKey(randomIngredient))
            {
                _requiredIngredients[randomIngredient]++;
            }
            else
            {
                _requiredIngredients[randomIngredient] = 1;
            }
        }


        SetText();
        var a = _orderText;
        foreach (var i in a)
        {
            Debug.Log(i);
        }
        Debug.Log(GetPositiveText());
        Debug.Log(GetNegativeText());
    }

    

    private void SetText()///******여기서 앞 부분 줄바꿈 할거임.
    {
        _orderText = new List<string>();

        // 대사가 2번 나온다면 먼저 나올 대사
        string _foreText = "";
        _customerNomalTextData = ScriptManager.Instance.GetRandomCustomerText();



        _foreText += _customerNomalTextData.foreword + " ";

        //0415
        //줄바꿈을 했는지 여부
        bool addedLineBreak = false;


        //0415
        foreach (var ingredient in _requiredIngredients)
        {
            // 첫 번째 재료에서만 줄바꿈을 추가
            if (!addedLineBreak)
            {
                _foreText += "\n"; // 첫 번째 재료 앞에 줄바꿈 추가
                addedLineBreak = true; // 줄바꿈을 추가한 후 플래그를 true로 설정
            }

            _foreText += $"{ConvertIngredientTypeToKoreanString(ingredient.Key)} {ingredient.Value}개, ";
        }

        /*foreach (var ingredient in _requiredIngredients)
        {
            _foreText += $"{ConvertIngredientTypeToKoreanString(ingredient.Key)} {ingredient.Value}개, ";
        }*/
        _foreText = _foreText[..^2];
        _foreText += _customerNomalTextData.afterword;

        _orderText.Add(_foreText);

        // 대화가 2번 나온다면 나중에 나올 대사.
        if (_customerNomalTextData.nexttext != "")
        {
            _orderText.Add(_customerNomalTextData.nexttext);
        }

        // 긍정 부정 받아와서 넣어준다.
        var positiveTextData = ScriptManager.Instance.GetRandomCustomerPositive();
        var negativeTextData = ScriptManager.Instance.GetRandomCustomerNegative();
        _positiveText = positiveTextData.positivetext;
        _negativeText = negativeTextData.negativetext;
    }
}
