using System.Collections.Generic;
using UnityEngine;

public class Customer_Special_quizVillain : Customer_Special
{
    protected override void Initialize()
    {
        base.Initialize();

        _requiredIngredients = new Dictionary<IngredientType, uint>()
        {
            {IngredientType.Meat, 2}, {IngredientType.BokChoy, 1}
        };
        OrderManager.Instance.SetOrder(_requiredIngredients);
    }

    protected override void SpriteInitialize()
    {
        return;
    }
}
