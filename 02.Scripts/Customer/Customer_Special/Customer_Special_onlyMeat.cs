using System.Collections.Generic;
using UnityEngine;

public class Customer_Special_onlyMeat : Customer_Special
{
    protected override void Initialize()
    {
        base.Initialize();

        _requiredIngredients = new Dictionary<IngredientType, uint>()
        {
            {IngredientType.Meat, 6}
        };
        OrderManager.Instance.SetOrder(_requiredIngredients);
    }
}
