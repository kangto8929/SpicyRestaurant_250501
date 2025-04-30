using System.Collections.Generic;
using UnityEngine;

public class Customer_Special_itsokDing : Customer_Special
{
    protected override void Initialize()
    {
        base.Initialize();

        _requiredIngredients = new Dictionary<IngredientType, uint>();

        int level = GameManager.Instance.CurrentLevel;

        if (level == 0)
        {
            _requiredIngredients[IngredientType.BokChoy] = 1;
            _requiredIngredients[IngredientType.EnokiMushroom] = 1;
            _requiredIngredients[IngredientType.Meat] = 1;
            _requiredIngredients[IngredientType.Tofu] = 1;
        }
        else if (level == 1)
        {
            _requiredIngredients[IngredientType.BokChoy] = 1;
            _requiredIngredients[IngredientType.EnokiMushroom] = 1;
            _requiredIngredients[IngredientType.Meat] = 1;
            _requiredIngredients[IngredientType.Tofu] = 1;
            _requiredIngredients[IngredientType.GlassNoodle] = 1;
            _requiredIngredients[IngredientType.Cilantro] = 1;
        }

        else
        {
            // 최고 레벨용 (예시)
            _requiredIngredients[IngredientType.BokChoy] = 1;
            _requiredIngredients[IngredientType.EnokiMushroom] = 1;
            _requiredIngredients[IngredientType.Meat] = 1;
            _requiredIngredients[IngredientType.Tofu] = 1;
            _requiredIngredients[IngredientType.GlassNoodle] = 1;
            _requiredIngredients[IngredientType.Cilantro] = 1;
            _requiredIngredients[IngredientType.WoodEarMushroom] = 1;
            _requiredIngredients[IngredientType.Shrimp] = 1;
        }

        OrderManager.Instance.SetOrder(_requiredIngredients);

        /* base.Initialize();

         // 모든 재료 1씩 주문할거다.
         _requiredIngredients = new Dictionary<IngredientType, uint>();
         foreach (string ingredients in _selectableIngredients)
         {
             _requiredIngredients[ConvertKoreanStringToIngredientType(ingredients)] = 1;
         }
         OrderManager.Instance.SetOrder(_requiredIngredients);*/

    }
}
