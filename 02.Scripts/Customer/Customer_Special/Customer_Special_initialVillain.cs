using UnityEngine;
using System.Collections.Generic;

public class Customer_Special_initialVillain : Customer_Special
{
    protected override void Initialize()
    {
        base.Initialize();


        _requiredIngredients = new Dictionary<IngredientType, uint>()
        {
            {IngredientType.Meat, 1}, {IngredientType.Tofu, 1}, {IngredientType.EnokiMushroom, 1}, {IngredientType.BokChoy, 1}, {IngredientType.Cilantro, 1}
        };
        OrderManager.Instance.SetOrder(_requiredIngredients);
    }
}
