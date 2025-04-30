using System.Collections.Generic;
using UnityEngine;

public class Customer_Special_chillGuy : Customer_Special
{
    protected override void Initialize()
    {
        base.Initialize();

        _selectableIngredients = IngredientManager.Instance.GetIngredientsTypeString();
        _requiredIngredients = new Dictionary<IngredientType, uint>();
        foreach(string ingredients in _selectableIngredients)
        {
            IngredientType tmp = ConvertKoreanStringToIngredientType(ingredients);
            if(tmp == IngredientType.Cilantro)
            {
                continue;
            }
            _requiredIngredients[tmp] = 1;
        }

        // 칠 가이는 부정 대답을 하지 않는다.
        _negativeText = _positiveText;
        // 주문하기
        OrderManager.Instance.SetOrder(_requiredIngredients);
    }
    protected override void SpriteInitialize()
    {
        return;
    }

}
