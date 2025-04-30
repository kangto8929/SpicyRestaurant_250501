using System.Collections.Generic;
using UnityEngine;

public enum CustomerTypes
{
    Normal,
    OnlyText,
    Special,


    Count,
}

public class Customer_Base : MonoBehaviour
{


    public CustomerTypes CustomerType;

    [SerializeField]
    protected Dictionary<IngredientType, uint> _requiredIngredients;
    [SerializeField]
    protected int _requiredIngredientsCount = 6;

    [SerializeField]
    protected List<string> _orderText;
    [SerializeField]
    protected string _positiveText = "";
    [SerializeField]
    protected string _negativeText = "";

    [SerializeField]
    protected Sprite _customerSprite;
    protected SpriteRenderer _customerSpriterenderer;

    protected CustomerTextStruct _customerTextStruct;

    protected List<string> _selectableIngredients = new List<string>();


    public virtual List<string> GetText()
    {
        return _orderText;
    }

    // 조건을 만족했을 경우 
    public virtual string GetPositiveText()
    {
        return _positiveText;
    }

    public virtual string GetNegativeText()
    {
        return _negativeText;
    }

    public virtual Dictionary<IngredientType, uint> GetRequiredIngredients()
    {
        return _requiredIngredients;
    }

    protected IngredientType ConvertKoreanStringToIngredientType(string korean)
    {
        IngredientType ingredientType = IngredientType.GlassNoodle;
        switch (korean)
        {
            case "중국당면":
                ingredientType = IngredientType.GlassNoodle;
                break;
            case "새우":
                ingredientType = IngredientType.Shrimp;
                break;
            case "고수":
                ingredientType = IngredientType.Cilantro;
                break;
            case "고기":
                ingredientType = IngredientType.Meat;
                break;
            case "두부":
                ingredientType = IngredientType.Tofu;
                break;
            case "청경채":
                ingredientType = IngredientType.BokChoy;
                break;
            case "팽이버섯":
                ingredientType = IngredientType.EnokiMushroom;
                break;
            case "목이버섯":
                ingredientType = IngredientType.WoodEarMushroom;
                break;
            default:
                Debug.Log("유효하지 않은 타입(허정범)");
                break;
        }
        return ingredientType;
    }


    protected string ConvertIngredientTypeToKoreanString(IngredientType type)
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

    protected void MakeCustomerTextStruct()
    {
        // 텍스트구조체 만들고
        _customerTextStruct.CustomerType = this.CustomerType;
        _customerTextStruct.OrderText = _orderText;
        _customerTextStruct.PositiveText = _positiveText;
        _customerTextStruct.NegativeText = _negativeText;

        // 내보내기
        UI_CustomerText.Instance.SetCustomerText(_customerTextStruct);
    }
}
