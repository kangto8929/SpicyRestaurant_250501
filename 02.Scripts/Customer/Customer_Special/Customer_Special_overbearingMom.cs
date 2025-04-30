using System.Collections.Generic;
using UnityEngine;

public class Customer_Special_overbearingMom : Customer_Special
{
    protected override void Initialize()
    {
        base.Initialize();

        _requiredIngredients = new Dictionary<IngredientType, uint>()
        {
            {IngredientType.Meat, 9999}
        };

        // 뭘 하든 부정 대답을 한다.
        _positiveText = _negativeText;

        OrderManager.Instance.SetOrder(_requiredIngredients);
    }
    protected override void SpriteInitialize()
    {
        return;
    }
}
